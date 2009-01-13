using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Diff.Engine
{
  public interface IDiffList
  {
    int Count{ get; }
    IComparable GetByIndex(int index);
  }
}
