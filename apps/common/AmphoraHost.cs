namespace Amphora.Common
{
    public static class AmphoraHost
    {
        public static void SetHost(string host)
        {
            if (host == null)
            {
                return;
            }

            if (host.StartsWith("http://"))
            {
                throw new System.ArgumentException("MainHost should not start with unencryped http://");
            }

            host = host.TrimEnd('/');

            if (!host.StartsWith("https://"))
            {
                MainHost = $"https://{host}";
            }
            else
            {
                MainHost = host;
            }
        }

        public static string MainHost { get; private set; } = "https://app.amphoradata.com";
    }
}