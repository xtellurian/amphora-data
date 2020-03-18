using System;

namespace Amphora.Common.Exceptions
{
    public class IdentityServerException : System.ApplicationException
    {
        public IdentityServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}