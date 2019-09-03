using System;
using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Common.Extensions
{
    public static class EntityExtensions
    {
        // prefixes
        private static readonly Dictionary<Type, string> prefixes = new Dictionary<Type, string>
        {
            {typeof(Amphora.Common.Models.Amphora), nameof(Amphora.Common.Models.Amphora)},
            {typeof(Amphora.Common.Models.Organisation), nameof(Amphora.Common.Models.Organisation)},
            {typeof(Amphora.Common.Models.OrganisationMembership), nameof(Amphora.Common.Models.OrganisationMembership)},
        };

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
                var entityPrefix = t.GetEntityPrefix();
                return $"{entityPrefix}|{id}";
            }
            else
            {
                throw new ArgumentException($"Type {t} does not implement {typeof(IEntity)}");
            }
        }

        public static string GetEntityPrefix(this Type t)
        {
            if(prefixes.ContainsKey(t))
            {
                return prefixes[t];
            }
            else
            {
                throw new ArgumentException($"Unknown type {t}");
            }
        }
    }
}