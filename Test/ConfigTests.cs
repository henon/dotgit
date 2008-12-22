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
      Assert.IsNotNull(config.GetValue<int>(ConfigSections.Core, "repositoryformatversion"), "Config core.repositoryformatversion cannot be null after reading/creating configuration");
    }

    [Test]
    public void InitializeConfigWithPathParameterShouldReturnNewFile()
    {
      if (File.Exists(newConfigFilePath))
        File.Delete(newConfigFilePath);

      Configuration config2 = Configuration.Create(newConfigFilePath);
      config2.Save();
      Assert.IsTrue(File.Exists(newConfigFilePath), "New configuration file was not created on filesystem");
      Assert.IsFalse(String.IsNullOrEmpty(File.ReadAllText(newConfigFilePath)), "New configuration file cannot be empty. Core section should exist");

    }

    [Test]
    [ExpectedException(typeof(ArgumentException), UserMessage = "Cannot recreate config file which already exists")]
    public void InitializeConfigWhichAlreadyExistsShouldThrowException()
    {
      Configuration config2 = Configuration.Create(path);
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

      config.SetValue(ConfigSections.Core, ConfigKeys.IgnoreCase, !config.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase));

      config.Reload();
      Assert.IsTrue(
        config.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase)
        ==
        config2.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase)
        , "After reloading values should reflect old (file) values");

    }

    #endregion

    #region Getting / Setting

    [Test]
    public void ConfigUserNameShouldBeTest()
    {
      Assert.IsTrue(String.Equals(config.GetValue<string>(ConfigSections.User, ConfigKeys.Name), "Test"), "Username configuration in repository should be 'Test'");
    }

    [Test]
    public void ConfigUserEmailShouldBeTestEmail()
    {
      Assert.IsTrue(String.Equals(config.GetValue<string>(ConfigSections.User, ConfigKeys.Email), "dotGitTest@test.com"), "User email should be test@test.com");
    }

    [Test]
    public void ConfigUserEmailShouldBeSettable()
    {

      string initial = config.GetValue<string>(ConfigSections.User, ConfigKeys.Email);
      string newEmail = "newemail@somehost.com";
      config.SetValue(ConfigSections.User, ConfigKeys.Email, newEmail);

      Assert.IsTrue(String.Equals(config.GetValue<string>(ConfigSections.User, ConfigKeys.Email), newEmail), "User email should be {0} after setting it".FormatWith(newEmail));

    }

    [Test]
    public void ConfigUserNameShouldBeSettable()
    {
      string initial = config.GetValue<string>(ConfigSections.User, ConfigKeys.Name);
      string newName = "MyFirst MyLast";
      config.SetValue(ConfigSections.User, ConfigKeys.Name, newName);

      Assert.IsTrue(String.Equals(config.GetValue<string>(ConfigSections.User, ConfigKeys.Name), newName), "User name should be {0} after setting it".FormatWith(newName));
    }

    [Test]
    public void ConfigIgnoreCaseShouldBeSettable()
    {
      Configuration config2 = Configuration.Load(path);

      bool initial = config2.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase);
      config2.SetValue(ConfigSections.Core, ConfigKeys.IgnoreCase, !initial);

      Assert.IsTrue(config2.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase) != initial, "IgnoreCase should be {0} after setting it".FormatWith(!initial));
    }

    [Test]
    public void ConfigSymLinksShouldBeSettable()
    {
      Configuration config2 = Configuration.Load(path);
      bool initial = config2.GetValue<bool>(ConfigSections.Core, ConfigKeys.SymLinks);
      config2.SetValue(ConfigSections.Core, ConfigKeys.SymLinks, !initial);

      Assert.IsTrue(config2.GetValue<bool>(ConfigSections.Core, ConfigKeys.SymLinks) != initial, "SymLinks should be {0} after setting it".FormatWith(!initial));
    }

    [Test]
    public void OriginShouldBeTestRepositoryAtGitHub()
    {
      // We love GitHub ;)

      Assert.IsTrue(String.Equals(
        config.GetValue<string>(ConfigSections.Remote, "origin", ConfigKeys.Url)
        , "git@github.com:pheew/dotgit.git"), "Repository is not 'origin'ated at github");
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
    public void ConfigShouldReturnOriginRemote()
    {
      Assert.IsFalse(String.IsNullOrEmpty(config.GetValue<string>(ConfigSections.Remote, "origin", ConfigKeys.Url)), "Test repository origin url cannot be null or empty");
    }

    [Test]
    public void CoreSectionShouldContainStandardEntries()
    {
      Assert.IsNotNull(config.GetValue<bool>(ConfigSections.Core, ConfigKeys.Bare), "Config.Bare cannot be null");
      Assert.IsNotNull(config.GetValue<bool>(ConfigSections.Core, ConfigKeys.SymLinks), "Config.SymLinks cannot be null");
      Assert.IsNotNull(config.GetValue<bool>(ConfigSections.Core, ConfigKeys.FileMode), "Config.FileMode cannot be null");
      Assert.IsNotNull(config.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase), "Config.IgnoreCase cannot be null");
      Assert.IsNotNull(config.GetValue<int>(ConfigSections.Core, ConfigKeys.RepositoryFormatVersion), "Config.RepositoryFormatVersion cannot be null");

      //Assert.IsInstanceOfType(typeof(bool), config.Core.Bare, "Config.Bare is not of type boolean");
      //Assert.IsInstanceOfType(typeof(bool), config.Core.SymLinks, "Config.SymLinks is not of type boolean");
      //Assert.IsInstanceOfType(typeof(bool), config.Core.FileMode, "Config.FileMode is not of type boolean");
      //Assert.IsInstanceOfType(typeof(bool), config.Core.IgnoreCase, "Config.IgnoreCase is not of type boolean");
      //Assert.IsInstanceOfType(typeof(string), config.Core.RepositoryFormatVersion, "Config.RepositoryFormatVersion is not of type string");
      //Assert.IsInstanceOfType(typeof(bool), config.Core.LogAllRefUpdates, "Config.LogAllRefUpdates is not of type boolean");

    }

    [Test]
    public void TestRepositoryShouldReturnCorrectValues()
    {

      Repository repo = Repository.Open(Global.TestRepositoryPath);

      Assert.IsTrue(repo.Config.GetValue<bool>(ConfigSections.Core, ConfigKeys.IgnoreCase), "IgnoreCase should be true for test repository");
      Assert.IsFalse(repo.Config.GetValue<bool>(ConfigSections.Core, ConfigKeys.FileMode), "FileMode should be false for test repository");

      Assert.IsTrue(repo.Config.GetValue<string>(ConfigSections.Remote, "origin", ConfigKeys.Url).Contains("github"), "Test repository origin should be from github");
    }

    #endregion


    [Test]
    public void SavingUserEmailChangeShouldPersistSettings()
    {

      Repository repo = Repository.Open(Global.TestRepositoryPath);
      string first = repo.Config.GetValue<string>(ConfigSections.User, ConfigKeys.Email);
      string newEmail = "newemail@somehost.com";
      repo.Config.SetValue(ConfigSections.User, ConfigKeys.Email, newEmail);
      repo.Config.Save();
      repo.Config.Reload();

      Assert.IsTrue(String.Equals(repo.Config.GetValue<string>(ConfigSections.User, ConfigKeys.Email), newEmail), "User email should be {0} after setting and saving it".FormatWith(newEmail));

    }

    [Test]
    public void AddingRemoteShouldPersist()
    {

      Repository repo = Repository.Open(Global.TestRepositoryPath);

      string remoteName = "MyFirstRemote";
      string url = "git@somehost.com:repo.git";
      repo.Config.SetValue(ConfigSections.Remote, remoteName, ConfigKeys.Url, url);
      repo.Config.Save();

      repo.Config.Reload();

      string newUrl = repo.Config.GetValue<string>(ConfigSections.Remote, remoteName, ConfigKeys.Url);

      Assert.IsNotNull(url, "Remote should persist after saving configuration");
      Assert.IsTrue(newUrl == url, "New remote url is not persisted");

    }

  }
}
