using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordFilter.Filters
{
    public class ContainsSansSpaceAndNumberKeywordMatchmaker : IKeywordMatchmaker
    {
        private readonly bool usePluralizationService;
        private readonly IList<FilteredKeyword> filteredKeywords;
        private readonly StringBuilder stringBuilder;
        private IDictionary<string, FilteredKeyword> associateMatchedFilteredKeywordsReturnValue;
        private bool isCachedInitialized;
        private IDictionary<string, List<FilteredKeyword>> filteredKeywordTokenMap;

        public ContainsSansSpaceAndNumberKeywordMatchmaker(IList<FilteredKeyword> filteredKeywords, bool usePluralizationService = true)
        {
            if (filteredKeywords == null)
            {
                throw new ArgumentNullException("filteredKeywords");
            }

            this.stringBuilder = new StringBuilder();
            this.filteredKeywords = filteredKeywords;
            this.usePluralizationService = usePluralizationService;
        }

        public IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string, FilteredKeyword> suspects)
        {
            if (suspects == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            InitializeCache();

            associateMatchedFilteredKeywordsReturnValue = new Dictionary<string, FilteredKeyword>();

            foreach (var suspect in suspects)
            {
                if (suspect.Value == null)
                {
                    AssociateMatchedFilters(suspect.Key);
                }
                else
                {
                    associateMatchedFilteredKeywordsReturnValue[suspect.Key] = suspect.Value;
                }
            }

            return associateMatchedFilteredKeywordsReturnValue;
        }

        private void InitializeCache()
        {
            if (isCachedInitialized)
                return;

            filteredKeywordTokenMap = new Dictionary<string, List<FilteredKeyword>>(filteredKeywords.Count());

            foreach (var filterKeyword in filteredKeywords)
            {
                var tokens = filterKeyword.Keyword.Split(' ');
                var expandedTokenSet = ExpandTokenSet(tokens);

                foreach (var token in expandedTokenSet)
                {
                    if (!filteredKeywordTokenMap.ContainsKey(token))
                    {
                        filteredKeywordTokenMap[token] = new List<FilteredKeyword>();
                    }

                    filteredKeywordTokenMap[token].Add(filterKeyword);
                }
            }

            isCachedInitialized = true;
        }

        private void AssociateMatchedFilters(string suspect)
        {
            var suspectTokens = suspect.Split(' ');
            var expandedSuspectTokenSet = ExpandTokenSet(suspectTokens);

            if (usePluralizationService)
            {
                expandedSuspectTokenSet = ExpandTokenSetWithPlurals(expandedSuspectTokenSet);
            }

            var applicableFilters = new List<FilteredKeyword>();
            foreach (var token in expandedSuspectTokenSet)
            {
                List<FilteredKeyword> filterSubset;
                var additionalFilterSubset = new List<FilteredKeyword>();

                filteredKeywordTokenMap.TryGetValue(token, out filterSubset);

                if (filterSubset == null)
                    continue;

                foreach (var keyword in filterSubset)
                {
                    var singularPluralKeyword = SingularPluralKeyword.CreateInstance(keyword.Keyword);

                    if (singularPluralKeyword.HasBothSingularAndPlural)
                    {
                        additionalFilterSubset.Add(singularPluralKeyword.Singular == keyword.Keyword
                            ? new FilteredKeyword { Keyword = singularPluralKeyword.Plural }
                            : new FilteredKeyword { Keyword = singularPluralKeyword.Singular });
                    }
                }

                applicableFilters.AddRange(filterSubset);
                applicableFilters.AddRange(additionalFilterSubset);
            }

            applicableFilters = applicableFilters.Distinct().ToList();

            var filterKeywordPairs = applicableFilters
                .Select(x => new { filter = x, keyword = Normalize(x.Keyword) });

            var suspectNorm = Normalize(suspect);

            var matchedFiltered = filterKeywordPairs
                .FirstOrDefault(x => suspectNorm.Contains(x.keyword));

            if (!associateMatchedFilteredKeywordsReturnValue.ContainsKey(suspect))
            {
                associateMatchedFilteredKeywordsReturnValue.Add(suspect,
                    matchedFiltered == null ? null : matchedFiltered.filter);
            }
        }

        private static IEnumerable<string> ExpandTokenSet(string[] tokens)
        {
            var expandedSet = new List<string>();

            for (var combinationalCount = 1; combinationalCount <= tokens.Count(); combinationalCount++)
            {
                for (var startIndex = 0; startIndex + combinationalCount <= tokens.Count(); startIndex++)
                {
                    expandedSet.Add(string.Join("", tokens, startIndex, combinationalCount));
                }
            }

            return expandedSet;
        }

        private IEnumerable<string> ExpandTokenSetWithPlurals(IEnumerable<string> suspectTokens)
        {
            var expandedTokenSet = new List<string>();

            foreach (var suspectToken in suspectTokens)
            {
                var singularPluralKeyword = SingularPluralKeyword.CreateInstance(suspectToken);

                if (singularPluralKeyword.HasBothSingularAndPlural)
                {
                    expandedTokenSet.Add(singularPluralKeyword.Singular);
                    expandedTokenSet.Add(singularPluralKeyword.Plural);
                }
                
                expandedTokenSet.Add(singularPluralKeyword.OriginalKeyword);
            }

            return expandedTokenSet;
        }

        private string Normalize(string str)
        {
            stringBuilder.Clear();
            var normailizedStringArray = str.Split(new char[1] {' '});

            foreach (var item in normailizedStringArray.Where(item => !IsTokenNumeric(item)))
            {
                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }

        private static bool IsTokenNumeric(string token)
        {
            int result;

            return int.TryParse(token, out result);
        }
    }
}
