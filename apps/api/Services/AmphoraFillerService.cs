using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Services
{
    public class AmphoraFillerService : IAmphoraFillerService
    {
        private readonly IEntityStore<Amphora.Common.Models.Amphora> amphoraModelStore;
        private readonly IBinaryAmphoraFiller binaryAmphoraFiller;
        private readonly IEntityStore<Schema> schemaStore;

        public AmphoraFillerService(IEntityStore<Amphora.Common.Models.Amphora> amphoraModelStore,
            IBinaryAmphoraFiller binaryAmphoraFiller, 
            IEntityStore<Amphora.Common.Models.Schema> schemaStore)
        {
            this.amphoraModelStore = amphoraModelStore;
            this.binaryAmphoraFiller = binaryAmphoraFiller;
            this.schemaStore = schemaStore;
        }

        public async Task FillWithBinary(string amphoraId, Stream data)
        {
            var amphora = amphoraModelStore.Get(amphoraId);
            if (amphora == null) throw new ArgumentException($"Couldn't find amphora {amphoraId}");
            
            await binaryAmphoraFiller.Fill(amphora, data);
        }

        public async Task FillWithJson(string amphoraId, IEnumerable<JObject> jsonPayloads)
        {
            var amphoraModel = amphoraModelStore.Get(amphoraId);
            if (amphoraModel == null) throw new ArgumentException($"Couldn't find amphora {amphoraId}");

            

            var schemaId = amphoraModel.SchemaId;
            var schema = schemaStore.Get(schemaId);
            foreach (var jObj in jsonPayloads)
            {
                if (!schema.IsValid(jObj))
                {
                    throw new ArgumentException("Invalid Payload");
                }
            }
            // TODO
            throw new NotImplementedException();
        }
    }
}