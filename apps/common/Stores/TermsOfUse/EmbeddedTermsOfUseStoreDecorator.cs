using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Stores.EFCore;

namespace Amphora.Common.Stores.TermsOfUse
{
    public class EmbeddedTermsOfUseStoreDecorator : StoreDecorator<TermsOfUseModel>
    {
        private static TermsOfUseModel standardTerms =
            new TermsOfUseModel("Standard Platform Terms",
            "The Amphora Data standard terms, as defined in the Service Agreement.")
            {
                Id = "0"
            };

        public EmbeddedTermsOfUseStoreDecorator(IEntityStore<TermsOfUseModel> store) : base(store)
        {
        }

        public override async Task<IEnumerable<TermsOfUseModel>> QueryAsync(Expression<Func<TermsOfUseModel, bool>> where, int skip, int take)
        {
            if (take <= 0)
            {
                return new List<TermsOfUseModel>();
            }
            else if (skip == 0)
            {
                // include embedded
                var results = new List<TermsOfUseModel>
                {
                    standardTerms
                };

                results.AddRange(await store.QueryAsync(where, skip, take - 1));
                return results;
            }
            else
            {
                // skip embedded
                return await store.QueryAsync(where, skip - 1, take);
            }
        }

        public override async Task<TermsOfUseModel?> ReadAsync(string id)
        {
            if (string.Equals(id, standardTerms.Id))
            {
                return standardTerms;
            }
            else
            {
                return await store.ReadAsync(id);
            }
        }
    }
}