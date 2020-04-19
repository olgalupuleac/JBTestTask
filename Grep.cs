using System;
using System.IO;
using System.Linq;

namespace JBTestTask
{
    public class Grep
    {
        public delegate bool Matcher(string line);

        public delegate void Printer(string filename, int index, string line);

        private readonly Matcher _matcher;
        private readonly Printer _printer;


        public Grep(string pattern)
        {
            _matcher = line => line.Contains(pattern);
            _printer = (filename, index, line) => Console.WriteLine($"{filename}:{index}: {line}");
        }

        public Grep(Matcher matcher, Printer printer)
        {
            _matcher = matcher;
            _printer = printer;
        }

        public void Execute(string path)
        {
            if (File.Exists(path))
            {
                ProcessFile(path);
                return;
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException("The file or directory does not exist");
            }

            Directory.GetDirectories(path).ToList().ForEach(Execute);
            Directory.GetFiles(path).ToList().ForEach(ProcessFile);
        }

        private void ProcessFile(string filename)
        {
            File.ReadLines(filename).Select((line, index) => (line, index)).Where(x => _matcher(x.line))
                .ToList().ForEach(x => _printer(filename, x.index, x.line));
        }
    }
}