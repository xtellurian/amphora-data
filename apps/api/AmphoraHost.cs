namespace Amphora.Api
{
    public static class AmphoraHost
    {
        internal static void SetHost(string host)
        {
            if (host == null)
            {
                return;
            }

            if (host.StartsWith("http://"))
            {
                throw new System.ArgumentException("MainHost should not start with unencryped http://");
            }

            if (!host.EndsWith('/'))
            {
                host += "/";
            }

            if (!host.StartsWith("https://"))
            {
                MainHost = $"https://{host}";
            }
            else
            {
                MainHost = host;
            }
        }

        public static string MainHost { get; private set; } = "https://beta.amphoradata.com";
    }
}