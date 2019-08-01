using System;
using System.Collections.Generic;
using api.Contracts;
using api.Models;

namespace api.Services
{
    public class InMemoryAmphoraModelService : IAmphoraModelService
    {
        private Dictionary<string, AmphoraModel> amphoraeStore = new Dictionary<string, AmphoraModel>();
        public AmphoraModel GetAmphora(string id)
        {
            if (this.amphoraeStore.ContainsKey(id)){
                return this.amphoraeStore[id];
            }
            else 
            {
                return null;
            }
        }

        public IEnumerable<string> ListAmphoraeIds()
        {
            return this.amphoraeStore.Keys;
        }

        public AmphoraModel SetAmphora(AmphoraModel model)
        {
            if(string.IsNullOrEmpty(model.Id)) 
            {
                model.Id = Guid.NewGuid().ToString();
            }

            this.amphoraeStore[model.Id] = model;
            return model;
        }
    }
}