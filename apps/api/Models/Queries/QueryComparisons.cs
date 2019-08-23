namespace Amphora.Api.Models.Queries
{
    // This is a duplicate of Microsoft.Azure.Cosmos.Table.QueryComparisons
    // it is meant to minimise coupling
    public static class QueryComparisons
    {
        public const string Equal = "eq";
        public const string NotEqual = "ne";
        public const string GreaterThan = "gt";
        public const string GreaterThanOrEqual = "ge";
        public const string LessThan = "lt";
        public const string LessThanOrEqual = "le";
    }
}
