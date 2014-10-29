using System;
using System.Collections.ObjectModel;
using System.Linq;
using KeywordFilter.Filters;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.QueryParsers;
using Version = Lucene.Net.Util.Version;

namespace KeywordFilter.Common
{
    public static class Extensions
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

        public static void AddFilteredKeywordListItem( this ObservableCollection<FilteredKeyword> fkw, bool checkForDuplicates, string keyword, string category = "0", byte bit = 0 )
        {
            if (checkForDuplicates && fkw.Any(x => x.Keyword == keyword)) return;
            fkw.Add( new FilteredKeyword { Keyword = keyword, Category = category, CategoryBit = bit} );
        }
    }
}
