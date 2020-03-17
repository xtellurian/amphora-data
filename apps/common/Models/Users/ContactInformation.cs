namespace Amphora.Common.Models.Users
{
    public class ContactInformation
    {
        public ContactInformation()
        { }

        public ContactInformation(string? email, string? fullName)
        {
            Email = email;
            FullName = fullName;
        }

        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? FullName { get; set; }
    }
}