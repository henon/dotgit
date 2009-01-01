using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dotGit.Exceptions;
using IO = System.IO;
using System.Text.RegularExpressions;
using dotGit.Objects;
using dotGit.Generic;


namespace dotGit.Refs
{
	/// <summary>
	/// Represents a reference to an object
	/// </summary>
	public abstract class Ref
  {
    #region Constructors

    private Ref(Repository repo)
		{
			Repo = repo;
		}

		protected Ref(Repository repo, string sha)
			:this(repo)
		{
			SHA = sha;
		}

		protected Ref(Repository repo, string path, string sha)
			: this(repo, sha)
		{
			Path = path;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Reference to the repository this Ref belongs to.
    /// </summary>
    protected Repository Repo
		{
			get;
			private set;
		}

		public string Path
		{
			get;
			protected set;
		}

		/// <summary>
		/// The SHA this tag is referenced by
		/// </summary>
		public string SHA
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of this REF
		/// </summary>
		public string Name
		{
			get { return IO.Path.GetFileName(Path); }
    }

    #endregion

    internal abstract void Deserialize();
		
	}
}
