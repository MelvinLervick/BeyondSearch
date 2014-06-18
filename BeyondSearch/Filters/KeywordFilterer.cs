using System.Collections.Generic;
using BeyondSearch.Common.ProhibitedKeywordFilter;

namespace BeyondSearch.Filters
{
    public class KeywordFilterer
    {
        private readonly List<string> suspectKeywordsList;
        private readonly KeywordMatchmaker compositeMatchmaker;

        public KeywordFilterer(IEnumerable<FilteredKeyword> masterFilteredKeywords, bool usePluralizationService = true)
        {
            var toLowerMasterFilteredKeywords = new List<FilteredKeyword>();
            suspectKeywordsList = new List<string>();

            foreach (var masterKeyword in masterFilteredKeywords)
            {
                masterKeyword.Keyword = masterKeyword.Keyword.ToLower();
                toLowerMasterFilteredKeywords.Add(masterKeyword);
            }

            var matchMakers = new List<IKeywordMatchmaker>
            {
                new ExactMatchKeywordMatchmaker(toLowerMasterFilteredKeywords)//,
                //new FuzzyContainsKeywordMatchmaker(toLowerMasterFilteredKeywords),
                //new StrictContainsKeywordMatchmaker(toLowerMasterFilteredKeywords),
                //new ContainsSansSpaceAndNumberKeywordMatchmaker(toLowerMasterFilteredKeywords, usePluralizationService)
            };

            compositeMatchmaker = new KeywordMatchmaker(matchMakers);
        }

        public IDictionary<string, FilteredKeyword> Filter(IEnumerable<string> suspectKeywords)
        {
            if (suspectKeywords == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            var suspects = new Dictionary<string, FilteredKeyword>();
            foreach (var suspectKeyword in suspectKeywords)
            {
                var lowerSuspect = suspectKeyword.ToLower();
                if (suspects.ContainsKey(lowerSuspect)) continue;
                suspects.Add(lowerSuspect, null);
            }

            return compositeMatchmaker.FilterKeywords( suspects );
        }

        public bool IsProhibitedKeyword(string suspectKeyword)
        {
            suspectKeywordsList.Clear();
            suspectKeywordsList.Add(suspectKeyword);

            var filteredKeywords = Filter(suspectKeywordsList);

            if (filteredKeywords[suspectKeyword.ToLower()] == null)
            {
                return false;
            }

            return true;
        }
    }
}
