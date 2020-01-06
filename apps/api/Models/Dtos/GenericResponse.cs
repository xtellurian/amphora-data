namespace Amphora.Api.Models.Dtos
{
    public class GenericResponse
    {
        public GenericResponse() { }
        public GenericResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}