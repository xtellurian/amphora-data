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
                Schema = new JsonSchema { Type = JsonObjectType.String },
                IsRequired = true,
                Description = "API Version Number",
                Default = ApiVersion.CurrentVersion.Major.ToString()
            });

            return true;
        }
    }
}