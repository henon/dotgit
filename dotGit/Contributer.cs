using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dotGit.Generic;
using dotGit.Objects;

namespace dotGit
{

  /// <summary>
  /// Class to represent an author or committer in the Git world. Mainly used by the pack file when reading a Commit.
  /// </summary>
	public class Contributer
  {

    #region Fields

    private static List<Contributer> _contributerCache;

    #endregion Fields

    #region Constructors

    private Contributer()
		{	}

		public Contributer(string name)
		{
			Name = name;
		}

		public Contributer(string name, string email)
			:this(name)
		{
			Email = email;
    }

    #endregion

    /// <summary>
		/// Load Contributer from git formatted string.
		/// </summary>
		/// <param name="input">string in format: 'John Doe &lt;john@doe.com&gt;'</param>
		/// <returns>parsed Contributer</returns>
		public static Contributer Parse(string input)
		{
      string name = input;
      string email=String.Empty;
      int index = input.LastIndexOf('<');
      if( index > -1 )
      {
        name = input.Substring(0,index-1).Trim();
        email = input.Substring(index).Trim(' ','>','<');
      }

      Contributer con = ContributerCache.SingleOrDefault(c => c.Email == email && c.Name == name);

      if (con != null)
        return con;
      else
      {
        con = new Contributer(name, email);
        ContributerCache.Add(con);
        return con;
      }
		}

		/// <summary>
		/// Returns the name and email in git format: 'John Doe &lt;john@doe.com&gt;'
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} <{1}>", Name, Email);
		}

		/// <summary>
		/// The name of this contributer
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// The email address of this contribute
		/// </summary>
		public string Email
		{
			get;
			private set;
		}

    private static List<Contributer> ContributerCache
    {
      get
      {
        if (_contributerCache == null)
          _contributerCache = new List<Contributer>();

        return _contributerCache;

      }
    }
    
   }
}
