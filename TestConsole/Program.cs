using System;
using System.Collections.Generic;
using System.Linq;
using dotGit;
using dotGit.Objects;


namespace TestConsole
{
  class Program
  {
    #region History Builder (Example)

    static Dictionary<string,Commit> _seen = new Dictionary<string,Commit>(5000);
    static Queue<Commit> _pending = new Queue<Commit>();
    
    private static void BuildHistory()
    {

      while (_pending.Count > 0)
      {
        Commit entry = _pending.Dequeue();
        if (!_seen.Keys.Contains(entry.SHA))
        {
          _seen.Add(entry.SHA,  entry);
        
          if (entry.HasParents)
          {
             for (int idx = entry.Parents.Count - 1; idx >= 0; idx--)
              {
                _pending.Enqueue(entry.Parents[idx]);
              }
          }
        }
      }
    }

    #endregion

    private static string RepositoryPath = @"f:\code\dotGit\";

    /// <summary>
    /// This example program opens a Git repository (specify the path above in the RepositoryPath variable) and reads the complete
    /// history (all objects) from the pack file by using the BuildHistory() function.
    /// </summary>
    /// <param name="args"></param>
    /// 
    static void Main(string[] args)
    {
      Repository repo = Repository.Open(RepositoryPath);

      Console.WriteLine(repo.Storage);
      
      DateTime start = DateTime.Now;
      Console.WriteLine("Start building traversing history @ {0}", DateTime.Now);

      _pending.Enqueue(repo.HEAD.Branch.Commit);
      BuildHistory();

      Console.WriteLine("Traversed history in {0} seconds", (DateTime.Now - start).TotalSeconds);

#if DEBUG
      Console.ReadLine();
#endif

    }

    #region More Examples 

    private static void WalkTree(Tree tree, int level)
    {
      Console.WriteLine(new String('-', level) + tree.Path);
      foreach (TreeNode n in tree.Children)
      {
        if (n.IsBlob)
          Console.WriteLine(new String('-', level) + n.Path);
        else
          WalkTree((Tree)n, level++);
      }
    }

    #endregion

  }
}

