using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace dotGit.Config
{
  public class Configuration
  {

    #region Fields

    Dictionary<string, NameValueCollection> data =
       new Dictionary<string, NameValueCollection>();


    private Core _core = new Core();
    private User _user = new User();
    private Remotes _remotes = new Remotes();
    private Branches _branches = new Branches();

    #endregion

    #region Constructors

    private Configuration(string path, bool load)
    {
      Path = path;

      if (load)
      {
        ReadConfiguration();
        ParseData();
      }
    }

    public static Configuration Load(string path)
    {
      return new Configuration(path, true);
    }

    public static Configuration Create(string newConfigPath)
    {
      return new Configuration(newConfigPath, false);
    }

    #endregion

    #region Parsing

    private void ParseData()
    {
      foreach (string key in data.Keys)
      {
        switch (key.ToLower())
        {
          case "core":
            _core = new Core(data[key]);
            break;
          case "user":
            _user = new User(data[key]);
            break;
          default:
            if (key.StartsWith("remote"))
              _remotes.Add(key, new Remote(key, data[key]));

            if (key.StartsWith("branch"))
              _branches.Add(new Branch(key, data[key]));

            break;
        }
      }
    }

    static readonly Regex regRemoveEmptyLines =
        new Regex
        (
            @"(\s*;[\d\D]*?\r?\n)+|\r?\n(\s*\r?\n)*",
            RegexOptions.Multiline | RegexOptions.Compiled
        );

    static readonly Regex regParseconfig =
        new Regex
        (
            @"
                (?<IsSection>
                    ^\s*\[(?<SectionName>[^\]]+)?\]\s*$
                )
                |
                (?<IsKeyValue>
                    ^\s*(?<Key>[^(\s*\=\s*)]+)?\s*\=\s*(?<Value>[\d\D]*)$
                )",
            RegexOptions.Compiled |
            RegexOptions.IgnoreCase |
            RegexOptions.IgnorePatternWhitespace
        );


    #endregion

    private void ReadConfiguration()
    {
      string config = File.ReadAllText(Path);



      string lastSection = String.Empty;
      config = regRemoveEmptyLines.Replace(config, "\n");

      string[] lines =
          config.Split
          (
              new char[] { '\n' },
              StringSplitOptions.RemoveEmptyEntries
          );

      foreach (string s in lines)
      {
        Match m = regParseconfig.Match(s);
        if (m.Success)
        {
          if (m.Groups["IsSection"].Length > 0)
          {
            string sName =
                m.Groups["SectionName"].Value.
                ToLowerInvariant();

            if (lastSection != sName)
            {
              lastSection = sName;
              if (!data.ContainsKey(sName))
              {
                data.Add
                (
                    sName,
                    new NameValueCollection()
                );
              }
            }
          }
          else if (m.Groups["IsKeyValue"].Length > 0)
          {
            data[lastSection].Add
            (
                m.Groups["Key"].Value,
                m.Groups["Value"].Value
            );
          }
        }
      }
    }


    #region Properties

    public Core Core
    {
      get
      {
        return _core;
      }
    }
    public User User
    {
      get
      {
        return _user;
      }
    }

    public Remotes Remotes
    {
      get { return _remotes; }
    }

    public Branches Branches
    {
      get { return _branches; }
    }

    public string Path
    {
      get;
      private set;
    }
    #endregion


    #region Methods
    public void Save()
    {
      throw new NotImplementedException();
    }

    public void Reload()
    {
      ReadConfiguration();
      ParseData();
    }
    #endregion

  }
}
