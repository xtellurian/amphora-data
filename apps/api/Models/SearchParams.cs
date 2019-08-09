namespace Amphora.Api.Models
{
    public class SearchParams
    {
        public SearchParams()
        {
            
        }
        public bool IncludeAmphorae { get; set; } = true;
        public bool IncludeTemporae { get; set; } = true;
    }
}