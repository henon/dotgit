using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using dotGit;
using System.IO;
using dotGit.Objects;
using dotGit.Refs;


namespace Test
{
	[TestFixture]	
	public class RepositoryTests
	{

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RepositoryThrowsArgumentExceptionForNullPath()
		{
			Repository.Open(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RepositoryThrowsArgumentExceptionForEmptyPath()
		{
			Repository.Open(String.Empty);
		}

		[Test]
		public void RepositoryShouldNotThrowExceptionWhenWalkingTree()
		{
			Repository repo = Repository.Open(Global.TestRepositoryPath);
			try
			{
				WalkHistory(repo.HEAD.Commit);
				Assert.IsTrue(true, "Walking the history from HEAD does not throw exception");
			}
			catch (Exception ex)
			{
				Assert.IsTrue(false, "Walking the history should not throw exception, caught: '{0}'".FormatWith(ex));
			}
		}

		[Test]
		public void TestRepositoryShouldContainTagZeroPointOne()
		{
			Repository repo = Repository.Open(Global.TestRepositoryPath);
			try
			{
				Tag t = repo.Tags["0.1-alpha"];
				Assert.IsNotNull(t, "dotGit should not return null when fethcing tag '0.1-alpha'");
			}
			catch (Exception ex)
			{
				Assert.IsTrue(false, "Tag '0.1-alpha' should be in test repository, caught: {0}".FormatWith(ex));
			}
		}

    [Test]
    public void TestRepositoryShouldThrowIndexOutOfRangeExceptionForInvalidTag()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      try
      {
        Tag t = repo.Tags["TagThatDoesNotExist"];
        Assert.IsNull(t, "dotGit should return null when fethcing a tag that doesn't exist");
      }
      catch (IndexOutOfRangeException)
      {
        Assert.IsTrue(true);
      }
      catch(Exception ex)
      {
        Assert.IsTrue(false, "Tag getter should not throw exception other then IndexOutOfRangeException, caught: {0}".FormatWith(ex));
      }
    }


    [Test]
    public void SHAOfTagZeroPointOneShouldBeTestRepositorySHA()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      try
      {
        Tag t = repo.Tags["0.1-alpha"];
        Assert.IsTrue(String.Equals(t.SHA.ToLower(), "945ec04f178784065ffc4ed494f1f0a81af7b891"));
      }
      catch (IndexOutOfRangeException ex)
      {
        Assert.IsTrue(false, "Tag '0.1-alpha' should be in test repository, caught: {0}".FormatWith(ex));
      }
      catch (Exception ex)
      {
        Assert.IsTrue(false, "Tag getter should not throw exception other then IndexOutOfRangeException, caught: {0}".FormatWith(ex));
      }
    }


		private void WalkHistory(Commit commit)
		{
			if (commit.HasParents)
			{
				foreach(Commit parent in commit.Parents)
					WalkHistory(parent);
			}
		}

	}
}
