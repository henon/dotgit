using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Config
{
  public class Branches : List<Branch>
  {

    public Branch this[string name]
    {
      get
      {
        foreach (Branch b in this)
        {
          if (b.Name == name)
            return b;
        }

        throw new IndexOutOfRangeException("Branch {0} was not found in configuration".FormatWith(name));
      }
    }
  }
}
