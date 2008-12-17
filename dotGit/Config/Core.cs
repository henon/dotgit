using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace dotGit.Config
{
  public class Core : ConfigSection
  {
    #region Fields
    public bool Bare { get; set; }
    public bool SymLinks { get; set; }
    public bool FileMode { get; set; }
    public bool IgnoreCase { get; set; }
    public bool? AutoCrLf { get; set; }
    public bool? SafeCrLf { get; set; }
    public string RepositoryFormatVersion { get; set; }
    public bool LogAllRefUpdates { get; set; }
    #endregion

    internal Core()
    {
      Bare = false;
      SymLinks = false;
      FileMode = true;
      IgnoreCase = false;
      AutoCrLf = null;
      SafeCrLf = null;
      RepositoryFormatVersion = "0";
      LogAllRefUpdates = true;
    }

    internal Core(NameValueCollection data)
    {
      foreach (string key in data.Keys)
      {
        switch (key.ToLower())
        {
          case "bare":
            Bare = Boolean.Parse(data[key]);
            break;
          case "symlinks":
            SymLinks = Boolean.Parse(data[key]);
            break;
          case "ignorecase":
            IgnoreCase = Boolean.Parse(data[key]);
            break;
          case "filemode":
            FileMode = Boolean.Parse(data[key]);
            break;
          case "repositoryformatversion":
            RepositoryFormatVersion = data[key];
            break;
          case "logallrefupdates":
            LogAllRefUpdates = Boolean.Parse(data[key]);
            break;

        }
      }
    }
  }
}
