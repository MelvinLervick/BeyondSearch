using System;
using System.Collections.Generic;
using System.Linq;

namespace BeyondSearch.Filters
{
    public class DictionaryMatchmaker : IKeywordMatchmaker
    {
        private readonly IDictionary<string, FilteredKeyword> filterMap;

        public DictionaryMatchmaker(IEnumerable<FilteredKeyword> filteredKeywords)
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

        public IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string, FilteredKeyword> suspects)
        {
            if (suspects == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            var matchedDictionary = new Dictionary<string, FilteredKeyword>();
            foreach (var suspect in suspects)
            {
                if (matchedDictionary.ContainsKey(suspect.Key)) continue;

                var words = suspect.Key.Split(' ');
                if (suspect.Value == null && words.Count() == 1)
                {
                    matchedDictionary[suspect.Key] = filterMap.ContainsKey(suspect.Key)
                    ? null
                    : new FilteredKeyword();
                }
                else
                {
                    matchedDictionary[suspect.Key] = suspect.Value;
                }
            }

            return matchedDictionary;
        }
    }
}
