using System.Collections.Generic;

namespace BeyondSearch.Common.ProhibitedKeywordFilter
{
    public class ProhibitedKeywordFilterer
    {
        private readonly List<string> suspectKeywordsList;
        private readonly CompositeFilteredKeywordMatchmaker compositeMatchmaker;

        public ProhibitedKeywordFilterer(IEnumerable<FilteredKeyword> masterFilteredKeywords, bool usePluralizationService = true)
        {
            var toLowerMasterFilteredKeywords = new List<FilteredKeyword>();
            suspectKeywordsList = new List<string>();

            foreach (var masterKeyword in masterFilteredKeywords)
            {
                masterKeyword.Keyword = masterKeyword.Keyword.ToLower();
                toLowerMasterFilteredKeywords.Add(masterKeyword);
            }

            var matchMakers = new List<IFilteredKeywordMatchmaker>
            {
                new ExactMatchFilteredKeywordMatchmaker(toLowerMasterFilteredKeywords),
                new FuzzyContainsFilteredKeywordMatchmaker(toLowerMasterFilteredKeywords),
                new StrictContainsFilteredKeywordMatchmaker(toLowerMasterFilteredKeywords),
                new ContainsSansSpaceAndNumberFilteredKeywordMatchmaker(toLowerMasterFilteredKeywords, usePluralizationService)
            };

            compositeMatchmaker = new CompositeFilteredKeywordMatchmaker(matchMakers);
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

            return compositeMatchmaker.AssociateMatchedFilteredKeywords( suspects );
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
