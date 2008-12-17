using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Config
{
  public class Remotes : List<Remote>
{

    public Remote this[string name]
    {
      get
      {
        foreach (Remote r in this)
        {
          if (r.Name == name)
            return r;
        }

        throw new IndexOutOfRangeException("Remote {0} was not found in configuration".FormatWith(name));
      }
    }

  }

}
