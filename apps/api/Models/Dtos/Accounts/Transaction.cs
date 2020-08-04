using System;

namespace Amphora.Api.Models.Dtos.Accounts
{
    public class Transaction
    {
        public string Id { get; set; }
        public string AmphoraId { get; set; }
        public double Balance { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string Label { get; set; }
    }
}