using System;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Feeds
{
    public class AmphoraCreatedPost : IPost
    {
        public AmphoraCreatedPost(AmphoraModel amphora)
        {
            SubjectId = amphora.Id;
            Timestamp = amphora.CreatedDate ?? DateTimeOffset.MinValue;
            if (amphora.CreatedBy == null)
            {
                Text = $"Amphora {amphora.Name} was created";
            }
            else
            {
                Text = $"Amphora {amphora.Name} was created by {amphora.CreatedBy.UserName}";
            }
        }

        public PostSubjectType SubjectType => PostSubjectType.Amphora;
        public PostEventType EventType => PostEventType.Created;
        public DateTimeOffset Timestamp { get; set; }
        public string SubjectId { get; set; }
        public string Text { get; set; }
    }
}
