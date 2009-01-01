using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotGit.Generic;

namespace dotGit.Objects.Storage.PackObjects
{
  /// <summary>
  /// Base class representing a object in the Git Pack file. Not to be used directly.
  /// </summary>
  public abstract class PackObject
  {
    private PackObject()
    { }

    internal PackObject(long size, ObjectType type)
    {
      Size = size;
      Type = type;
    }

    /// <summary>
    /// Size of this object in bytes.
    /// </summary>
    public long Size
    {
      get;
      private set;
    }

    public ObjectType Type
    {
      get;
      private set;
    }
  }
}
