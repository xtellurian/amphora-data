namespace Amphora.Common.Models.Permissions.Rules
{
    public class AllRule : AccessRule
    {
        private string AllRuleId => System.Guid.NewGuid().ToString();
        public AllRule() : base()
        {
            this.Id = AllRuleId;
        }

        public AllRule(Kind? kind, int? priority)
        : base(kind, priority)
        {
            this.Id = AllRuleId;
        }

        public override string Name()
        {
            return $"{this.Kind}";
        }
    }
}