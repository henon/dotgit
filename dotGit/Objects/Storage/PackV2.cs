using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dotGit.Exceptions;
using dotGit.Generic;
using dotGit.Objects.Storage.PackObjects;
using System.Diagnostics;
using Winterdom.IO.FileMap;


namespace dotGit.Objects.Storage
{
  public class PackV2 : Pack
  {
    private MemoryMappedFile _map;
    private int length;


    #region Constructors / Destructor

    internal PackV2(Repository repo, string path)
      : base(repo, path)
    {
      Index = new PackIndexV2(IndexFilePath);
      Pack = new PackFileV2(Repo, PackFilePath);

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

      _map = MemoryMappedFile.Create(PackFilePath, MapProtection.PageReadOnly, info.Length, name);

    }

    public override IStorableObject GetObject(string sha)
    {
      Debug.WriteLine("Fetching object with sha: {0}".FormatWith(sha));

      try
      {
        if (Index != null)
        {
          long packFileOffset = Index.GetPackFileOffset(new Sha(sha));

          using (GitObjectReader reader = new GitObjectReader(_map.MapView(MapAccess.FileMapRead, 0, length)))
          {
            reader.Position = packFileOffset;
            PackObject obj = Pack.GetObjectWithOffset(reader);

            if (obj is Undeltified)
            {
              return ((Undeltified)obj).ToGitObject(Repo, sha);
            }
            else if (obj is Deltified)
            {
              List<Deltified> deltas = new List<Deltified>();
              deltas.Add((Deltified)obj);
              while (obj is Deltified)
              {
                if (obj is REFDelta)
                {
                  string baseSha = ((REFDelta)obj).BaseSHA;
                  packFileOffset = Index.GetPackFileOffset(new Sha(baseSha));
                }
                else
                {
                  packFileOffset -= ((OFSDelta)obj).BackwardsBaseOffset;
                }
                reader.Position = packFileOffset;

                obj = Pack.GetObjectWithOffset(reader);

                if (obj is Deltified)
                  deltas.Add((Deltified)obj);
              }

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
