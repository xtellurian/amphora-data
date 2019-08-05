using Amphora.Common.Contracts;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Amphora.Common.Models
{
    public class Schema : Entity, IEntity
    {
        protected JSchema _jSchema;
        public Schema() { }
        public Schema(JSchema jSchema)
        {
            this._jSchema = jSchema;
        }

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