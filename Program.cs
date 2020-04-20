using System;
using clipr;

namespace JBTestTask
{
    internal class ColoredConsole
    {
        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private const ConsoleColor DefaultColor = ConsoleColor.Green;
        private const ConsoleColor HighlightColor = ConsoleColor.Magenta;

        static ColoredConsole()
        {
            Console.ForegroundColor = DefaultColor;
        }

        /// <summary>
        /// Prints an error message
        /// </summary>
        /// <param name="message">Message to be printed</param>
        public static void ReportError(string message)
        {
            var normalColor = Console.ForegroundColor;
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine(message);
            Console.ForegroundColor = normalColor;
        }

        /// <summary>
        /// Prints a matching line
        /// </summary>
        /// <param name="line">A line to be printed</param>
        public static void Print(string line)
        {
            Console.WriteLine(line);
        }

        /// <summary>
        /// Prints a line and highlights the first match.
        /// </summary>
        /// <param name="start">A part of the line before the match (default color)</param>
        /// <param name="match">The match (highlighted)</param>
        /// <param name="end">A part of the line after the match (default color)</param>
        public static void PrintHighlighted(string start, string match, string end)
        {
            Console.Write(start);
            Console.ForegroundColor = HighlightColor;
            Console.Write(match);
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine(end);
        }
    }


    [ApplicationInfo(Description = "This is a set of options.")]
    public class Options
    {
        [NamedArgument('c', "color", Action = ParseAction.StoreTrue,
            Description = "Highlight the first match in each output line")]
        public bool Colored { get; set; }

        [PositionalArgument(0,
            Description = "Pattern to be searched for")]
        public string Pattern { get; set; }

        [PositionalArgument(1, Description = "Path to file or directory")]
        public string Path { get; set; }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var opt = CliParser.Parse<Options>(args);
                var grep = new Grep(opt.Pattern, opt.Colored);
                grep.Execute(opt.Path);
            }
            catch (ParseException e)
            {
                ColoredConsole.ReportError(e.Message);
                ColoredConsole.ReportError("Usage: [-c|--color] <pattern> <path>");
            }
            catch (Exception e)
            {
                ColoredConsole.ReportError(e.Message);
            }
        }
    }
}