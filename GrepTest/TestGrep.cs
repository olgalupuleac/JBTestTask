using System.Collections;
using System.IO;
using JBTestTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GrepTest
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
                "foo"
            };

            File.WriteAllLines(b, bLines);

            string[] cLines =
            {
                "abaab",
                "ab"
            };

            File.WriteAllLines(c, cLines);

            string[] dLines =
            {
                "ccc",
                "ab"
            };
            File.WriteAllLines(d, dLines);
            using (File.Create("empty"))
            {
            }
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


        [TestMethod]
        public void TestMultipleFiles()
        {
            var results = new ArrayList();
            var grep = new Grep("ab")
            {
                Printer = (filename, index, line) =>
                    results.Add($"{filename}:{index}: {line}")
            };
            string aPath = Path.Combine(".", "a.txt");
            string bPath = Path.Combine(".", "b.txt");
            string cPath = Path.Combine(".", "test_dir", "c.txt");
            string dPath = Path.Combine(".", "test_dir", "subdir", "d.txt");
            grep.Execute(".");
            CollectionAssert.AreEquivalent(new ArrayList
            {
                $"{aPath}:1: abaab", $"{aPath}:2: aba",
                $"{aPath}:3: ab", $"{aPath}:8: bab",
                $"{bPath}:1: abaab", $"{cPath}:1: abaab",
                $"{cPath}:2: ab", $"{dPath}:2: ab"
            }, results);
        }

        [TestMethod]
        public void TestEmptyFile()
        {
            var results = new ArrayList();
            var grep = new Grep("ab")
            {
                Printer = (filename, index, line) =>
                    results.Add($"{filename}:{index}: {line}")
            };
            grep.Execute("empty");
            Assert.AreEqual(0, results.Count);
        }
    }
}