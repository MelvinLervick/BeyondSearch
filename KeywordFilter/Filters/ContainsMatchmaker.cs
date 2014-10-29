using System;
using System.Collections.Generic;
using System.Linq;

namespace KeywordFilter.Filters
{
    public class ContainsMatchmaker : IKeywordMatchmaker
    {
        private readonly IDictionary<string, FilteredKeyword> filterMap;
        private const int Threshhold = 40;

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
                    foreach (var key in filterMap.Keys)
                    {
                        if (suspect.Key.Contains(key))
                        {
                            if (badSuspect == null)
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
                        if ((badSuspect != null) && (badSuspect.CategoryBit > Threshhold)) break;
                    }
                    //matchedFilters[suspect.Key] = BadSuspectSingleThreshhold(badSuspect);
                    matchedFilters[suspect.Key] = BadSuspectMultipleThreshholds(badSuspect, suspect.Key);
                }
                else
                {
                    matchedFilters[suspect.Key] = suspect.Value;
                }
            }

            return matchedFilters;
        }

        private static FilteredKeyword BadSuspectSingleThreshhold(FilteredKeyword badSuspect)
        {
            if ((badSuspect == null) || (badSuspect.CategoryBit <= Threshhold))
            {
                return null;
            }

            return badSuspect;
        }

        private static FilteredKeyword BadSuspectMultipleThreshholds(FilteredKeyword badSuspect, string suspectKey)
        {
            if (badSuspect == null) return null;

            int suspectWordCount = suspectKey.Split( ' ' ).Count();
            if ((suspectWordCount > 3 && badSuspect.CategoryBit < 24) ||
                (suspectWordCount == 2 && badSuspect.CategoryBit < 40) ||
                (suspectWordCount == 1 && badSuspect.CategoryBit < 64)
               )
            {
                return null;
            }

            return badSuspect;
        }
    }
}
