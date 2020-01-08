using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amphora.Common.Contracts;

namespace Amphora.Api.Services
{
    public class LoggerScope<T> : Dictionary<string, object>
    {
        private string UserName => nameof(UserName);
        private string Method => nameof(Method);
        private string Type => nameof(Type);

        public LoggerScope() : base() { }
        public LoggerScope(IUser user, [CallerMemberName] string method = "") : this()
        {
            if (user?.UserName != null) { this.Add(UserName, user.UserName); }
            else { this.Add(UserName, "NULL"); }
            this.Add(Method, method);
            this.Add(Type, typeof(T));
        }
    }
}