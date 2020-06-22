using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Applications
{
    public class UpdateApplication : ApplicationBase
    {
        public UpdateApplication() { }

        public UpdateApplication(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a collection of locations your application will run.
        /// </summary>
        public IList<CreateAppLocation> Locations { get; set; } = new List<CreateAppLocation>();
    }
}