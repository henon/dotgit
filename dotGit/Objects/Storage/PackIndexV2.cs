using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotGit.Exceptions;
using System.IO;
using dotGit.Generic;

namespace dotGit.Objects.Storage
{
  internal class PackIndexV2 : IEnumerable<PackIndexEntry>
  {
    private static readonly string MAGIC_NUMBER = Encoding.ASCII.GetString(new byte[] { 255, 116, 79, 99 });

    private static readonly int FANOUT = 256;

    protected int[] fanout;
    protected int[][] shas;
    protected byte[][] crcs;
    protected byte[][] offsets;





    internal PackIndexV2(string path)
    {
      Path = path;

      Load();
    }

    #region Properties

    public int Version
    {
      get { return 2; }
    }

    public string Path
    {
      get;
      private set;
    }

    public int NumberOfObjects
    {
      get;
      protected set;
    }

    #endregion



    public PackIndexEntry GetEntry(int index)
    {
      int levelOne = Array.BinarySearch(fanout, index+1);
      int level;
      if( levelOne >= 0 )
      {
        level = fanout[levelOne];
        while(levelOne > 0 && level == fanout[levelOne-1])
          levelOne--;
      }
      else{
        levelOne = -(levelOne+1);
      }

      level = levelOne > 0 ? fanout[levelOne-1] : 0;
      int position = index - level;

      return new PackIndexEntry(shas[levelOne], (position << 2) + position);
    
    }

    private void Load()
    {
      string magicNumber;
      int version;

      using (GitObjectReader reader = new GitObjectReader(File.OpenRead(Path)))
      {
        magicNumber = reader.ReadBytes(4).GetString();
        version = reader.ReadBytes(4).Sum(b => b);

        if (Version != version)
          throw new PackFileException(String.Format("Bad version number {0}. Was expecting {1}", version, Version), Path);

        if (MAGIC_NUMBER != magicNumber)
          throw new PackFileException("Invalid header for pack-file. Needs to be: 'PACK'", Path);

        #region Fanout Table

        fanout = new int[FANOUT];
        shas = new int[FANOUT][];
        offsets = new byte[FANOUT][];
        crcs = new byte[FANOUT][];

        // one big read, faster as 256 small 4 byte read statements ?
        byte[] fanoutRaw = new byte[FANOUT * 4];
        fanoutRaw = reader.ReadBytes(FANOUT * 4);

        for (int idx = 0; idx < FANOUT; idx++)
          fanout[idx] = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(fanoutRaw, idx * 4));


        #endregion

        NumberOfObjects = fanout[FANOUT - 1];


        #region SHA's

        for (int idx = 0; idx < FANOUT; idx++)
        {
          int bucketCount;

          if (idx == 0)
            bucketCount = fanout[idx];
          else
            bucketCount = fanout[idx] - fanout[idx - 1];

          if (bucketCount == 0)
          {
            shas[idx] = new int[] { };
            crcs[idx] = new byte[] { };
            offsets[idx] = new byte[] { };
            continue;
          }


          int recordLength = bucketCount * 20;

          int[] bin = new int[recordLength >> 2];
          byte[] rawRecord = reader.ReadBytes(recordLength);

          for (int i = 0; i < bin.Length; i++)
          {
            bin[i] = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(rawRecord, i << 2));
          }

          shas[idx] = bin;

          offsets[idx] = new byte[bucketCount * 4];
          crcs[idx] = new byte[bucketCount * 4];

        }


        #endregion

        #region CRC32

        for (int idx = 0; idx < FANOUT; idx++)
          crcs[idx] = reader.ReadBytes(crcs[idx].Length);

        #endregion

        #region 32 bit offset table

        for (int idx = 0; idx < FANOUT; idx++)
          offsets[idx] = reader.ReadBytes(offsets[idx].Length);

        #endregion

        // TODO: Support 64 bit tables

        string packChecksum = reader.ReadBytes(20).GetString();
        string idxChecksum = reader.ReadBytes(20).GetString();

      }
    }

    private int SearchLevelTwo(Sha sha, int index)
    {
      int[] data = shas[index];

      int high = offsets[index].Length >> 2;
      if (high == 0)
        return -1;

      int low = 0;
      while (low < high)
      {
        int mid = (low + high) >> 1;
        int mid4 = mid << 2;
        int cmp;

        cmp = sha.CompareTo(data, mid4 + mid);
        if (cmp < 0)
          high = mid;
        else if (cmp == 0)
          return mid;
        else
          low = mid + 1;
      }


      return -1;
    }


    /// <summary>
    /// returns the byte offset in the pack index of the given Sha object
    /// </summary>
    /// <param name="sha"></param>
    /// <returns>offset (int)</returns>
    public int GetPackFileOffset(Sha sha)
    {
      int levelTwo = SearchLevelTwo(sha, sha.FirstByte);

      if (levelTwo == -1)
        return -1;

      return System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(offsets[sha.FirstByte], levelTwo << 2));
    }


    #region Methods

    /// <summary>
    /// Returns true/false indicating if the given Sha is present in the PackIndexEntry collection of this Repository
    /// </summary>
    /// <param name="sha"></param>
    /// <returns>boolean</returns>
    public bool Contains(Sha sha)
    {
      return (GetPackFileOffset(sha) != -1);
    }

    #endregion


    #region IEnumerable<PackIndexEntry> Members

    public IEnumerator<PackIndexEntry> GetEnumerator()
    {

      return new PackIndexEntryEnumerator(this);
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return new PackIndexEntryEnumerator(this);
    }

    #endregion




    public class PackIndexEntryEnumerator : IEnumerator<PackIndexEntry>
    {


      private PackIndexEntry _entry = null;
      private int returnedEntries = 0;

      private int levelOne = 0;
      private int levelTwo = 0;

      private PackIndexV2 _index;

      public PackIndexEntryEnumerator(PackIndexV2 index)
      {
        _index = index;
      }


      #region IEnumerator<PackIndexEntry> Members

      public PackIndexEntry Current
      {
        get { return _entry; }
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        _entry = null;
      }

      #endregion

      #region IEnumerator Members

      object System.Collections.IEnumerator.Current
      {
        get { return _entry; }
      }

      public bool MoveNext()
      {
        bool result = false;
        for (; levelOne < _index.shas.Length; levelOne++)
        {
          if (levelTwo < _index.shas[levelOne].Length)
          {
            
            _entry = new PackIndexEntry(_index.shas[levelOne],levelTwo);
          
            int arrayIndex = levelTwo / (Sha.ShaByteLength / 4) * 4;

            int offs = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(_index.offsets[levelOne], arrayIndex));
            
            _entry.Offset = offs;
            levelTwo += Sha.ShaByteLength / 4;
            returnedEntries++;
            result = true;
            break;
          }
          else
          {
            levelTwo = 0;
          }
        }
        return result;
      }

      public void Reset()
      {
        _entry = null;

        returnedEntries = 0;
        levelTwo = 0;
        levelOne = 0;
      }

      #endregion
    }

  }




  public class PackIndexEntry : Sha
  {

    public PackIndexEntry(string sha)
      : base(sha)
    {
    }

    public PackIndexEntry(int[] words)
      : base(words)
    {
    }

    public PackIndexEntry(int[] input, int position)
      : base(input, position)
    {
    }

    public int Offset
    {
      get;
      internal set;
    }
  }
}
