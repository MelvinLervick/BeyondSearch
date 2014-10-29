using System.Collections.Generic;
using KeywordFilter.Filters;

namespace KeywordFilter.Common
{
    public class GoodOrBadKeywords
    {
        public List<string> GoodKeywords { get; internal set; }
        public List<string> BadKeywords { get; internal set; }

        public GoodOrBadKeywords(IEnumerable<KeyValuePair<string, FilteredKeyword>> keywords)
        {
            GoodKeywords = new List<string>();
            BadKeywords = new List<string>();

            foreach ( var keyword in keywords )
            {
                if ( keyword.Value == null )
                {
                    GoodKeywords.Add( keyword.Key );
                }
                else
                {
                    BadKeywords.Add( keyword.Key );
                }
            }
        }
    }
}