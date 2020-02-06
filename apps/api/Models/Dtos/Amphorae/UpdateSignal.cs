using System.Collections.Generic;

namespace Amphora.Common.Models.Amphorae
{
    public class UpdateSignal
    {
        public UpdateSignal()
        {
        }

        public UpdateSignal(IDictionary<string, string> meta)
        {
            Meta = meta;
        }

        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
    }
}