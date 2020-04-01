namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraFileSize
    {
        public AmphoraFileSize(long sizeInBytes)
        {
            SizeInBytes = sizeInBytes;
        }

        public long SizeInBytes { get; set; }
    }
}