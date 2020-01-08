using System.IO;
using System.Threading.Tasks;

namespace Amphora.Api.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <returns> The contents of the stream as a byte array. </returns>
        public static async Task<byte[]> ReadFullyAsync(this Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read <= 0) { return ms.ToArray(); }
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}