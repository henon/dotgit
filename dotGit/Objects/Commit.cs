using System;
using System.Collections.Generic;
using System.Text;
using dotGit.Objects.Storage;
using System.Security.Cryptography;
using dotGit.Objects;
using dotGit.Generic;
using dotGit.Exceptions;
using System.Linq;

namespace dotGit.Objects
{

  /// <summary>
  /// A Commit references a tree which is a copy of the working directory
  /// </summary>
  public class Commit : GitObject
  {
    private string _treeSha = null;
    private Tree _tree = null;

    private List<string> _parentShas = null;
    private CommitCollection _parents = null;

    internal Commit(Repository repo)
      : base(repo)
    { }

    internal Commit(Repository repo, string sha)
      : base(repo, sha)
    { }

    /// <summary>
    /// The message for this commit
    /// </summary>
    public string Message
    {
      get;
      private set;
    }

    /// <summary>
    /// The tree object this commit is referencing
    /// </summary>
    public Tree Tree
    {
      get
      {
        if (_tree != null) return _tree;

        if (!String.IsNullOrEmpty(_treeSha))
          _tree = Repo.Storage.GetObject<Tree>(_treeSha);

        return _tree;
      }
      private set
      {
        _tree = value;
      }
    }


    /// <summary>
    /// The ancestors of this commit. 
    /// </summary>
    public CommitCollection Parents
    {
      get
      {
        if (_parents == null && _parentShas.Count > 0)
          LoadParents();

        return _parents;
      }
    }


    /// <summary>
    /// Called from the Parents getter for lazy loading
    /// </summary>
    private void LoadParents()
    {
      _parents = new CommitCollection(_parentShas.Count);

      try
      {
        foreach (string parentSha in _parentShas)
          _parents.Add(Repo.Storage.GetObject<Commit>(parentSha));
      }
      catch (Exception)
      {
        // Reset _parents field, otherwise the object would be in an invalid state
        _parents = null;

        throw;
      }
    }

    /// <summary>
    /// Returns true if this commit has at least 1 parent commit
    /// </summary>
    public bool HasParents
    {
      get { return Parents != null && Parents.Count > 0; }
    }

    /// <summary>
    /// The contributer that made this commit
    /// </summary>
    public Contributer Committer
    {
      get;
      private set;
    }

    /// <summary>
    /// The date this commit has been made
    /// </summary>
    public DateTime CommittedDate
    {
      get;
      private set;
    }

    /// <summary>
    /// The original author of this commit
    /// </summary>
    public Contributer Author
    {
      get;
      private set;
    }

    /// <summary>
    /// The original date of this commit
    /// </summary>
    public DateTime AuthoredDate
    {
      get;
      private set;
    }

    /// <summary>
    /// Loads the commit from the GitObjectReader
    /// </summary>
    /// <param name="input">A reader with inflated commit contents</param>
    public override void Deserialize(GitObjectReader input)
    {

      // Get the contents from the stream to avoid ReadByte() every time
      string contents = input.ReadToEnd().GetString();

      int length = contents.Length;
      int index = 0;

      _treeSha = contents.Substring(index + 5, 40);
      _parentShas = new List<string>();

      index += 46; // "tree " + sha + \n
      while (index + 48 < contents.Length && contents.Substring(index, 7).Equals("parent "))
      {
        // got parent
        _parentShas.Add(contents.Substring(index + 7, 40));
        index += 48;
      }

      // Check if we can get an author from the next characters
      if (index + 7 < length && contents.Substring(index, 7) == "author ")
      {
        index += 7;
        int authorLength = 0;
        while (contents[index] != '\n')
        {
          authorLength++;
          index++;
        }

        String authorLine = contents.Substring(index - authorLength, authorLength);
        AuthoredDate = Utility.StripDate(authorLine, out authorLine);
        Author = Contributer.Parse(authorLine);

        // Skip the \n
        index++;
      }

      // Read committer from the stream
      if (index + 9 < length && contents.Substring(index, 10) == "committer ")
      {
        index += 10;
        int committerLength = 0;
        while (contents[index] != '\n')
        {
          committerLength++;
          index++;
        }

        String committerLine = contents.Substring(index - committerLength, committerLength);
        CommittedDate = Utility.StripDate(committerLine, out committerLine);
        Committer = Contributer.Parse(committerLine);

        // Skip the \n
        index++;
      }

      index++; // extra \n
      if (index < contents.Length)
        Message = contents.Substring(index);
      else
        Message = String.Empty;

    }

    /// <summary>
    /// Serializes this commit to a byte array
    /// </summary>
    /// <returns>byte[] containing this commit's contents</returns>
    public override byte[] Serialize()
    {
      throw new NotImplementedException();
    }


    #region System Overrides

    /// <summary>
    /// Returns a string representing this Commit.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return SHA;
    }

    /// <summary>
    /// Compares commit to other commit on SHA
    /// </summary>
    /// <param name="obj">The other commit to compare with</param>
    /// <returns>Boolean indicating this object is equal to <paramref name="obj"/></returns>
    public bool Equals(Commit c)
    {
      return String.Equals(c.SHA, SHA);
    }

    /// <summary>
    /// Compares commit to other commit on SHA
    /// </summary>
    /// <param name="obj">The other commit to compare with</param>
    /// <returns>Boolean indicating this object is equal to <paramref name="obj"/></returns>
    public override bool Equals(object obj)
    {
      if (obj is Commit)
        return String.Equals(SHA, ((Commit)obj).SHA);
      else
        return false;

    }
    /// <summary>
    /// Returns the hashcode for this commit
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return SHA.GetHashCode();
    }

    #endregion

  }
}
