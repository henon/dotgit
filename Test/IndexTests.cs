using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using dotGit;
using dotGit.Index;
using dotGit.Generic;
using System.IO;

namespace Test
{
  [TestFixture]
  public class IndexTests
  {
    private Repository repo;

    [SetUp]
    public void Setup()
    {
      repo = Repository.Open(Global.TestRepositoryPath);
    }

    [TearDown]
    public void TearDown()
    {
      repo = null;
    }

    [Test]
    [Category("Index Handling")]
    public void IndexShouldNotBeNullAfterOpeningRepository()
    {
      Assert.IsNotNull(repo.Index, "Index is null after opening repository");
    }

    [Test]
    [Category("Index Handling")]
    public void IndexShouldContainsOneOrMoreEntries()
    {
      Assert.GreaterOrEqual(repo.Index.NumberOfEntries, 1, "Index should contain at least one entry");
    }

    [Test]
    [Category("Index Handling")]
    public void IndexShouldBeEnumerableForTypeIndexEntry()
    {
      int idx = 0;
      foreach (IndexEntry entry in repo.Index)
      {
        Assert.IsNotNull(entry,"Enumerated IndexEntry cannot be null");
        idx++;
      }

      Assert.AreEqual(idx, repo.Index.NumberOfEntries, "NumberOfEntries is not equal to number of enumerated entries.");

    }

    [Test]
    [Category("Index Handling")]
    public void IndexNumberOfEntriesShouldBeEqualToEntriesCount()
    {
      Assert.AreEqual(repo.Index.NumberOfEntries, repo.Index.Entries.Count, "Loaded number of entries is not equal to NumberOfEntries");
    }

    [Test]
    [Category("Index Handling")]
    public void IndexEntryShouldContainLastCreationTime()
    {
      // get first entry;
      IndexEntry entry = repo.Index.Entries[0];

      Assert.IsNotNull(entry.Created, "CreationTime should not be null");
      Assert.IsTrue(entry.Created < DateTime.Now, "CreationTime should be in the past");
      
    }

    [Test]
    [Category("Index Handling")]
    public void IndexEntryShouldContainLastModificationTime()
    {
      // get first entry;
      IndexEntry entry = repo.Index.Entries[0];

      Assert.IsNotNull(entry.Modified, "Modified should not be null");
      Assert.IsTrue(entry.Modified < DateTime.Now, "Modified should be in the past");
    }

    [Test]
    [Category("Index Handling")]
    public void IndexEntryShouldContainAPath()
    {
      // get first entry;
      IndexEntry entry = repo.Index.Entries[0];
      Assert.IsFalse(String.IsNullOrEmpty(entry.Path), "IndexEntry.Path should not be null or empty");
    }

    [Test]
    [Category("Index Handling")]
    public void IndexEntryShouldContainAValidSha()
    {
      // get first entry;
      IndexEntry entry = repo.Index.Entries[0];
      Assert.IsTrue(Sha.IsValidSha(entry.SHA), "IndexEntry.Sha should be a valid SHA-1");
    }


    [Test]
    [Category("Index Handling")]
    public void IndexShouldReturnNullOnInvalidShaIndexer()
    {
      Assert.IsNull(repo.Index[String.Empty], "IndexEntry cannot be non-null for string.empty");
    }


    [Test]
    [Category("Index Handling")]
    public void StageFileShouldCreateNewIndexEntry()
    {
      string fileName = "TestFile-" + Guid.NewGuid().ToString();
      string path = Path.Combine(Global.TestRepositoryPath, fileName);
      File.WriteAllText(path, "THIS IS SOME TEXT");
      
      Sha newSha = repo.Stage(path);
      IndexEntry entry = repo.Index[newSha.SHAString];

      Assert.IsNotNull(entry, "Newly added IndexEntry cannot be null");
      Assert.IsTrue(repo.Index.StageIsNormal,"StageIsNormal is false, adding a new entry failed");
      Assert.AreEqual(entry.Stage,IndexStage.Normal, "Newly added entry is not Stage==Normal");
    }

    [Test]
    [Category("Index Handling")]
    public void StageDirectoryShouldCreateNewIndexEntries()
    {
      string subPath = Path.Combine(Global.TestRepositoryPath,"NewSubDir");
      int numberOfFiles = 10;

      for (int idx = 0; idx < numberOfFiles; idx++)
      {
        string fileName = "TestFile-Dir-" + idx.ToString();
        string path = Path.Combine(subPath, fileName);
        File.WriteAllText(path, String.Format("THIS IS SOME TEXT, file {0}",idx));
      }

      List<Sha> newShas = repo.StageDirectory(subPath);

      bool result = true;
      foreach (Sha s in newShas)
      {
        if (!repo.Index.Contains(s.SHAString) || repo.Index[s.SHAString] == null)
          result = false;
      }

      Assert.IsNotNull(newShas,"StageDirectory should return a List<Sha>");
      Assert.IsTrue(result, "The Index should contain every entry added");
      Assert.AreEqual(numberOfFiles, newShas.Count, String.Format("StageDirectory result does not match number of files {0}",numberOfFiles));
      Assert.IsTrue(repo.Index.StageIsNormal, "StageIsNormal is false, adding new entries failed");
    }


  }
}
