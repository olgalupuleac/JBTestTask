using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace JBTestTask
{
    /// <summary>
    /// 
    /// </summary> 
    public class Grep
    {
        private static readonly Logger Logger;

        /// <summary>
        /// Initializes logger.
        /// </summary>
        static Grep()
        {
            var consoleTarget = new ColoredConsoleTarget();
            var config = new LoggingConfiguration();
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
            LogManager.Configuration = config;
            Logger = LogManager.GetCurrentClassLogger();
        }


        /// <summary>
        /// Checks if the line matches the pattern. By default,
        /// returns true if a string contains the provided pattern. 
        /// </summary>
        /// <param name="line">is a line to be check</param>
        /// <returns>true if the line matches</returns>
        public delegate bool MatcherDelegate(string line);

        /// <summary>
        /// Prints the resulting strings. 
        /// </summary>
        /// <param name="filename">Is a name of the file where the matching line was found</param>
        /// <param name="index">Is a line number.</param>
        /// <param name="line">Is the </param>
        public delegate void PrinterDelegate(string filename, int index, string line);

        /// <summary>
        /// 
        /// </summary>
        public MatcherDelegate Matcher { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PrinterDelegate Printer { get; set; }

        public Grep(string pattern, bool coloredOutput = true)
        {
            Matcher = line => line.Contains(pattern);
            if (coloredOutput)
            {
                Printer = (filename, index, line) =>
                {
                    int indexPattern = line.IndexOf(pattern, StringComparison.Ordinal);
                    Debug.Assert(indexPattern != -1);
                    var start = line.Substring(0, indexPattern);
                    Logger.Debug($"{indexPattern + pattern.Length}, {line.Length}, {line}, {indexPattern}");
                    var end = indexPattern + pattern.Length == line.Length
                        ? ""
                        : line.Substring(indexPattern + pattern.Length);
                    ColoredConsole.PrintHighlighted($"{filename}:{index}: {start}", pattern, end);
                };
            }
            else
            {
                Printer = (filename, index, line) => ColoredConsole.Print($"{filename}:{index}: {line}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void Execute(string path)
        {
            Logger.Debug($"Current directory is {Directory.GetCurrentDirectory()}");
            if (File.Exists(path))
            {
                ProcessFile(path);
                return;
            }

            if (!Directory.Exists(path))
            {
                Logger.Warn($"{path} does not exist");
                throw new ArgumentException($"The path {path} does not exist");
            }

            ProcessDirectory(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void ProcessDirectory(string path)
        {
            Logger.Debug($"Processing directory {path}");
            try
            {
                Directory.GetDirectories(path).ToList().ForEach(ProcessDirectory);
                Directory.GetFiles(path).ToList().ForEach(ProcessFile);
            }
            catch (Exception exception)
            {
                Logger.Warn(exception.StackTrace);

                if (exception is UnauthorizedAccessException || exception is PathTooLongException)
                {
                    ColoredConsole.ReportError(exception.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        private void ProcessFile(string filename)
        {
            Logger.Debug($"Processing file {filename}");
            try
            {
                File.ReadLines(filename).Select((line, index) => (line, index)).Where(x => Matcher(x.line))
                    .ToList().ForEach(x => Printer(filename, x.index + 1, x.line));
            }
            catch (Exception exception)
            {
                Logger.Warn(exception.StackTrace);

                if (exception is UnauthorizedAccessException || exception is PathTooLongException ||
                    exception is SecurityException)
                {
                    ColoredConsole.ReportError(exception.Message);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}