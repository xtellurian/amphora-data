using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Events
{
    public class UserCreatedEvent : EventBase, IEvent
    {
        public UserCreatedEvent(ApplicationUser user)
        {
            Subject = user.Email;
            Data = new
            {
                Email = user.Email,
                UserId = user.Id,
                FullName = user.FullName,
                About = user.About
            };
        }

        public string EventType => "AmphoraData.Users.UserCreated";

        public object Data { get; private set; }

        public string Subject { get; private set; }
    }
}