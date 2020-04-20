namespace Amphora.Common.Models.Platform
{
    public class EnvironmentInfo
    {
        public bool IsStaging { get; set; }
        public bool IsDevelopment { get; set; }
        public string? Stack { get; set; }
        public string? Location { get; set; }
    }
}