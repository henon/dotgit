using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using dotGit;
using dotGit.Index;
using dotGit.Generic;

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
      Assert.Greater(entry.Created.Seconds, 0, "CreationTime should be greate than zero");
    }

    [Test]
    [Category("Index Handling")]
    public void IndexEntryShouldContainLastModificationTime()
    {
      // get first entry;
      IndexEntry entry = repo.Index.Entries[0];

      Assert.IsNotNull(entry.Modified, "Modified should not be null");
      Assert.Greater(entry.Modified.Seconds, 0, "Modified should be greate than zero");
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
  }
}
