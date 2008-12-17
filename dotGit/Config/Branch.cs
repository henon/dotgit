using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace dotGit.Config
{
  public class Branch : ConfigSection
  {

    private static readonly Regex remoteRegex = new Regex(" \"(.*)\"");

    public string Name
    {
      get;
      private set;
    }
    public string Remote
    {
      get;
      private set;
    }
    public string Merge
    {
      get;
      private set;
    }



    
    internal Branch(string configurationKey, NameValueCollection data)
    {
      Match m = remoteRegex.Match(configurationKey);

      if (m.Groups.Count != 2)
        throw new ArgumentOutOfRangeException("Branch '{0}' is not a valid remote tag in Git configuration".FormatWith(configurationKey));

      Name = m.Groups[1].Value;
      
      foreach (string key in data.Keys)
      {
        switch (key.ToLower())
        {
          case "remote":
            Remote = data[key];
            break;

          case "merge":
            Merge= data[key];
            break;
        }
      }

    }
  }
}
