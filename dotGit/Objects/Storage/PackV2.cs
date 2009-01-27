using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dotGit.Exceptions;
using dotGit.Generic;
using dotGit.Objects.Storage.PackObjects;
using Winterdom.IO.FileMap;


namespace dotGit.Objects.Storage
{
  public class PackV2 : Pack
  {
    private MemoryMappedFile _map;
    private long _packFileLength;
    private Win32.SYSTEM_INFO _systemInfo;

    #region Constructors / Destructor

    internal PackV2(Repository repo, string path)
      : base(repo, path)
    {
      Index = new PackIndexV2(IndexFilePath);

      ReverseIndex = new PackReverseIndexV2(Index);

      Pack = new PackFileV2(Repo, PackFilePath);

      _systemInfo = new Win32.SYSTEM_INFO();
      Win32.GetSystemInfo(ref _systemInfo);

      MapPackFile();


    }

    ~PackV2()
    {
      if (_map != null)
        _map.Close();
      _map = null;
    }

    #endregion

    #region Properties

    public override int Version
    {
      get { return 2; }
    }

    private string PackFilePath
    {
      get { return Path + ".pack"; }
    }

    private string IndexFilePath
    {
      get { return Path + ".idx"; }
    }

    private PackIndexV2 Index
    {
      get;
      set;
    }

    private PackReverseIndexV2 ReverseIndex
    {
      get;
      set;
    }

    private PackFileV2 Pack
    {
      get;
      set;
    }

    #endregion

    private void MapPackFile()
    {
      // create a unique name for this memory map
      string name = "MX=" + Guid.NewGuid().ToString();

      // determine file size and create a memory map from the Pack file.
      FileInfo info = new FileInfo(PackFilePath);
      _packFileLength = info.Length;

      _map = MemoryMappedFile.Create(PackFilePath, MapProtection.PageReadOnly, _packFileLength, name);

    }


    private Stream GetPackStream(int packFileOffset, int length, ref int viewOffset)
    {
      
      int dwFileMapStart = (packFileOffset / (int)_systemInfo.dwAllocationGranularity) * (int)_systemInfo.dwAllocationGranularity;
      int dwMapViewSize = (packFileOffset % (int)_systemInfo.dwAllocationGranularity) + length;
      int dwFileMapSize = packFileOffset + length;
       viewOffset = packFileOffset - dwFileMapStart;
      return _map.MapView(MapAccess.FileMapRead, dwFileMapStart, dwMapViewSize);

    }

    public override IStorableObject GetObject(string sha)
    {
      System.Diagnostics.Debug.WriteLine("Fetching object with sha: {0}".FormatWith(sha));

      try
      {
        if (Index != null)
        {
          int packFileOffset = Index.GetPackFileOffset(new Sha(sha));

          int nextOffset = ReverseIndex.GetNextOffset(packFileOffset, (int)_packFileLength);

          int viewOffset =0;

          using (GitObjectReader reader = new GitObjectReader(GetPackStream(packFileOffset,nextOffset-packFileOffset, ref viewOffset)))
          {
            reader.Position = viewOffset;
            PackObject obj = Pack.GetObjectWithOffset(reader, nextOffset - packFileOffset);

            if (obj is Undeltified)
            {
              return ((Undeltified)obj).ToGitObject(Repo, sha);
            }
            else if (obj is Deltified)
            {
              // Loop while we're still finding delta's and collect them

              // Kick it off with the object found above
              List<Deltified> deltas = new List<Deltified>();
              deltas.Add((Deltified)obj);


              while (obj is Deltified)
              {
                if (obj is REFDelta)
                { // It's a REFDelta, we need fetch an offset from the idx file
                  string baseSha = ((REFDelta)obj).BaseSHA;
                  packFileOffset = Index.GetPackFileOffset(new Sha(baseSha));
                }
                else
                { // It's an OFSDelta, apply it to current packFileOffset
                  packFileOffset -= ((OFSDelta)obj).BackwardsBaseOffset;
                }


                nextOffset = ReverseIndex.GetNextOffset(packFileOffset, (int)_packFileLength);

                using (GitObjectReader deltaReader = new GitObjectReader(GetPackStream(packFileOffset, nextOffset - packFileOffset, ref viewOffset)))
                {

                  // Set stream offset to new object's offset
                  deltaReader.Position = viewOffset;
                  // Fetch next object
                  obj = Pack.GetObjectWithOffset(deltaReader, nextOffset-packFileOffset);
                }

                // Collect delta
                if (obj is Deltified)
                  deltas.Add((Deltified)obj);
              }

              // Whole object found! Apply found delta's starting with the last one we've found
              for (int i = deltas.Count - 1; i >= 0; i--)
              {
                ((Undeltified)obj).ApplyDelta(deltas[i]);
              }


              return ((Undeltified)obj).ToGitObject(Repo, sha);
            }
            else
            {
              throw new ApplicationException("Don't know what to do with: {0}".FormatWith(obj.GetType().FullName));
            }
          }
        }
        else
        {
          return Pack.GetObject(sha);
        }
      }
      catch (Exception ex)
      {
        throw new PackFileException("Something went wrong while fetching object: {0}".FormatWith(sha), PackFilePath, ex);
      }
    }
  }
}
