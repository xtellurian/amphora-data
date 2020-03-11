namespace Amphora.Common.Models.Host
{
    public class HostOptions
    {
        public HostOptions() { }
        public HostOptions(string mainHost)
        {
            MainHost = mainHost;
        }

        public string? MainHost { get; set; }

        public string GetBaseUrl()
        {
            if (MainHost == null)
            {
                throw new System.NullReferenceException("MainHost is null");
            }

            if (MainHost.StartsWith("http://"))
            {
                throw new System.ArgumentException("MainHost should not start with unencryped http://");
            }

            if (!MainHost.EndsWith('/'))
            {
                MainHost += "/";
            }

            if (!MainHost.StartsWith("https://"))
            {
                return $"https://{MainHost}";
            }
            else
            {
                return MainHost;
            }
        }
    }
}