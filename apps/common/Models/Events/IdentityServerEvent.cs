using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class IdentityServerEvent : EventBase, IEvent
    {
        public IdentityServerEvent(string name, string message, string category)
        {
            var data = new EventDataDictionary("An Identity Server event occured.");
            data.Set("Name", name);
            data.Set("Message", message);
            data.Set("Category", category);
            this.Data = data;
            Subject = $"amphora/identity/identityserver/{category}";
        }

        public string EventType => "AmphoraData.IdentityServer";

        public IEventData Data { get; }
        public override string Subject { get; set; }
    }
}