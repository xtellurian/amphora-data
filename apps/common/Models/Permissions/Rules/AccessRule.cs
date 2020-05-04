namespace Amphora.Common.Models.Permissions.Rules
{
    public abstract class AccessRule
    {
        protected AccessRule(Kind? kind, int? priority)
        {
            Kind = kind;
            Priority = priority;
        }

        protected AccessRule()
        { }

        public string Id { get; set; } = null!;
        public Kind? Kind { get; set; }
        public int? Priority { get; set; }
        public abstract string Name();
    }
}