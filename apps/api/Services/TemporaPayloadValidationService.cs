using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Schemas.Library;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Services
{
    public class TemporaPayloadValidationService: ITemporaPayloadValidationService
    {
        private readonly IEntityStore<Schema> schemaStore;
        private readonly ILogger<TemporaPayloadValidationService> logger;
        private readonly SchemaLibrary library;

        public TemporaPayloadValidationService(IEntityStore<Schema> schemaStore, ILogger<TemporaPayloadValidationService> logger )
        {
            this.schemaStore = schemaStore;
            this.logger = logger;
            this.library = new Schemas.Library.SchemaLibrary();
        }

        public async Task<bool> IsValidAsync(Tempora tempora, JObject payload)
        {
            if (string.IsNullOrEmpty(tempora.SchemaId)) return false;
            var schema = await LoadSchema(tempora);

            return schema?.IsValid(payload) ?? false;
        }

        private async Task<Schema> LoadSchema(Tempora tempora)
        {
            var schema = library.Load(tempora.SchemaId);
            if(schema == null)
            {
                schema = await schemaStore.ReadAsync(tempora.SchemaId);
            }
            if(schema == null) return null;

            schema.JSchema.AllowAdditionalProperties = false;
            return schema;
        }
    }
}