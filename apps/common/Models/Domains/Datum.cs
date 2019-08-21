using System;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Domains
{
    public abstract class Datum
    {
        public Datum()
        {
            T = DateTime.Now;
        }
        public Datum(string amphora): this()
        {
            Amphora = amphora;
        }
        public virtual bool IsValid()
        {
            return this.T != null && this.Amphora != null;
        }
        
        [JsonProperty(Required = Required.Default)]        
        public DateTime T { get; set; }
        public string Amphora { get; set; }

    }
}