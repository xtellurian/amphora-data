using System;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models
{
    public class OnboardingState : Entity, IEntity
    {
        public string OnboardingStateId { get; set; }
        public DateTime? DateTimeExpires {get; set;}
        public DateTime DateTimeCreated {get; set;}
        public string EmailToInvite {get; set; }
        public bool IsNewOrganisation { get; set; }
        public string State { get; set; }
        public string ConsumedByUserId {get; set; }
        public override void SetIds()
        {
            this.OnboardingStateId = System.Guid.NewGuid().ToString();
            this.Id = this.OnboardingStateId.AsQualifiedId<OnboardingState>();
        }


        public static class States
        {
            public static string Invited => nameof(Invited);
            public static string Begun => nameof(Begun);
            public static string AwaitingOrganisation => nameof(AwaitingOrganisation);
            public static string Completed => nameof(Completed);

        }
    }
}