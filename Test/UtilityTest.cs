using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using dotGit;
using System.IO;
using System.Reflection;
using dotGit.Generic;
using dotGit.Objects;

namespace Test
{
  [TestFixture]
  public class UtilityTest
  {
    private string _fullTempTestRepoName = null;
    private string _tempTestRepoName = @"Resources\UtilityTestTempRepoPath";

    [SetUp]
    public void Setup()
    {
      _fullTempTestRepoName = Path.Combine(Global.AssemblyDir, _tempTestRepoName);

      if (Directory.Exists(_fullTempTestRepoName))
        Directory.Delete(_fullTempTestRepoName, true);

      Directory.CreateDirectory(_fullTempTestRepoName);
    }

    [TearDown]
    public void TearDown()
    {
      if (Directory.Exists(_fullTempTestRepoName))
        Directory.Delete(_fullTempTestRepoName, true);
    }

    [Test]
    public void IsGitRepositoryShouldReturnFalseOnInvalidPath()
    {
      DirectoryInfo gitDir;
      Assert.IsFalse(Utility.IsGitRepository(null, out gitDir), "IsGitRepository must return false when null is passed as path");
      Assert.IsTrue(gitDir == null, "IsGitRepository must return set dir to null if path is invalid");
    }

    [Test]
    public void IsGitRepositoryShouldReturnTrueOnValidPath()
    {
      DirectoryInfo gitDir;
      Assert.IsTrue(Utility.IsGitRepository(Global.TestRepositoryPath, out gitDir), "IsGitRepository must return true for a valid path to a Git repository");
      Assert.IsTrue(gitDir != null && gitDir.Exists, "IsGitRepository cannot return dir as null when true was returned");
    }

    [Test]
    public void CreateGitDirectoryStructureShouldCreateTheRightStructure()
    {
      // All the directories that should be created when calling Utility.CreateGitDirectoryStructure
      string[] directories = new string[] { "hooks", "info", "objects", @"objects\info", @"objects\pack", "refs", @"refs\heads", @"refs\tags" };
      string dotGitDirectory = Path.Combine(_fullTempTestRepoName, ".git");

      // Create .git directory structure
      Utility.CreateGitDirectoryStructure(dotGitDirectory);

      foreach (string dirName in directories)
        Assert.IsTrue(Directory.Exists(Path.Combine(dotGitDirectory, dirName)), String.Format("CreateGitDirectoryStructure must create '{0}' directory in the new .git dir", dirName));

      // Check for info\exclude
      Assert.IsTrue(
          File.Exists(Path.Combine(dotGitDirectory, @"info\exclude")),
          @"CreateGitDirectoryStructure must create a default info\exclude file"
        );

      // Check for config file
      Assert.IsTrue(
          File.Exists(Path.Combine(dotGitDirectory, "config")),
          "CreateGitDirectoryStructure must create a default config file"
        );

      // Check for description file
      Assert.IsTrue(
          File.Exists(Path.Combine(dotGitDirectory, "description")),
          "CreateGitDirectoryStructure must create a default description file"
        );

      // Check for HEAD file
      Assert.IsTrue(
        File.Exists(Path.Combine(dotGitDirectory, "HEAD")),
        "CreateGitDirectoryStructure must create a default HEAD file"
        );

      // Check contents of HEAD file
      Assert.IsTrue(
        File.ReadAllText(Path.Combine(dotGitDirectory, "HEAD")) == "ref: refs/heads/master",
        "CreateGitDirectoryStructure should create HEAD file with 'ref: refs/heads/master' as contents"
        );
    }

    [Test]
    public void IsValidSHAShouldReturnFalseForEmptyString()
    {
      Assert.IsFalse(Utility.IsValidSHA(String.Empty), "Empty strings are not valid SHA objects");
    }
    [Test]
    public void IsValidSHAShouldReturnTrueForEmptyString()
    {
      Assert.IsTrue(Utility.IsValidSHA("945ec04f178784065ffc4ed494f1f0a81af7b891"), "Valid SHA strings should return true");
    }


    [Test]
    public void StripDateShouldReturnMinDateForEmptyString()
    {
      string remainder = String.Empty;
      Assert.IsTrue(DateTime.MinValue == Utility.StripDate(String.Empty, out remainder), "StripDate should return DateTime.MinDate for empty strings");
    }

    [Test]
    public void StripDateRemainderShouldBeEmptyForEmptyString()
    {
      string remainder = String.Empty;
      Utility.StripDate(String.Empty, out remainder);
      Assert.IsTrue(String.Equals(String.Empty, remainder), "Remainder should be empty for an empty input string");
    }

    long dateTicks = 633650589250000000;
    private string stripDateInput = "Mark <sjonge@hotmail.com> 1229458525 +0100";

    [Test]
    public void StripDateRemainderShouldBeValidForAuthorLine()
    {
      string remainder = String.Empty;
      Utility.StripDate(stripDateInput, out remainder);
      Assert.IsTrue(String.Equals("Mark <sjonge@hotmail.com>", remainder), "Remainder should return valid information for a valid input string");
    }

    [Test]
    public void StripDateOutputShouldBeValidForAuthorLine()
    {
      string remainder = String.Empty;
      DateTime dt = Utility.StripDate(stripDateInput, out remainder);
      Assert.IsTrue(dt.Ticks == dateTicks,"Date should be a valid date for given input string");
    }

  }
}
