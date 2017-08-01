using System;

namespace Sample.AppDomain.App1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running App1 from Main");

            if (args.Length > 0)
            {
                Console.WriteLine("Arguments:");

                foreach (var arg in args)
                {
                    Console.WriteLine(arg);
                }
            }

//            Console.WriteLine("Press enter to exit.");
//            Console.ReadLine();
        }
    }
}
