using Newtonsoft.Json.Schema.Generation;
using Amphora.Common.Models;
using Newtonsoft.Json.Serialization;
using System;

namespace Amphora.Schemas.Library
{
    public class SchemaLibrary
    {
        public bool IsInLibrary(string id)
        {
            return string.Equals(id, IdValueSchema.LibraryId);
        }
        public AmphoraSchema Load(string id)
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

        public class IdValueSchema : AmphoraSchema
        {
            public const string LibraryId = "IdValue";
            class IdValue
            {
                public string Id { get; set; }
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