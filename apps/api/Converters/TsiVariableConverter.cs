using System;
using System.Collections.Generic;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Converters
{
    public class TsiVariableConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Variable);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var item = JObject.Load(reader);

                if (item["kind"].Value<string>()?.ToLower() == "numeric")
                {
                    var o = item.ToObject<NumericVariable>();
                    return o;
                }
                else if (item["kind"].Value<string>()?.ToLower() == "aggregate")
                {
                    var o = item.ToObject<AggregateVariable>();
                    return o;
                }
                else
                {
                    throw new NotImplementedException("Unknown kind");
                }
            }
            else
            {
                throw new NotImplementedException("Unknown start");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // dont need this
            throw new NotImplementedException();
        }
    }
}