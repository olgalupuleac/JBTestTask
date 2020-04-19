using System;
using System.IO;
using System.Linq;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace JBTestTask
{
    public class Grep
    {
        private static readonly Logger Logger;

        static Grep()
        {
            var consoleTarget = new ColoredConsoleTarget();
            var config = new LoggingConfiguration();
            config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
            LogManager.Configuration = config;
            Logger = LogManager.GetCurrentClassLogger();
        }

        public delegate bool MatcherDelegate(string line);

        public delegate void PrinterDelegate(string filename, int index, string line);

        public MatcherDelegate Matcher { get; set; }
        public PrinterDelegate Printer { get; set; }

        public Grep(string pattern)
        {
            Matcher = line => line.Contains(pattern);
            Printer = (filename, index, line) => Console.WriteLine($"{filename}:{index}: {line}");
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
                Logger.Error($"{path} does not exist");
                throw new ArgumentException($"The path {path} does not exist");
            }

            ProcessDirectory(path);
        }

        private void ProcessDirectory(string path)
        {
            Logger.Info($"Processing directory {path}");
            Directory.GetDirectories(path).ToList().ForEach(ProcessDirectory);
            Directory.GetFiles(path).ToList().ForEach(ProcessFile);
        }

        private void ProcessFile(string filename)
        {
            Logger.Info($"Processing file {filename}");
            File.ReadLines(filename).Select((line, index) => (line, index)).Where(x => Matcher(x.line))
                .ToList().ForEach(x => Printer(filename, x.index, x.line));
        }
    }
}