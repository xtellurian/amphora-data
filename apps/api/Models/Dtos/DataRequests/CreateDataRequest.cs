using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;

namespace Amphora.Api.Models.Dtos.DataRequests
{
    public class CreateDataRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }

        public DataRequestModel ToEntity()
        {
            GeoLocation geo = null;
            if (Lat.HasValue && Lon.HasValue)
            {
                geo = new GeoLocation(Lon.Value, Lat.Value);
            }

            return new DataRequestModel(Name, Description, geo);
        }
    }
}