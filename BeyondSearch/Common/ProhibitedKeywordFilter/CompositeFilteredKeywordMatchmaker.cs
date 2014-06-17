using System;
using System.Collections.Generic;
using System.Linq;

namespace BeyondSearch.Common.ProhibitedKeywordFilter
{
    public class CompositeFilteredKeywordMatchmaker : IFilteredKeywordMatchmaker
    {
        private readonly IEnumerable<IFilteredKeywordMatchmaker> matchmakers;

        public CompositeFilteredKeywordMatchmaker(IEnumerable<IFilteredKeywordMatchmaker> matchmakers)
        {
            if (matchmakers == null)
            {
                throw new ArgumentNullException("matchmakers");
            }

            this.matchmakers = matchmakers;
        }

        public IDictionary<string, FilteredKeyword> AssociateMatchedFilteredKeywords(Dictionary<string, FilteredKeyword> suspects)
        {
            if (suspects == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            foreach (var matchmaker in matchmakers)
            {
                suspects = matchmaker.AssociateMatchedFilteredKeywords(suspects) as Dictionary<string, FilteredKeyword>;
                if (suspects != null && suspects.All(x => x.Value != null)) break;
            }

            return suspects;
        }
    }
}
