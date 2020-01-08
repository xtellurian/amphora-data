namespace Amphora.Api.Models.Dtos.Amphorae.Files
{
    public class UploadResponse
    {
        public UploadResponse(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}