using System;

namespace Host.Contract.Log
{
    [Serializable]
    public class ConsoleLogger : ILogger
    {        
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}