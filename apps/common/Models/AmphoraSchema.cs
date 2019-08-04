using common.Contracts;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Amphora.Common.Models
{
    public class AmphoraSchema : IAmphoraEntity
    {
        protected JSchema _jSchema;
        public AmphoraSchema() { }
        public AmphoraSchema(JSchema jSchema)
        {
            this._jSchema = jSchema;
        }

        public string Id { get; set; }
        public string JsonSchema
        {
            get
            {
                return this._jSchema?.ToString();
            }
            set
            {
                if(value != null) this._jSchema = JSchema.Parse(value);
                else this._jSchema = null;
            }
        }
        public JSchema JSchema => _jSchema;

        public bool IsValid(JObject jObj)
        {
            return jObj.IsValid(JSchema);
        }
    }
}