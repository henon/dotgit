using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IO = System.IO;
using dotGit.Exceptions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;


namespace dotGit.Objects.Storage
{
  /// <summary>
  /// This a base class for all Pack file processing classes and should not be used directly
  /// </summary>
  public abstract class Pack
  {
    private Pack() { }

    internal Pack(Repository repo, string path)
    {
      Repo = repo;
      Path = path.TrimEnd(IO.Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Loads a pack file, ready for processing
    /// </summary>
    /// <param name="repo">The repository this pack file belongs</param>
    /// <param name="path">Relative path to pack</param>
    /// <returns>An instance of one of <see cref="Pack" />'s subclasses</returns>
    public static Pack LoadPack(Repository repo, string path)
    {
      // TODO: Add version parsing to instantiate the right Pack version. Much like GetObject in ObjectStorage
      return new PackV2(repo, path);
    }

    /// <summary>
    /// Number of objects in this pack
    /// </summary>
    public int NumberOfObjects
    {
      get;
      protected set;
    }

    /// <summary>
    /// The version of this pack
    /// </summary>
    public abstract int Version
    {
      get;
    }

    /// <summary>
    /// The path to this pack
    /// </summary>
    public string Path
    {
      get;
      private set;
    }

    /// <summary>
    /// The repository this pack file belongs
    /// </summary>
    protected Repository Repo
    {
      get;
      private set;
    }

    /// <summary>
    /// Searches for an object by it's sha in this pack and returns it as an IStorableObject
    /// </summary>
    /// <param name="sha">The sha of the object to find</param>
    /// <returns>An IStorableObject</returns>
    public abstract IStorableObject GetObject(string sha);

  }
}
