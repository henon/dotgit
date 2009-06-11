using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.ComponentModel;

namespace dotGit.Config
{
  /// <summary>
  /// Configuration class to represent global and repository scoped Git settings. Parts of this code were derived from the 
  /// INI file handler described here : http://www.codeproject.com/KB/files/TA_INIDocument_cs.aspx
  /// 
  /// Global configuration is read/saved on demand. Repository configuration is loaded by instantiating this class and only saved after calling Save() for now.
  /// </summary>
  public class Configuration
  {

    #region Static / Global Configuration

    private static string GlobalConfigurationFile
    {
      get
      {
        return System.IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".gitconfig");
      }
    }

    internal static volatile string lockString = "LOCKSTRING";

    internal static Configuration GlobalConfiguration
    {
      get
      {
        return new Configuration(GlobalConfigurationFile, true, true);
      }
    }

    #endregion

    #region Fields

    private bool _isGlobal = false;
    internal bool IsGlobal { get { return _isGlobal; } }

    internal Dictionary<string, NameValueCollection> Data =
       new Dictionary<string, NameValueCollection>();

    public IEnumerable<Section> Sections
    {
        get
        {
            return Data.Select(pair => new Section(this, pair.Key, pair.Value)).ToArray();
        }
    }
    #endregion

    #region Properties

    public string Path
    {
      get;
      private set;
    }

    #endregion

    #region Constructors

    private Configuration(string path)
    {
      Path = path;
    }

    private Configuration(string path, bool load, bool global)
      : this(path)
    {
      _isGlobal = global;

      if (load)
      {
        ReadConfiguration();
      }
      else
      {
        InitValues();
      }

    }

    /// <summary>
    /// This method is called by the Configuration constructor to set initial configuration values. Add your own defaults in here.
    /// </summary>
    private void InitValues()
    {
      this.SetValue(ConfigSections.Core, ConfigKeys.FileMode, false);
      this.SetValue(ConfigSections.Core, ConfigKeys.RepositoryFormatVersion,0);
      this.SetValue(ConfigSections.Core, ConfigKeys.IgnoreCase, false);
      this.SetValue(ConfigSections.Core, ConfigKeys.Bare, false);
    }

    public static Configuration Load(string path)
    {
      return new Configuration(path, true, false);
    }

    public static Configuration Create(string newConfigPath)
    {
      if (File.Exists(newConfigPath))
        throw new ArgumentException("Config file already exists at {0}. Use Configuration.Load() instead.".FormatWith(newConfigPath), "newConfigPath");

      return new Configuration(newConfigPath, false, false);
    }

    #endregion

    #region Getting Values 

    public T GetValue<T>(ConfigSections section, ConfigKeys key)
    {
      return GetValue<T>(section.ToString().ToLower(), key.ToString().ToLower());
    }

    public T GetValue<T>(ConfigSections section, string key)
    {
      return GetValue<T>(section.ToString().ToLower(), key);
    }

    public T GetValue<T>(ConfigSections section, string subsection, ConfigKeys key)
    {
      return GetValue<T>(section.ToString().ToLower(), subsection,key.ToString().ToLower());
    }

    public T GetValue<T>(string section, string key)
    {
      return GetValue<T>(section, String.Empty, key);
    }
    /// <summary>
    /// Returns a value of type <T> from the Git Configuration file. If a value doesn't exist at repositoUTFry level, the global configuration file
    /// is read. When no value exists for a given key, the default for type T is returned.
    /// </summary>
    /// <typeparam name="T">Type to return from the configuration file</typeparam>
    /// <param name="section">Git configuration file section key (example: [core]). Pass parameter without brackets</param>
    /// <param name="subsection">Git subsection. (example: "origin" in the key [remote "origin"]. Pass parameter without brackets</param>
    /// <param name="key">Git configuration key (example : "email", together with section parameter "user", this will return the users email address.</param>
    /// <returns>Effective configuration value of type <T>/></returns>
    public T GetValue<T>(string section, string subsection, string key)
    {
      string sKey = section;
      if (!String.IsNullOrEmpty(subsection))
        sKey = section + " \"" + subsection + "\"";

      string value = String.Empty;
      if (Data.Keys.Contains(sKey) && Data[sKey][key] != null)
        value = Data[sKey][key];
      else
      {
        if(!IsGlobal)
          value = GlobalConfiguration.GetValue<string>(section, subsection, key);
      }

      TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
      if (tc.CanConvertFrom(typeof(String)))
        return (T)tc.ConvertFromString(value);
      else
      {
        return default(T);
      }
    }

    #endregion

    #region Setting Values

    public void SetValue(ConfigSections section, string key, object value)
    {
      SetValue(section.ToString().ToLower(), key, value);
    }

    public void SetValue(ConfigSections section, ConfigKeys key, object value)
    {
      SetValue(section.ToString().ToLower(), key.ToString().ToLower(), value);
    }

    public void SetValue(string section, string key, object value)
    {
      SetValue(false, section, String.Empty, key, value);
    }





    public void SetValue(ConfigSections section, string subsection, ConfigKeys key, object value)
    {
      SetValue(false, section.ToString().ToLower(), subsection, key.ToString().ToLower(), value);
    }
    
    public void SetValue(bool global, ConfigSections section, string subsection, ConfigKeys key, object value)
    {
      SetValue(global, section.ToString().ToLower(), subsection, key.ToString().ToLower(), value);
    }

    public void SetValue(string section, string subsection, string key, object value)
    {
      SetValue(false, section, subsection, key);
    }

    public void SetValue(bool global, string section, string key, object value)
    {
      SetValue(global, section, String.Empty, key, value);
    }

    public void SetValue(bool global, string section, string subsection, string key, object value)
    {
      if (!String.IsNullOrEmpty(subsection))
      {
        string sKey = section + " \"" + subsection + "\"";
        if (!Data.Keys.Contains(sKey))
          Data.Add(sKey, new NameValueCollection());

        Data[sKey][key] = value.ToString();
      }
      else
      {
        if (!Data.Keys.Contains(section))
          Data.Add(section, new NameValueCollection());
        Data[section][key] = value.ToString();
      }
      
      // TODO : Determine if saving is ab-so-lute necessary after setting a value
      // Save();
    }

    public void Remove(string section, string key, bool global)
    {
      Remove(section, String.Empty, key, global);
    }
    public void Remove(string section, string subsection, string key, bool global)
    {
      if (!String.IsNullOrEmpty(subsection))
        Data[section + " \"" + subsection + "\""].Remove(key);
      else
        Data[section].Remove(key);

      // TODO : Determine if saving is ab-so-lute necessary after setting a value
      // Save();
    }

    #endregion

    #region File Handling

    #region Regex

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


    private void ReadConfigurationFile(string path)
    {

      string config = File.ReadAllText(path);

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
                m.Groups["SectionName"].Value;

            //    ToLowerInvariant();

            if (lastSection != sName)
            {
              lastSection = sName;
              if (!Data.ContainsKey(sName))
              {
                Data.Add
                (
                    sName,
                    new NameValueCollection()
                );
              }
            }
          }
          else if (m.Groups["IsKeyValue"].Length > 0)
          {
            Data[lastSection].Add
            (
                m.Groups["Key"].Value,
                m.Groups["Value"].Value
            );
          }
        }
      }
    }

    private void ReadConfiguration()
    {
      ReadConfigurationFile(Path);
    }
    #endregion



    #region Public Methods

    public void Save()
    {
      string tmpName = System.IO.Path.GetTempFileName();

      #region Write Configuration to temporary file

      using (FileStream fs = new FileStream(tmpName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
      {
        using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
        {
          Dictionary<string, NameValueCollection>.Enumerator en = Data.GetEnumerator();

          while (en.MoveNext())
          {
            KeyValuePair<string, NameValueCollection> cur = en.Current;

            if (!string.IsNullOrEmpty(cur.Key))
            {
              sw.WriteLine("[{0}]", cur.Key);
            }

            foreach (string key in cur.Value.Keys)
            {
              if (!string.IsNullOrEmpty(key))
              {
                if (!string.IsNullOrEmpty(cur.Value[key]))
                  sw.WriteLine("\t{0} = {1}", key, cur.Value[key]);
              }
            }

          }

          sw.Flush();
        }
      }

      #endregion

      #region Swap temporary file with config file

      File.Delete(Path);
      File.Move(tmpName, Path);

      #endregion

      
    }

    public void Reload()
    {
      Data.Clear();
      ReadConfiguration();
    }

    #endregion












  }
}
