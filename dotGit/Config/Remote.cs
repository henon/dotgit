using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace dotGit.Config
{
  public class Remote
  {

    private static readonly Regex remoteRegex = new Regex(" \"(.*)\"");

    public string Name
    {
      get;
      private set;
    }

    public string Url
    {
      get;
      private set;
    }

    public string Fetch
    {
      get;
      private set;
    }

    public Remote(string name, string url)
    {
      Name = name;
      Url = url;
    }
    internal Remote(string configurationKey, NameValueCollection data)
    {
      Match m = remoteRegex.Match(configurationKey);

      if (m.Groups.Count != 2)
        throw new ArgumentOutOfRangeException("Remote '{0}' is not a valid remote tag in Git configuration".FormatWith(configurationKey));

      Name = m.Groups[1].Value;
      
      foreach (string key in data.Keys)
      {
        switch (key.ToLower())
        {
          case "url":
            Url = data[key];
            break;

          case "fetch":
            Fetch = data[key];
            break;
        }
      }

    }
  }
}
