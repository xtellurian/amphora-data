using System;
using Amphora.Api.Models.Feeds;

namespace Amphora.Api.Models.Dtos.Feeds
{
    public class FeedItem : IDto, IFeedEvent
    {
        public DateTimeOffset Timestamp { get; set; }

        public PostSubjectType SubjectType { get; set; }

        public PostEventType EventType { get; set; }

        public string SubjectId { get; set; }
        public string Text { get; set; }
    }
}