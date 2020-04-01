using System;
using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Admin
{
    public class Report
    {
        public List<LogMessage> LogMessages { get; set; } = new List<LogMessage>();

        public void Log(string message)
        {
            LogMessages.Add(new LogMessage(message));
        }

        public struct LogMessage
        {
            public LogMessage(string message, DateTimeOffset? timestamp = null)
            {
                Message = message;
                Timestamp = timestamp is null ? DateTime.Now : timestamp.Value;
            }

            public string Message { get; }
            public DateTimeOffset Timestamp { get; }
        }
    }
}