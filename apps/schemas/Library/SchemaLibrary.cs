using Newtonsoft.Json.Schema.Generation;
using Amphora.Common.Models;
using Newtonsoft.Json.Serialization;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Amphora.Schemas.Library
{
    public class SchemaLibrary
    {

        public List<Schema> List()
        {
            return new List<Schema>
            {
                Load(IdValueSchema.LibraryId)
            };
        }
        public bool IsInLibrary(string id)
        {
            return string.Equals(id, IdValueSchema.LibraryId);
        }

        public Schema Load(string id)
        {
            if (IsInLibrary(id))
            {
                switch (id)
                {
                    case IdValueSchema.LibraryId:
                        return new IdValueSchema();
                    default:
                        return null;
                }
            }
            else
            {
                throw new ArgumentException($"{id} not found in Schema Library");
            }
        }

        public class IdValueSchema : Schema
        {
            public const string LibraryId = "IdValue";
            class IdValue
            {
                [JsonProperty(Required = Required.Default)]
                public DateTime T { get; set; }
                [JsonProperty(Required = Required.Always)]
                public string Id { get; set; }
                [JsonProperty(Required = Required.Always)]
                public double Value { get; set; }
            }

            public IdValueSchema()
            {
                var generator = new JSchemaGenerator();
                generator.ContractResolver = new CamelCasePropertyNamesContractResolver();
                this._jSchema = generator.Generate(typeof(IdValue));
                this.Id = LibraryId;
            }
        }
    }
}