using System;

namespace JBTestTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: <pattern> <path>");
                return;
            }

            try
            {
                var grep = new Grep(args[0]);
                grep.Execute(args[1]);
                Console.WriteLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}