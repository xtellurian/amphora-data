using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Amphora.Api.AspNet
{
    public class AmphoraDataApiVersionOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            context.OperationDescription.Operation.Parameters.Add(
            new NSwag.OpenApiParameter
            {
                Name = ApiVersion.HeaderName,
                Kind = NSwag.OpenApiParameterKind.Header,
                Type = JsonObjectType.String, // this might be required for some clients.
                Schema = new JsonSchema { Type = JsonObjectType.String },
                IsRequired = false,
                Description = "API Version Number",
                // Default = ApiVersion.CurrentVersion.Major.ToString()
            });

            return true;
        }
    }
}