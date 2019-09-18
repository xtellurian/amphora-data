using System;
using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Transactions;
using Amphora.Common.Models.UserData;

namespace Amphora.Common.Extensions
{
    public static class EntityExtensions
    {
        // prefixes
        private static readonly Dictionary<Type, string> prefixes = new Dictionary<Type, string>
        {
            {typeof(AmphoraModel), "Amphora"},
            {typeof(OrganisationModel), "Organisation"},
            {typeof(UserDataModel), "UserData"},
            {typeof(TransactionModel), "Transaction"},
        };

        public static string AsQualifiedId<T>(this string id) where T: IEntity
        {
            return id.AsQualifiedId(typeof(T));
        }

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

        public static Type InferEntityType(this string id)
        {
            var split = id.Split('|');
            if(split.Length != 2) throw new ArgumentException($"Cannot infer type from id {id}");

            if(prefixes.Values.Any( (v) => string.Equals(split[0], v)))
            {
                var kvp = prefixes.ToList().FirstOrDefault( (x) => string.Equals(split[0], x.Value));
                return kvp.Key;
            }
            else
            {
                throw new ArgumentException($"Couldn't infer type from prefix {split[0]}");
            }
        }
    }
}