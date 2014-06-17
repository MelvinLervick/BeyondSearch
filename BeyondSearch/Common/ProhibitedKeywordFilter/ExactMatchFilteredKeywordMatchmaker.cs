using System;
using System.Collections.Generic;

namespace BeyondSearch.Common.ProhibitedKeywordFilter
{
    public class ExactMatchFilteredKeywordMatchmaker : IFilteredKeywordMatchmaker
    {
        private IDictionary<string, FilteredKeyword> filterMap;
 
        public ExactMatchFilteredKeywordMatchmaker(IList<FilteredKeyword> filteredKeywords)
        {
            if (filteredKeywords == null)
            {
                throw new ArgumentNullException("filteredKeywords");
            }

            filterMap = new Dictionary<string, FilteredKeyword>();

            foreach (var filteredKeyword in filteredKeywords)
            {
                if (filterMap.ContainsKey(filteredKeyword.Keyword)) continue;

                filterMap.Add(filteredKeyword.Keyword, filteredKeyword);
            }
        }

        public IDictionary<string, FilteredKeyword> AssociateMatchedFilteredKeywords(Dictionary<string, FilteredKeyword> suspects)
        {
            if (suspects == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            var matchedFilters = new Dictionary<string, FilteredKeyword>();
            foreach (var suspect in suspects)
            {
                if (matchedFilters.ContainsKey(suspect.Key)) continue;

                if (suspect.Value == null)
                {
                    FilteredKeyword suspectFilter;
                    matchedFilters[suspect.Key] = filterMap.TryGetValue(suspect.Key, out suspectFilter)
                        ? suspectFilter
                        : null;
                }
                else
                {
                    matchedFilters[suspect.Key] = suspect.Value;
                }
            }

            return matchedFilters;
        }
    }
}
