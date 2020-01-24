namespace Amphora.Api.Models
{
    public class FileWrapper
    {
        public FileWrapper(byte[] raw, string fileName)
        {
            Raw = raw;
            FileName = fileName;
        }

        public byte[] Raw { get; set; }
        public string FileName { get; set; }
    }
}