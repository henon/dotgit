using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace dotGit.Config
{
  public class User : ConfigSection
  {

    public string Name
    {
      get;
      set;
    }

    public string Email
    {
      get;
      set;
    }

    internal User()
    {
      Email = String.Empty;
      Name = String.Empty;
    }

    internal User(NameValueCollection data) : this()
    {

      foreach (string key in data.Keys)
      {
        switch (key.ToLower())
        {
          case "email":
            Email = data[key];
            break;
          case "name":
            Name = data[key];
            break;
          default:
            break;
        }
      }
    }
  }
}
