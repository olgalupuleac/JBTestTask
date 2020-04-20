using System;
using System.Collections;
using System.IO;
using JBTestTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestGrep
{
    [TestClass]
    public class TestGrep
    {
        [AssemblyInitialize]
        public static void CreateTestFileSystem(TestContext context)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            Directory.SetCurrentDirectory(tempDirectory); 
            const string a = "a.txt";
            var b = "b.txt";
            Directory.CreateDirectory("test_dir");
            var c = Path.Combine("test_dir", "c.txt");
            Directory.CreateDirectory(Path.Combine("test_dir", "subdir"));
            var d = Path.Combine("test_dir", "subdir", "d.txt");
            string[] aLines =
            {
                "abaab",
                "aba",
                "ab",
                "aa",
                "bbb",
                "ba",
                "b",
                "bab"
            };
            File.WriteAllLines(a, aLines);

            string[] bLines =
            {
                "abaab",
                "aba",
                "ab",
                "aa",
                "bbb",
                "ba",
                "b",
                "bab"
            };
        }

        [TestMethod]
        public void TestSingleFile()
        {
            var results = new ArrayList();
            var grep = new Grep("ab")
            {
                Printer = (filename, index, line) =>
                    results.Add($"{filename}:{index}: {line}")
            };
            const string path = "a.txt";
            grep.Execute(path);
            CollectionAssert.AreEqual(
                new ArrayList
                {
                    $"{path}:1: abaab", $"{path}:2: aba",
                    $"{path}:3: ab", $"{path}:8: bab"
                }, results);
        }


    }


}