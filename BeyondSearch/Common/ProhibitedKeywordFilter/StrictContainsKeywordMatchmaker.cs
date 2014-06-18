using System;
using System.Collections.Generic;
using System.Linq;
using BeyondSearch.Filters;

namespace BeyondSearch.Common.ProhibitedKeywordFilter
{
    public class StrictContainsKeywordMatchmaker : IKeywordMatchmaker
    {
        private readonly IDictionary<string, FilteredKeyword> filterMap;

        public StrictContainsKeywordMatchmaker(IEnumerable<FilteredKeyword> filteredKeywords)
        {
            if (filteredKeywords == null)
            {
                throw new ArgumentNullException("filteredKeywords");
            }

            filterMap = new Dictionary<string, FilteredKeyword>();

            foreach (var filteredKeyword in filteredKeywords)
            {
                if (filterMap.ContainsKey(AddBeginningAndTrailingSpace(filteredKeyword.Keyword))) continue;

                filterMap.Add(AddBeginningAndTrailingSpace(filteredKeyword.Keyword), filteredKeyword);
            }
        }

        public IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string, FilteredKeyword> suspects)
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
                    var adjustedSuspect = AddBeginningAndTrailingSpace(suspect.Key);
                    matchedFilters[suspect.Key] = filterMap.Any(x => adjustedSuspect.Contains(x.Key))
                        ? new FilteredKeyword {Keyword = suspect.Key}
                        : null;
                }
                else
                {
                    matchedFilters[suspect.Key] = suspect.Value;
                }
            }

            return matchedFilters;
        }

        private static string AddBeginningAndTrailingSpace(string keyword)
        {
            return string.Format(" {0} ", keyword);
        }
    }
}
