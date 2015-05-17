using System;
using System.Collections.Generic;
using System.Linq;

namespace BeyondSearch.Filters
{
    public class KeywordMatchmaker : IKeywordMatchmaker
    {
        private readonly IEnumerable<IKeywordMatchmaker> matchmakers;

        public KeywordMatchmaker(IEnumerable<IKeywordMatchmaker> matchmakers)
        {
            if (matchmakers == null)
            {
                throw new ArgumentNullException("matchmakers");
            }

            this.matchmakers = matchmakers;
        }

        public IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string, FilteredKeyword> suspects)
        {
            if (suspects == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            foreach (var matchmaker in matchmakers)
            {
                suspects = matchmaker.FilterKeywords(suspects) as Dictionary<string, FilteredKeyword>;
                if (suspects != null && suspects.All(x => x.Value != null)) break;
            }

            return suspects;
        }
    }
}
