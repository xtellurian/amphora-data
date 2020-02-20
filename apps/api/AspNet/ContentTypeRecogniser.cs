namespace Amphora.Api.AspNet
{
    public static class ContentTypeRecogniser
    {
        public static string GetContentType(string file)
        {
            var f = file.ToLower();
            if (f.EndsWith("jpg") || f.EndsWith("jpeg"))
            {
                return "image/jpeg";
            }
            else if (f.EndsWith("png"))
            {
                return "image/png";
            }
            else if (f.EndsWith("tiff") || f.EndsWith("tif"))
            {
                return "image/tiff";
            }
            else if (f.EndsWith("svg"))
            {
                return "image/svg+xml";
            }
            else if (f.EndsWith("gif"))
            {
                return "image/gif";
            }
            else
            {
                return "application/octet-stream";
            }
        }

        public static bool IsImage(string file)
        {
            return GetContentType(file).StartsWith("image");
        }
    }
}