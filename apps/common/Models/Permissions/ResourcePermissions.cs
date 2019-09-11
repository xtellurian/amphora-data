namespace Amphora.Common.Models
{
    public static class ResourcePermissions
    {
        public static string Create => nameof(Create);
        public static string Read => nameof(Read);
        public static string Update => nameof(Update);
        public static string Delete => nameof(Delete);

        // specific to amphorae contents
        public static string ReadContents => nameof(ReadContents);
        public static string WriteContents => nameof(WriteContents);
    }
}