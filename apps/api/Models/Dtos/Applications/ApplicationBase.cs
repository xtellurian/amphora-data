namespace Amphora.Api.Models.Dtos.Applications
{
    public abstract class ApplicationBase
    {
        /// <summary>
        /// Gets or sets the name of your application.
        /// Will be shown on the consent page.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a url for front channel HTTP logouts.
        /// </summary>
        public string LogoutUrl { get; set; }
    }
}