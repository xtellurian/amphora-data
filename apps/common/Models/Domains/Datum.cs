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
        public virtual bool IsValid()
        {
            return this.T != null && this.Tempora != null;
        }
        
        [JsonProperty(Required = Required.Default)]        
        public DateTime T { get; set; }
        public string Tempora { get; set; }

    }
}