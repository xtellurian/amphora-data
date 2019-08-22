namespace Amphora.Common.Models
{
    public class Position 
    {
        public Position()
        {
            
        }
        public Position(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }
        public double Lat {get; set; }
        public double Lon {get; set; }
    }
}