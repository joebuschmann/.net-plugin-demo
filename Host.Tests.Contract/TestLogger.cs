using System;
using System.Collections.Generic;
using System.Linq;
using Host.Contract.Log;

namespace Host.Tests.Contract
{
    [Serializable]
    public class TestLogger : ILogger
    {
        private readonly List<string> _messages = new List<string>();

        public List<string> Messages => _messages.ToList();

        public void Write(string message)
        {
            _messages.Add(message);
        }
    }
}