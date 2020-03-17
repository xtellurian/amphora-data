using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models.Logging
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

        public LoggerScope(ClaimsPrincipal principal, [CallerMemberName] string method = "") : this()
        {
            var userName = principal.GetUserName();
            if (userName != null) { this.Add(UserName, userName); }
            else { this.Add(UserName, "NULL"); }
            this.Add(Method, method);
            this.Add(Type, typeof(T));
        }
    }
}