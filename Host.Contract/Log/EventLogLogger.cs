using System;
using System.Diagnostics;

namespace Host.Contract.Log
{
    [Serializable]
    public class EventLogLogger : ILogger
    {
        private readonly string _source;
        private readonly int _eventId;

        public EventLogLogger(string source, int eventId)
        {
            _source = source;
            _eventId = eventId;
        }

        public void Write(string message)
        {
            EventLog.WriteEntry(_source, message, EventLogEntryType.Information, _eventId);
        }
    }
}