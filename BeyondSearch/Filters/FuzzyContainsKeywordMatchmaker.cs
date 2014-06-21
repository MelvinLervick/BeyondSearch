using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.QueryParsers;
using Version = Lucene.Net.Util.Version;

namespace BeyondSearch.Filters
{
    public class FuzzyContainsKeywordMatchmaker : IKeywordMatchmaker
    {
        private readonly IList<FilteredKeyword> filteredKeywords;
        private readonly IDictionary<string, IEnumerable<PorterStemTokenizer<FilteredKeyword>>> filterMap;

        public FuzzyContainsKeywordMatchmaker(IList<FilteredKeyword> filteredKeywords)
        {
            if (filteredKeywords == null)
            {
                throw new ArgumentNullException("filteredKeywords");
            }

            this.filteredKeywords = filteredKeywords;

            if (filterMap == null)
            {
                filterMap = GenerateFilterMap(filteredKeywords);
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
                    var suspectToCheck = new Dictionary<string, FilteredKeyword>
                    {
                        {suspect.Key, null}
                    };
                    var matches =
                        suspectToCheck.Keys.Select(x => new PorterStemTokenizer<string>(x, y => y))
                            .Select(x => new {suspect = x.Source, filter = x.GetFirstMatch(filterMap)}).Distinct()
                            .ToDictionary(x => x.suspect, y => y.filter == null ? null : y.filter.Source);
                    matchedFilters[suspect.Key] = matches[suspect.Key];
                }
                else
                {
                    matchedFilters[suspect.Key] = suspect.Value;
                }
            }

            return matchedFilters;
        }

        private IDictionary<string, IEnumerable<PorterStemTokenizer<FilteredKeyword>>> GenerateFilterMap(IEnumerable<FilteredKeyword> filteredKeywords)
        {
            var tokenizedFilters = filteredKeywords.Select(x => new PorterStemTokenizer<FilteredKeyword>(x, y => y.Keyword)).ToList();
            var expanded = tokenizedFilters.SelectMany(AssociateTokenizerToTokens);
            var grouped = CollapseAssociatedTokenizers(expanded);
            return grouped;
        }

        private IEnumerable<KeyValuePair<string, PorterStemTokenizer<FilteredKeyword>>> AssociateTokenizerToTokens(
            PorterStemTokenizer<FilteredKeyword> tokenizer)
        {
            return tokenizer.Tokens.Keys.ToDictionary(token => token, value => tokenizer);
        }

        private IDictionary<string, IEnumerable<PorterStemTokenizer<FilteredKeyword>>> CollapseAssociatedTokenizers(
            IEnumerable<KeyValuePair<string, PorterStemTokenizer<FilteredKeyword>>> associations)
        {
            var grouped = associations.GroupBy(association => association.Key, association => association.Value);
            return grouped.ToDictionary(group => group.Key, group => group.ToList().AsEnumerable());
        }

        private class PorterStemTokenizer<T>
        {
            public readonly string OriginalString;
            public readonly T Source;
            public readonly IDictionary<string, int> Tokens;

            public PorterStemTokenizer(T input, Func<T, string> keywordSelector)
            {
                Source = input;
                OriginalString = keywordSelector(input);
                Tokens = OriginalString.ToPorterStemNormalizedWithStopWords()
                    .Split(' ').GroupBy(x => x)
                    .ToDictionary(x => x.Key, y => y.Count());
            }

            public PorterStemTokenizer<TV> GetFirstMatch<TV>(IDictionary<string, IEnumerable<PorterStemTokenizer<TV>>> tokenizedFilters)
            {
                var narrowedFilters = NarrowFilters(tokenizedFilters);
                var match = narrowedFilters.FirstOrDefault(KeywordMatchesFilterItem);
                return match;
            }

            private IEnumerable<PorterStemTokenizer<TV>> NarrowFilters<TV>(IDictionary<string, IEnumerable<PorterStemTokenizer<TV>>> tokenizedFilters)
            {
                var filtersForSuspectTokens =
                    Tokens.Keys.Where(tokenizedFilters.ContainsKey).Select(x => tokenizedFilters[x]);

                var smallestFilterSet =
                    filtersForSuspectTokens.Where(x => x.Any()).OrderBy(x => x.Count()).FirstOrDefault();

                return smallestFilterSet ?? Enumerable.Empty<PorterStemTokenizer<TV>>();
            }

            private bool KeywordMatchesFilterItem<TV>(PorterStemTokenizer<TV> tokenizedFilter)
            {
                var matched =
                    tokenizedFilter.Tokens.All(
                        x =>
                            Tokens.ContainsKey(x.Key) &&
                            Tokens[x.Key] >= tokenizedFilter.Tokens[x.Key]);

                return matched;
            }
        }

    }
    public static class StringExtensions
    {
        public const string PorterAnalyzerName = "Porter";

        public static string ToPorterStemNormalizedWithStopWords(this string str)
        {
            str = str.ToLowerAlphabetical();

            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            var parse = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, string.Empty, new SnowballAnalyzer(Version.LUCENE_29, PorterAnalyzerName));
            return parse.Parse(QueryParser.Escape(str)).ToString().Trim();
        }

        public static string ToLowerAlphabetical(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            return str.ToLower().ToAlphabetical();
        }

        public static string ToAlphabetical(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            var strTokens = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(strTokens);

            return string.Join(" ", strTokens).Trim();
        }
    }
}
