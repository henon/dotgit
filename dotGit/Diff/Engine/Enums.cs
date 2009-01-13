using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Diff.Engine
{
  public enum DiffResultSpanStatus
  {
    NoChange,
    Replace,
    DeleteSource,
    AddDestination
  }

  internal enum DiffStatus
  {
    Matched = 1,
    NoMatch = -1,
    Unknown = -2

  }

  public enum DiffEngineLevel
  {
    FastImperfect,
    Medium,
    SlowPerfect
  }
}
