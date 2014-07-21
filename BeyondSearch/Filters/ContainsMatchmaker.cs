using System;
using System.Collections.Generic;
using System.Linq;

namespace BeyondSearch.Filters
{
    public class ContainsMatchmaker : IKeywordMatchmaker
    {
        private readonly IDictionary<string, FilteredKeyword> filterMap;
 
        public ContainsMatchmaker(IEnumerable<FilteredKeyword> filteredKeywords)
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

            var matchedFilters = new Dictionary<string, FilteredKeyword>();
            foreach (var suspect in suspects)
            {
                if (matchedFilters.ContainsKey(suspect.Key)) continue;

                if (suspect.Value == null)
                {
                    FilteredKeyword badSuspect = null;
                    foreach ( var key in filterMap.Keys )
                    {
                        if ( suspect.Key.Contains( key ) )
                        {
                            if ( badSuspect == null )
                            {
                                var filter = filterMap[key];
                                badSuspect = new FilteredKeyword
                                {
                                    Keyword = filter.Keyword,
                                    Category = filter.Category,
                                    CategoryBit = filter.CategoryBit
                                };
                            }
                            else
                            {
                                badSuspect.CategoryBit = (byte)(badSuspect.CategoryBit | filterMap[key].CategoryBit);
                            }
                        }
                        if ((badSuspect != null) && (badSuspect.CategoryBit > 32)) break;
                    }
                    if ( (badSuspect == null) || (badSuspect.CategoryBit <= 32))
                    {
                        matchedFilters[suspect.Key] = null;
                    }
                    else
                    {
                        matchedFilters[suspect.Key] = badSuspect;
                    }
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
