namespace Amphora.Api.Models.Dtos.Amphorae.Files
{
    public class UploadResponse : Response
    {
        public UploadResponse(string url) : base()
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}