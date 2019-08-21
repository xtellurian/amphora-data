namespace Amphora.Api.Extensions
{
    public static class DtoModelValidation
    {
        public static bool IsValid(this Amphora.Common.Models.Amphora amphora)
        {
            return
                amphora != null &&
                ! string.IsNullOrEmpty(amphora.Title) &&
                ! string.IsNullOrEmpty(amphora.Description)  &&
                amphora.Price >= 0;
        }
    }
}