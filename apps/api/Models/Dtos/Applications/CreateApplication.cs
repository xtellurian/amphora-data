using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Applications
{
    public class CreateApplication : ApplicationBase
    {
        /// <summary>
        /// Gets or sets a collection of locations your application will run.
        /// </summary>
        public IList<CreateAppLocation> Locations { get; set; } = new List<CreateAppLocation>();
    }
}