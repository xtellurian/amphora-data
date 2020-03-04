using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public partial class UnifiedSearchIndex : Index
    {
        public UnifiedSearchIndex()
        {
            Fields = new List<Field>();

            // Common Fields
            Fields.Add(new Field(nameof(EntityBase.Id), DataType.String)
            {
                IsKey = true
            });
            // type
            Fields.Add(new Field("Discriminator", DataType.String)
            {
                IsFilterable = true
            });
            Fields.Add(new Field(nameof(ISearchable.Name), DataType.String)
            {
                IsSearchable = true,
                IsRetrievable = true
            });
            Fields.Add(new Field(nameof(ISearchable.IsDeleted), DataType.Boolean)
            {
                IsFilterable = true,
            });
            Fields.Add(new Field(nameof(ISearchable.CreatedById), DataType.String)
            {
                IsSearchable = true,
                IsRetrievable = true
            });

            // partial word matching
            // https://medium.com/cloud-maker/azure-search-implementing-partial-word-search-c770cd052f39
            TokenFilters ??= new List<TokenFilter>();
            Analyzers ??= new List<Analyzer>();
            ScoringProfiles ??= new List<ScoringProfile>();

            // Custom Token Filters
            var customTokenFilter = new EdgeNGramTokenFilterV2("edgeNGramCmTokenFilter", 2, 20);
            TokenFilters.Add(customTokenFilter);

            // Custom Analyzers
            var prefixAnalyzer = new CustomAnalyzer("prefixCmAnalyzer", TokenizerName.Standard,
            new List<TokenFilterName>
            {
                TokenFilterName.Lowercase,
                TokenFilterName.AsciiFolding,
                customTokenFilter.Name
            });
            Analyzers.Add(prefixAnalyzer);

            var standardAnalyzer = new CustomAnalyzer("standardCmAnalyzer", TokenizerName.Standard,
            new List<TokenFilterName>
            {
                TokenFilterName.Lowercase,
                TokenFilterName.AsciiFolding
            });
            Analyzers.Add(standardAnalyzer);

            Fields.Add(new Field("PartialName", DataType.String)
            {
                IsSearchable = true,
                IndexAnalyzer = prefixAnalyzer.Name,
                SearchAnalyzer = standardAnalyzer.Name
            });

            var weights = new TextWeights(new Dictionary<string, double>
            {
                { nameof(ISearchable.Name), 2 },
                { "PartialName", 1 }
            });
            ScoringProfiles.Add(new ScoringProfile("exactFirst", weights));
            DefaultScoringProfile = "exactFirst";

            AmphoraFields();
            OrganisationFields();
            DataRequests();

            Name = IndexName;
        }

        public static string IndexName => $"{ApiVersion.CurrentVersion.ToSemver('-')}-unified";
    }
}