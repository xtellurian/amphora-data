using System;
using System.Text.Json.Serialization;
using Amphora.Common.Models.Purchases;
using Newtonsoft.Json.Converters;

namespace Amphora.Api.Models.Feeds
{
    public class AmphoraPurchasedFeedEvent : IFeedEvent
    {
        public AmphoraPurchasedFeedEvent(PurchaseModel purchase)
        {
            SubjectId = purchase.AmphoraId;
            Timestamp = purchase.CreatedDate ?? DateTimeOffset.MinValue;
            if (purchase.Amphora == null && purchase.PurchasedByUser == null)
            {
                Text = $"An Amphora was purchased.";
            }
            else if (purchase.PurchasedByUser == null)
            {
                Text = $"The Amphora '{purchase.Amphora.Name}' was purchased.";
            }
            else if (purchase.Amphora == null)
            {
                Text = $"An Amphora was purchased by ${purchase.PurchasedByUser.UserName}.";
            }
            else
            {
                Text = $"Amphora {purchase.Amphora.Name} was purchased by {purchase.PurchasedByUser.UserName}";
            }
        }

        public PostSubjectType SubjectType => PostSubjectType.Amphora;
        public PostEventType EventType => PostEventType.Purchased;
        public DateTimeOffset Timestamp { get; set; }
        public string SubjectId { get; set; }
        public string Text { get; set; }
    }
}