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
    public void ConfigUserEmailShouldBeSettable()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      string first = repo.Config.User.Email;
      string newEmail = "newemail@somehost.com";
      repo.Config.User.Email = newEmail;

      Assert.IsTrue(String.Equals(repo.Config.User.Email, newEmail), "User email should be {0} after setting it".FormatWith(newEmail));
    }

    [Test]
    public void ConfigUserNameShouldBeSettable()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      string first = repo.Config.User.Name;
      string newName = "MyFirstName MyLastName";
      repo.Config.User.Name = newName;

      Assert.IsTrue(String.Equals(repo.Config.User.Name, newName), "User name should be {0} after setting it".FormatWith(newName));
    }

    [Test]
    public void OriginShouldBeNonEmptyForTestRepository()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsNotNull(repo.Remotes["origin"], "Test repository origin cannot be null or empty");
      Assert.IsFalse(String.IsNullOrEmpty(repo.Remotes["origin"].Url), "Test repository origin url cannot be null or empty");
      
    }


    [Test]
    public void OriginShouldBeTestRepositoryAtGitHub()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsTrue(String.Equals(repo.Remotes["origin"].Url, "git@github.com:pheew/dotgittestrepo.git"), "Repository is not 'origin'ated at github test repository");
    }

    [Test]
    public void CoreSectionShouldContainStandardEntries()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsNotNull( repo.Config.Core.Bare, "Config.Bare cannot be null");
      Assert.IsNotNull( repo.Config.Core.SymLinks, "Config.SymLinks cannot be null");
      Assert.IsNotNull( repo.Config.Core.FileMode, "Config.FileMode cannot be null");


      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.Bare, "Config.Bare is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.SymLinks, "Config.SymLinks is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.FileMode, "Config.FileMode is not of type boolean");

    }

	}
}
