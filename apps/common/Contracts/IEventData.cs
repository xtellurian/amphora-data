using System.Collections.Generic;

namespace Amphora.Common.Contracts
{
    public interface IEventData : IDictionary<string, string>
    {
        /// <summary>
        /// The user friendly name of the event.
        /// </summary>
        string FriendlyName { get; set; }
    }
}