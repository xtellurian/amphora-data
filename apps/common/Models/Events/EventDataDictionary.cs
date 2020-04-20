using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class EventDataDictionary : Dictionary<string, string>, IEventData
    {
        public EventDataDictionary(string friendlyName)
        {
            FriendlyName = friendlyName;
            Set(nameof(FriendlyName), friendlyName);
        }

        public string FriendlyName { get; set; }

        public void Set(string key, string? value)
        {
            this[key] = value ?? "Unknown";
        }

        public void SetAmphoraId(string? amphoraId)
        {
            Set("AmphoraId", amphoraId);
        }

        public void SetOrganisationId(string? organisationId)
        {
            Set("OrganisationId", organisationId);
        }

        public void SetTriggeredByUsername(string? username)
        {
            Set("TriggeredByUsername", username);
        }
    }
}