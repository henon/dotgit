using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using dotGit.DiffEngine;

namespace Test
{
  [TestFixture]	
  public class DiffEngineTests
  {
    private string sourceFile1 = Path.Combine(Global.AssemblyDir, @"Resources\SourceFile1.txt");
    private string sourceFile2 = Path.Combine(Global.AssemblyDir, @"Resources\SourceFile2.txt");

    [SetUp]
    public void Setup()
    {
    }

    [TearDown]
    public void TearDown()
    {
    }

    [Test]
    [Category("Diff Engine")]
    public void DiffEngineShouldDetectBasicChangesInTwoFiles()
    {
      Diff d = Diff.Compare(sourceFile1, sourceFile2);
    }

    [Test]
    [ExpectedException(ExceptionType=typeof(FileNotFoundException), UserMessage="Non existent files should throw a FileNotFoundException")]
    [Category("Diff Engine")]
    public void DiffEngineShouldThrowExceptionOnNonExistentFiles()
    {
      string file1 = String.Format("{0}.txt", Guid.NewGuid().ToString());
      string file2 = String.Format("{0}.txt", Guid.NewGuid().ToString());

      Diff d = Diff.Compare(file1, file2);
    }
  }
}
