using System;
using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Services.Timing;

namespace Amphora.Api.Models.Dtos.Admin
{
    public class Report
    {
        private readonly IDateTimeProvider dtProvider;

        public Report(IDateTimeProvider dtProvider = null)
        {
            this.dtProvider = dtProvider ?? new DateTimeProvider();
        }

        public List<LogMessage> LogMessages { get; set; } = new List<LogMessage>();

        public void Log(string message)
        {
            LogMessages.Add(new LogMessage(message, Level.Information, dtProvider.Now));
        }

        public void Warning(string message)
        {
            LogMessages.Add(new LogMessage(message, Level.Warning, dtProvider.Now));
        }

        public void Error(string message)
        {
            LogMessages.Add(new LogMessage(message, Level.Error, dtProvider.Now));
        }

        public struct LogMessage
        {
            public LogMessage(string message, Level level, DateTimeOffset timestamp)
            {
                Message = message;
                Level = level;
                Timestamp = timestamp;
            }

            public Level Level { get; set; }
            public string Message { get; }
            public DateTimeOffset Timestamp { get; }
        }

        public enum Level
        {
            Information,
            Warning,
            Error
        }
    }
}