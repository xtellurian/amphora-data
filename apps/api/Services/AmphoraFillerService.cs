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
        private readonly IAmphoraEntityStore<AmphoraModel> amphoraModelStore;
        private readonly IBinaryAmphoraFiller binaryAmphoraFiller;
        private readonly IAmphoraEntityStore<AmphoraSchema> schemaStore;

        public AmphoraFillerService(IAmphoraEntityStore<AmphoraModel> amphoraModelStore,
            IBinaryAmphoraFiller binaryAmphoraFiller, 
            IAmphoraEntityStore<AmphoraSchema> schemaStore)
        {
            this.amphoraModelStore = amphoraModelStore;
            this.binaryAmphoraFiller = binaryAmphoraFiller;
            this.schemaStore = schemaStore;
        }

        public async Task FillWithBinary(string amphoraId, Stream data)
        {
            var amphoraModel = amphoraModelStore.Get(amphoraId);
            if (amphoraModel == null) throw new ArgumentException($"Couldn't find amphora {amphoraId}");

            switch (amphoraModel.Class)
            {
                case AmphoraClass.Binary:
                    // handle
                    if(binaryAmphoraFiller.IsAmphoraSupported(amphoraModel))
                    {
                        await binaryAmphoraFiller.Fill(amphoraModel, data);
                    }
                    break;
                case AmphoraClass.TimeSeries:
                    // handle
                    throw new NotSupportedException("Cannot fill time series with binary data");
            }
        }

        public async Task FillWithJson(string amphoraId, IEnumerable<JObject> jsonPayloads)
        {
            var amphoraModel = amphoraModelStore.Get(amphoraId);
            if (amphoraModel == null) throw new ArgumentException($"Couldn't find amphora {amphoraId}");

            switch (amphoraModel.Class)
            {
                case AmphoraClass.Binary:
                    // handle
                    break;
                case AmphoraClass.TimeSeries:
                    // handle
                    break;
            }

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