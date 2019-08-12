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
        public static bool IsValid(this Amphora.Common.Models.Tempora tempora)
        {
            return
                tempora != null &&
                ! string.IsNullOrEmpty(tempora.Title) &&
                ! string.IsNullOrEmpty(tempora.Description)  &&
                ! string.IsNullOrEmpty(tempora.DomainId)  &&
                tempora.Price >= 0;
        }
    }
}