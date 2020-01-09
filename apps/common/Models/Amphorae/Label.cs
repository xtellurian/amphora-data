namespace Amphora.Common.Models.Amphorae
{
    public class Label
    {
        public Label() { }
        public Label(string name)
        {
            Name = name.Trim();
        }

        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}