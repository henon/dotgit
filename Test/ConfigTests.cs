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
      Assert.IsTrue(String.Equals(repo.Config.User.Email, "dotGitTest@test.com"), "User email should be test@test.com");
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
    public void ConfigIgnoreCaseShouldBeSettable()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      bool initial = repo.Config.Core.IgnoreCase;
      repo.Config.Core.IgnoreCase = !initial;

      Assert.IsTrue(repo.Config.Core.IgnoreCase != initial, "IgnoreCase should be {0} after setting it".FormatWith(!initial));
    }

    [Test]
    public void ConfigSymLinksShouldBeSettable()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      bool initial = repo.Config.Core.SymLinks;
      repo.Config.Core.SymLinks = !initial;

      Assert.IsTrue(repo.Config.Core.SymLinks != initial, "SymLinks should be {0} after setting it".FormatWith(!initial));
    }


    [Test]
    public void OriginShouldBeNonEmptyForTestRepository()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsNotNull(repo.Config.Remotes["origin"], "Test repository origin cannot be null or empty");
      Assert.IsFalse(String.IsNullOrEmpty(repo.Config.Remotes["origin"].Url), "Test repository origin url cannot be null or empty");
      
    }


    [Test]
    public void OriginShouldBeTestRepositoryAtGitHub()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsTrue(String.Equals(repo.Config.Remotes["origin"].Url, "git@github.com:pheew/dotgittestrepo.git"), "Repository is not 'origin'ated at github test repository");
    }

    [Test]
    public void CoreSectionShouldContainStandardEntries()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Assert.IsNotNull( repo.Config.Core.Bare, "Config.Bare cannot be null");
      Assert.IsNotNull( repo.Config.Core.SymLinks, "Config.SymLinks cannot be null");
      Assert.IsNotNull( repo.Config.Core.FileMode, "Config.FileMode cannot be null");
      Assert.IsNotNull(repo.Config.Core.IgnoreCase, "Config.IgnoreCase cannot be null");
      Assert.IsNotNull(repo.Config.Core.RepositoryFormatVersion, "Config.RepositoryFormatVersion cannot be null");

      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.Bare, "Config.Bare is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.SymLinks, "Config.SymLinks is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.FileMode, "Config.FileMode is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.IgnoreCase, "Config.IgnoreCase is not of type boolean");
      Assert.IsInstanceOfType(typeof(string), repo.Config.Core.RepositoryFormatVersion, "Config.RepositoryFormatVersion is not of type string");
      Assert.IsInstanceOfType(typeof(bool), repo.Config.Core.LogAllRefUpdates, "Config.LogAllRefUpdates is not of type boolean");

    }

    [Test]
    public void TestRepositoryShouldReturnCorrectValues()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);

      Assert.IsTrue(repo.Config.Core.IgnoreCase, "IgnoreCase should be true for test repository");
      Assert.IsFalse(repo.Config.Core.FileMode, "FileMode should be false for test repository");

      Assert.IsTrue(repo.Config.Remotes.Count > 0, "Test respository should contain at least one remote");
    }

    [Test]
    public void SavingUserEmailChangeShouldPersistSettings()
    {
     
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      string first = repo.Config.User.Email;
      string newEmail = "newemail@somehost.com";
      repo.Config.User.Email = newEmail;
      repo.Config.Save();
      repo.Config.Reload();

      Assert.IsTrue(String.Equals(repo.Config.User.Email, newEmail), "User email should be {0} after setting and saving it".FormatWith(newEmail));

    }

    [Test]
    public void AddingRemoteShouldPersist()
    {

      Repository repo = Repository.Open(Global.TestRepositoryPath);

      dotGit.Config.Remote newRemote = new dotGit.Config.Remote("MyFirstRemote", "git@somehost.com:repo.git");
      repo.Config.Remotes.Add(newRemote);
      repo.Config.Save();
      repo.Config.Reload();

      dotGit.Config.Remote afterSave = repo.Config.Remotes["MyFirstRemote"];

      Assert.IsNotNull(afterSave, "Remote should persist after saving configuration");
      Assert.IsTrue(afterSave.Name == newRemote.Name, "New remote name is not persisted");
      Assert.IsTrue(afterSave.Url== newRemote.Url, "New remote url is not persisted");

    }

    public void SavingConfigurationShouldNotMessUpUnknownConfigValues()
    {
      Assert.IsTrue(false, "To be determined");
    }
	}
}
