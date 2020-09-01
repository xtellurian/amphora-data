using System;
using Amphora.Api.Models.Dtos;

namespace Amphora.Api.Models.Feeds
{
    public interface IPost : IDto
    {
        DateTimeOffset Timestamp { get; set; }
        PostSubjectType SubjectType { get; }
        PostEventType EventType { get; }
        string SubjectId { get; set; }
        string Text { get; set; }
    }
}