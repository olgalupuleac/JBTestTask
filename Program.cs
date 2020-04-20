using System;
using NLog.Targets;

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

        public static void ReportError(string message)
        {
            var normalColor = Console.ForegroundColor;
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine(message);
            Console.ForegroundColor = normalColor;
        }

        public static void Print(string line)
        {
            Console.WriteLine(line);
        }

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
                var grep = new Grep(pattern)
                {
                    /*Printer = (filename, index, line) =>
                    {
                        Console.Write($"{filename}:{index}: ");
                        var match = line.IndexOf(pattern, StringComparison.Ordinal);
                        Console.Write(line.Substring(0, match + 1));
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write(pattern);
                        Console.ForegroundColor = DefaultTextColor;
                        Console.WriteLine(line.Substring(match + pattern.Length + 1));
                    }*/
                };
                grep.Execute(args[1]);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine();
        }
    }
}