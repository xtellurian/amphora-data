using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Amphorae.Signals
{
    public class Signal : CreateSignal
    {
        public Signal()
        {
        }

        public Signal(string id, string property, string valueType, IDictionary<string, string> attributes)
            : base(property, valueType, attributes)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}