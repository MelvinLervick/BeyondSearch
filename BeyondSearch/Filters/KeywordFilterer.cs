using System.Collections.Generic;
using System.Linq;

namespace BeyondSearch.Filters
{
    public class KeywordFilterer
    {
        private readonly List<string> suspectKeywordsList;
        private KeywordMatchmaker compositeMatchmaker;
        private readonly List<FilteredKeyword> toLowerMasterFilteredKeywords;

        public List<IKeywordMatchmaker> Matchmakers { get; set; }

        public KeywordFilterer(IEnumerable<string> masterFilteredKeywords, bool usePluralizationService = true)
        {
            suspectKeywordsList = new List<string>();

            toLowerMasterFilteredKeywords =
                masterFilteredKeywords.Select( masterKeyword => new FilteredKeyword {Keyword = masterKeyword.ToLower()} )
                    .ToList();

            Matchmakers = new List<IKeywordMatchmaker>
            {
                new ExactMatchKeywordMatchmaker(toLowerMasterFilteredKeywords)//,
                //new FuzzyContainsKeywordMatchmaker(toLowerMasterFilteredKeywords),
                //new StrictContainsKeywordMatchmaker(toLowerMasterFilteredKeywords),
                //new ContainsSansSpaceAndNumberKeywordMatchmaker(toLowerMasterFilteredKeywords, usePluralizationService)
            };

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
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

        public void SetMatchmakerToExactMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new ExactMatchKeywordMatchmaker(toLowerMasterFilteredKeywords));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToContainsMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new StrictContainsKeywordMatchmaker(toLowerMasterFilteredKeywords));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToFuzzyContainsMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new FuzzyContainsKeywordMatchmaker(toLowerMasterFilteredKeywords));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToContainsSansSpaceAndNumberMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new ContainsSansSpaceAndNumberKeywordMatchmaker(toLowerMasterFilteredKeywords));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }
    }
}
