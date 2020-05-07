using System.Net.Http;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Dtos.Users;

namespace Amphora.Tests.Helpers
{
    public class UserCache
    {
        public UserCache(string name, HttpClient http, AmphoraUser user, Organisation organisation)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Http = http ?? throw new System.ArgumentNullException(nameof(http));
            User = user ?? throw new System.ArgumentNullException(nameof(user));
            Organisation = organisation ?? throw new System.ArgumentNullException(nameof(organisation));
        }

        public string Name { get; }
        public HttpClient Http { get; }
        public AmphoraUser User { get; }
        public Organisation Organisation { get; }
    }
}