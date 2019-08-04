using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;

namespace Amphora.Api.Fillers
{
    public class InMemoryBinaryAmphora : IBinaryAmphoraFiller, IBinaryAmphoraDrinker
    {
        private Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();
        public bool IsAmphoraSupported(AmphoraModel amphora)
        {
            return amphora?.Class == AmphoraClass.Binary;
        }
        public async Task Fill(AmphoraModel amphora, Stream data)
        {
            if (amphora == null || amphora.Id == null)
            {
                throw new ArgumentException("Null Amphora cannot be filled");
            }
            var buffer = await ReadFullyAsync(data);
            this._store[amphora.Id] = buffer;
        }
        public Task<Stream> Drink(AmphoraModel amphora)
        {
            return Task<Stream>.Factory.StartNew(() =>
            {
                if (amphora == null || amphora.Id == null)
                {
                    throw new ArgumentException("Null Amphora cannot be drunk");
                }
                if (_store.ContainsKey(amphora.Id))
                {
                    var buffer = _store[amphora.Id];
                    return new MemoryStream(buffer);
                }
                else
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        public static async Task<byte[]> ReadFullyAsync(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}