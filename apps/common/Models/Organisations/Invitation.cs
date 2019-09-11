namespace Amphora.Common.Models.Organisations
{
    public class Invitation
    {
        public Invitation() {/* Empty Constructor */}
        public Invitation(string email)
        {
            TargetEmail = email;
        }
        public string TargetEmail { get; set; }
    }
}