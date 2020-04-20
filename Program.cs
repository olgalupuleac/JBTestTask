using System;

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

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                ColoredConsole.ReportError("Usage: <pattern> <path>");
                return;
            }

            var pattern = args[0];
            var path = args[1];


            try
            {
                var grep = new Grep(pattern);
                grep.Execute(path);
            }
            catch (Exception exception)
            {
                ColoredConsole.ReportError(exception.Message);
            }
        }
    }
}