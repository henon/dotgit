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
	public class ConfigTests
  {
    [Test]
    public void ConfigUserNameShouldBeTest()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsTrue(String.Equals(repo.Config.User.Name, "Test"), "Username configuration in repository should be 'Test'");
    }

    [Test]
    public void ConfigUserEmailShouldBeTestEmail()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsTrue(String.Equals(repo.Config.User.Email, "test@test.com"), "User email should be test@test.com");
    }

    [Test]
    public void OriginShouldBeTestRepositoryAtGitHub()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsTrue(String.Equals(repo.Remotes.Origin.Url, "git@github.com:pheew/dotgittestrepo.git"), "Repository is not 'origin'ated at github test repository");
    }


	}
}
