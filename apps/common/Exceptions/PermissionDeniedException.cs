namespace Amphora.Common.Exceptions
{
    public class PermissionDeniedException: System.Exception
    {
        public PermissionDeniedException() {}

        public PermissionDeniedException(string message): base(message) {}
        public PermissionDeniedException(string message, System.Exception innerException): base(message, innerException) {}
    }
}