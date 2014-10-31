using System.Collections.Generic;
using System.Linq;
using KeywordFilter.Filters;

namespace KeywordFilter
{
    public class KeywordFilterer
    {
        private List<string> suspectKeywordsList;
        private KeywordMatchmaker compositeMatchmaker;
        private List<FilteredKeyword> toLowerWordsInDictionary;
        private List<FilteredKeyword> toLowerCategorizedKeywordsToBeRemoved;
        private List<FilteredKeyword> toLowerUncategorizedKeywordsToBeRemoved;

        public List<IKeywordMatchmaker> Matchmakers { get; set; }

        public KeywordFilterer(IEnumerable<FilteredKeyword> uncategorizedKeywordsToBeRemoved, bool usePluralizationService = true)
        {
            suspectKeywordsList = new List<string>();
            StoreListOfUncategorizedKeywordsToBeRemoved(uncategorizedKeywordsToBeRemoved);
            StoreListOfWordsInDictionary(null);
            StoreListOfCategorizedKeywordsToBeRemoved(null);

            Matchmakers = new List<IKeywordMatchmaker>
            {
                new ExactMatchKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved)
                //new FuzzyContainsKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved),
                //new StrictContainsKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved),
                //new ContainsSansSpaceAndNumberKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved, usePluralizationService)
            };

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        #region Filters

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

        #endregion

        #region Set Dictionary, Categoryized Keywords, and Uncategorized Keywords to be Removed

        public void StoreListOfWordsInDictionary(IEnumerable<FilteredKeyword> wordsInDictionary)
        {
            toLowerWordsInDictionary = wordsInDictionary == null ? new List<FilteredKeyword>() : 
                wordsInDictionary.Select(
                    masterKeyword =>
                        new FilteredKeyword
                        {
                            Keyword = masterKeyword.Keyword.ToLower(),
                            Category = masterKeyword.Category,
                            CategoryBit = masterKeyword.CategoryBit
                        }).ToList();
        }

        public void StoreListOfCategorizedKeywordsToBeRemoved(IEnumerable<FilteredKeyword> categorizedKeywordsToBeRemoved)
        {
            toLowerCategorizedKeywordsToBeRemoved = categorizedKeywordsToBeRemoved == null ? new List<FilteredKeyword>() : 
                categorizedKeywordsToBeRemoved.Select(
                    masterKeyword =>
                        new FilteredKeyword
                        {
                            Keyword = masterKeyword.Keyword.ToLower(),
                            Category = masterKeyword.Category,
                            CategoryBit = masterKeyword.CategoryBit
                        }).ToList();
        }

        public void StoreListOfUncategorizedKeywordsToBeRemoved(IEnumerable<FilteredKeyword> uncategorizedKeywordsToBeRemoved)
        {
            toLowerUncategorizedKeywordsToBeRemoved = uncategorizedKeywordsToBeRemoved == null ? new List<FilteredKeyword>() : 
                uncategorizedKeywordsToBeRemoved.Select(
                    masterKeyword =>
                        new FilteredKeyword
                        {
                            Keyword = masterKeyword.Keyword.ToLower(),
                            Category = masterKeyword.Category,
                            CategoryBit = masterKeyword.CategoryBit
                        }).ToList();
        }

        #endregion

        #region Set MatchMakers

        public void SetMatchmakerToExactMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new ExactMatchKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToStrictContainsMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new StrictContainsKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToFuzzyContainsMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new FuzzyContainsKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToContainsSansSpaceAndNumberMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new ContainsSansSpaceAndNumberKeywordMatchmaker(toLowerUncategorizedKeywordsToBeRemoved));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToLucenePortStem(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new LucenePorterStemMatchmaker(toLowerUncategorizedKeywordsToBeRemoved));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToContainsMatch(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new ContainsMatchmaker(toLowerCategorizedKeywordsToBeRemoved));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        public void SetMatchmakerToDictionary(bool singleMatchmaker = true)
        {
            if (singleMatchmaker || Matchmakers == null) Matchmakers = new List<IKeywordMatchmaker>();

            Matchmakers.Add(new DictionaryMatchmaker(toLowerWordsInDictionary));

            compositeMatchmaker = new KeywordMatchmaker(Matchmakers);
        }

        #endregion
    }
}
