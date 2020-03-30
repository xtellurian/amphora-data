namespace Amphora.Common.Models.Organisations.Accounts
{
    public class Plan
    {
        public Plan()
        {
            PlanType = PlanTypes.Free; // default to free plan
        }

        public PlanTypes? PlanType { get; set; }
        public enum PlanTypes
        {
            Free = 0,
            Team = 1,
            Institution = 2
        }
    }
}