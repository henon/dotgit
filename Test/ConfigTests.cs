using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using dotGit;
using System.IO;
using dotGit.Objects;
using dotGit.Refs;
using dotGit.Config;


namespace Test
{
	[TestFixture]	
	public class ConfigTests
  {

    dotGit.Config.Configuration config;
    string path = Path.Combine(Global.AssemblyDir, @"Resources\config");
    string newConfigFilePath = Path.Combine(Global.AssemblyDir, @"Resources\config-2");

    [SetUp]
    public void ReadConfig()
    {
      if (File.Exists(newConfigFilePath))
        File.Delete(newConfigFilePath);

      config = Configuration.Load(path);
    }

    [TearDown]
    public void TearDown()
    {
      if (File.Exists(newConfigFilePath))
        File.Delete(newConfigFilePath);

    }


    #region Test Basic Scenarios
    [Test]
    public void InitializeConfigShouldReturnFreshConfiguration()
    {
      if (File.Exists(newConfigFilePath))
        File.Delete(newConfigFilePath);

      Configuration config2 = Configuration.Create(newConfigFilePath);

      Assert.IsNotNull(config, "configuration cannot be null after calling constructor");
      Assert.IsNotNull(config.Core, "Config.Core cannot be null after reading/creating configuration");
      Assert.IsNotNull(config.User, "Config.User cannot be null after reading/creating configuration");
    }

    [Test]
    [ExpectedException(ExceptionType=typeof(IOException), UserMessage="Cannot recreate config file which already exists")]
    public void InitializeConfigWhichAlreadyExistsShouldThrowException()
    {
      if (File.Exists(newConfigFilePath))
        File.Delete(newConfigFilePath);

      Configuration config2 = Configuration.Create(newConfigFilePath);
      Configuration config3 = Configuration.Create(newConfigFilePath);
       
      
    }

    [Test]
    public void InitializeConfigWithPathParameterShouldReturnNewFile()
    {
      Configuration config2 = Configuration.Create(newConfigFilePath);

      Assert.IsTrue(File.Exists(newConfigFilePath), "New configuration file was not created on filesystem");
      Assert.IsFalse(String.IsNullOrEmpty(File.ReadAllText(newConfigFilePath)), "New configuration file cannot be empty. Core section should exist");
      
    }

    [Test]
    public void ReadConfiguration()
    {
      Assert.IsNotNull(config, "Configuration cannot be null after reading it");
    }

    [Test]
    public void ReloadConfigurationShouldReturnOldValues()
    {
      Configuration config2 = Configuration.Load(path);

      config.Core.IgnoreCase = !config.Core.IgnoreCase;
      config.Reload();
      Assert.IsTrue(config.Core.IgnoreCase == config2.Core.IgnoreCase, "After reloading values should reflect old (file) values");

    }

    #endregion

    #region Getting / Setting

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
      
      bool initial = config.Core.SymLinks;
      config.Core.SymLinks = !initial;

      Assert.IsTrue(config.Core.SymLinks != initial, "SymLinks should be {0} after setting it".FormatWith(!initial));
    }

    [Test]
    public void OriginShouldBeTestRepositoryAtGitHub()
    {
      Assert.IsTrue(String.Equals(config.Remotes["origin"].Url, "git@github.com:pheew/dotgittestrepo.git"), "Repository is not 'origin'ated at github test repository");
    }

    #endregion

    #region Test Repository

    [Test]
    public void ConfigShouldBeNonEmptyForTestRepository()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);

      Assert.IsNotNull(repo.Config, "Test repository configuration cannot be null or empty");
      
    }

    [Test]
    public void ConfigShouldReturnOriginForTestRepository()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);

      Assert.IsNotNull(repo.Config.Remotes["origin"], "Test repository origin cannot be null or empty");
      Assert.IsFalse(String.IsNullOrEmpty(repo.Config.Remotes["origin"].Url), "Test repository origin url cannot be null or empty");

    }

    [Test]
    [ExpectedException(typeof(KeyNotFoundException), UserMessage="The Remotes collection should throw IndexOutOfRangeException for unknown remotes")]
    public void RemotesShouldThrowExceptionOnUnknownKey()
    {
      Repository repo = Repository.Open(Global.TestRepositoryPath);
      Console.WriteLine(repo.Config.Remotes[Guid.NewGuid().ToString()]);
    }

    [Test]
    public void CoreSectionShouldContainStandardEntries()
    {
      
      Assert.IsNotNull( config.Core.Bare, "Config.Bare cannot be null");
      Assert.IsNotNull( config.Core.SymLinks, "Config.SymLinks cannot be null");
      Assert.IsNotNull( config.Core.FileMode, "Config.FileMode cannot be null");
      Assert.IsNotNull(config.Core.IgnoreCase, "Config.IgnoreCase cannot be null");
      Assert.IsNotNull(config.Core.RepositoryFormatVersion, "Config.RepositoryFormatVersion cannot be null");

      Assert.IsInstanceOfType(typeof(bool), config.Core.Bare, "Config.Bare is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), config.Core.SymLinks, "Config.SymLinks is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), config.Core.FileMode, "Config.FileMode is not of type boolean");
      Assert.IsInstanceOfType(typeof(bool), config.Core.IgnoreCase, "Config.IgnoreCase is not of type boolean");
      Assert.IsInstanceOfType(typeof(string), config.Core.RepositoryFormatVersion, "Config.RepositoryFormatVersion is not of type string");
      Assert.IsInstanceOfType(typeof(bool), config.Core.LogAllRefUpdates, "Config.LogAllRefUpdates is not of type boolean");

    }

    [Test]
    public void TestRepositoryShouldReturnCorrectValues()
    {
 
      Repository repo = Repository.Open(Global.TestRepositoryPath);

      Assert.IsTrue(repo.Config.Core.IgnoreCase, "IgnoreCase should be true for test repository");
      Assert.IsFalse(repo.Config.Core.FileMode, "FileMode should be false for test repository");

      Assert.IsTrue(repo.Config.Remotes.Count > 0, "Test respository should contain at least one remote");
    }

    #endregion

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
      repo.Config.Remotes.Add("MyFirstRemote", newRemote);
      repo.Config.Save();
      repo.Config.Reload();

      dotGit.Config.Remote afterSave = repo.Config.Remotes["MyFirstRemote"];

      Assert.IsNotNull(afterSave, "Remote should persist after saving configuration");
      Assert.IsTrue(afterSave.Name == newRemote.Name, "New remote name is not persisted");
      Assert.IsTrue(afterSave.Url== newRemote.Url, "New remote url is not persisted");

    }

    [Test]
    public void SavingConfigurationShouldNotMessUpUnknownConfigValues()
    {
      Assert.IsTrue(false, "To be determined");
    }
	}
}
