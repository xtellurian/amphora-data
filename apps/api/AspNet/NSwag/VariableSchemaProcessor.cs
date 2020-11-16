using Microsoft.Azure.TimeSeriesInsights.Models;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Amphora.Api.AspNet.NSwag
{
    // This class adds the 'Kind' property to the Variable classes from TSI
    // This is required when the inheritence hierarchy is flattened
    // because the discriminator is lost
    public class VariableSchemaProcessor : ISchemaProcessor
    {
        private JsonSchemaProperty KindProperty(string discriminatorValue) =>
            new JsonSchemaProperty
            {
                IsRequired = true,
                Type = JsonObjectType.String,
                Description = $"Should be set to {discriminatorValue}"
            };

        public void Process(SchemaProcessorContext context)
        {
            if (context.Type == typeof(NumericVariable))
            {
                System.Console.WriteLine(context.Type);
                if (!context.Schema.Properties.ContainsKey("Kind"))
                {
                    context.Schema.Properties.Add("Kind", KindProperty("numeric"));
                }
            }
            else if (context.Type == typeof(AggregateVariable))
            {
                System.Console.WriteLine(context.Type);
                if (!context.Schema.Properties.ContainsKey("Kind"))
                {
                    context.Schema.Properties.Add("Kind", KindProperty("aggregate"));
                }
            }
            else if (context.Type == typeof(CategoricalVariable))
            {
                System.Console.WriteLine(context.Type);
                if (!context.Schema.Properties.ContainsKey("Kind"))
                {
                    context.Schema.Properties.Add("Kind", KindProperty("categorical"));
                }
            }
        }
    }
}