using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Test
{
  [TestFixture]	
  class RepositoryWriteTests
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
    public void ObjectWriteShouldReturnTrue()
    {
      Assert.IsTrue(false, "Not Implemented Yet");
    }


  }
}
