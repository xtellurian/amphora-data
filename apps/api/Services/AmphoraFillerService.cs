using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Services
{
    public class AmphoraFillerService : IAmphoraFillerService
    {
        private readonly IAmphoraEntityStore<AmphoraModel> amphoraModelStore;
        private readonly IAmphoraEntityStore<AmphoraSchema> schemaStore;

        public AmphoraFillerService(IAmphoraEntityStore<AmphoraModel> amphoraModelStore, IAmphoraEntityStore<AmphoraSchema> schemaStore)
        {
            this.amphoraModelStore = amphoraModelStore;
            this.schemaStore = schemaStore;
        }

        public async Task FillWithJson(string amphoraId, IEnumerable<JObject> jsonPayloads)
        {
            var amphoraModel = amphoraModelStore.Get(amphoraId);
            if (amphoraModel == null) throw new ArgumentException($"Couldn't find amphora {amphoraId}");

            var schemaId = amphoraModel.SchemaId;
            var schema = schemaStore.Get(schemaId);
            foreach(var jObj in jsonPayloads)
            {
                if( ! schema.IsValid(jObj))
                {
                    throw new ArgumentException("Invalid Payload");
                }
            }
            // TODO
            throw new NotImplementedException();
        }
    }
}