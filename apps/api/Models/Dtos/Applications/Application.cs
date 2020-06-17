using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Applications
{
    public class Application : ApplicationBase
    {
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a collection of locations your application will run.
        /// </summary>
        public IList<AppLocation> Locations { get; set; } = new List<AppLocation>();
    }
}