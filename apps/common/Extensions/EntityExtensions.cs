using System;
using Amphora.Common.Contracts;
using Amphora.Common.Models;

namespace Amphora.Common.Extensions
{
    public static class EntityExtensions
    {
        private static string amphoraPrefix = "Amphora";
        private static string organisationPrefix = "Organisation";
        private static string organisationMembershipPrefix = "OrganisationMembership";
        public static string AsQualifiedId(this string id, Type t)
        {
            if(id == null) throw new ArgumentException("Cannot qualify a null Id");
            if (typeof(IEntity).IsAssignableFrom(t))
            {
                var split = id.Split('|');
                if (split.Length == 2)
                {
                    return id;
                }
                if (t == typeof(Amphora.Common.Models.Amphora) && !string.Equals(split[0], amphoraPrefix))
                    return $"{amphoraPrefix}|{id}";
                else if (t == typeof(Organisation) && !string.Equals(split[0], organisationPrefix))
                    return $"{organisationPrefix}|{id}";
                else if (t == typeof(OrganisationMembership) && !string.Equals(split[0], organisationMembershipPrefix))
                    return $"{organisationMembershipPrefix}|{id}";
            }
            else
            {
                throw new ArgumentException($"Type {t} does not implement {typeof(IEntity)}");
            }
            return id;
        }
    }
}