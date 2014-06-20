using System.Collections.Generic;
using System.Linq;
using BeyondSearch.Common.ProhibitedKeywordFilter;

namespace BeyondSearch.Filters
{
    public class KeywordFilterer
    {
        private readonly List<string> suspectKeywordsList;
        private readonly KeywordMatchmaker compositeMatchmaker;

        public KeywordFilterer(IEnumerable<string> masterFilteredKeywords, bool usePluralizationService = true)
        {
            suspectKeywordsList = new List<string>();

            var toLowerMasterFilteredKeywords =
                masterFilteredKeywords.Select( masterKeyword => new FilteredKeyword {Keyword = masterKeyword.ToLower()} )
                    .ToList();

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
