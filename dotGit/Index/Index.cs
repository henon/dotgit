using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotGit.Generic;
using System.IO;
using dotGit.Exceptions;
using dotGit.Objects;
using dotGit.Objects.Storage;

namespace dotGit.Index
{
  public class Index : IEnumerable<IndexEntry>
  {
    private static readonly string HEADER = "DIRC";
    private static readonly int[] VERSIONS = new int[] { 2 };

    internal Index(Repository repo)
    {
      Repo = repo;
      
      using (GitObjectReader stream = new GitObjectReader(
        new FileStream(Path.Combine(Repo.GitDir.FullName, "index"), System.IO.FileMode.Open, FileAccess.Read, FileShare.Read, 8192)))
      {
        string header = stream.ReadBytes(4).GetString();

        if (header != HEADER)
          throw new ParseException("Could not parse Index file. Expected HEADER: '{0}', got: '{1}'".FormatWith(HEADER, header));

        Version = stream.ReadBytes(4).Sum(b => (int)b);

        if (!VERSIONS.Contains(Version))
          throw new ParseException("Unknown version number {0}. Needs to be one of: {1}".FormatWith(Version, String.Join(",", VERSIONS.Select(i => i.ToString()).ToArray())));


        NumberOfEntries = System.Net.IPAddress.HostToNetworkOrder(stream.ReadBytes(4).ToInt());

        Entries = new IndexEntryCollection(NumberOfEntries);


        for (int i = 0; i < NumberOfEntries; i++)
        {
          Entries.Add(new IndexEntry(stream));
        }

        stream.Position = stream.BaseStream.Length - 20;
        string indexSHA = stream.ReadToEnd().ToSHAString();
      }
    }


    #region Properties

    private Repository Repo
    { get; set; }

    /// <summary>
    /// Version of this index
    /// </summary>
    public int Version
    {
      get;
      private set;
    }

    /// <summary>
    /// Number of entries in this index
    /// </summary>
    public int NumberOfEntries
    {
      get;
      private set;
    }

    /// <summary>
    /// All entries in the index
    /// </summary>
    public IndexEntryCollection Entries
    {
      get;
      private set;
    }

    public bool StageIsNormal
    {
      get { return !Entries.Any(e => e.Stage != IndexStage.Normal); }
    }

    #endregion

    #region Enumerator / Index / Contains

    public bool Contains(string sha)
    {
      throw new NotImplementedException();
    }

    public IndexEntry this[string sha]
    {
      get
      {
        throw new NotImplementedException("To Implement");
      }
    }
    
    public IEnumerator<IndexEntry> GetEnumerator()
    {
      return Entries.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return Entries.GetEnumerator();
    }

    #endregion

    #region Public Methods

    public List<string> ChangedFiles()
    {
      List<string> list = new List<string>();

      foreach (IndexEntry entry in this)
      {
        DateTime lmt = File.GetLastWriteTimeUtc(Path.Combine(Repo.RepositoryDir.FullName, entry.Path));
        
        
        // Windows only stores seconds so we need to correct the git timestamp for that.

        long entryTicks = entry.Modified.Ticks;
        long lastm = lmt.Ticks;
        if (entryTicks % 1000 == 0)
          lastm = lastm - lastm % 10000000;

        if (lastm != entryTicks)
        {
          TimeSpan ts = lmt - Utility.UnixEPOCH;
          TimeSpan ts2 = entry.Modified - Utility.UnixEPOCH;
          list.Add(entry.Path);
        }
      }

      return list;
    }

    #endregion

    
  }
}
