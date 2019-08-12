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
        public Datum(string tempora): this()
        {
            Tempora = tempora;
        }
        [JsonProperty(Required = Required.Default)]        
        public DateTime T { get; set; }
        public string Tempora { get; set; }

    }
}