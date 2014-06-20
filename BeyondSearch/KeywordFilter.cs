using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using BeyondSearch.Filters;

namespace BeyondSearch
{
    public class KeywordFilter : IKeywordFilter
    {
        private List<string> filterList;

        #region IKeywordFilter Members

        public List<string> FilterList
        {
            get
            {
                return filterList;
            }
        }

        public List<string> Exact(IEnumerable<string> keywords)
        {
            var filterer = new KeywordFilterer(filterList);

            var filteredList = filterer.Filter(keywords);

            return ( from item in filteredList where item.Value == null select item.Key ).ToList();
        }

        public List<string> Exact1(IEnumerable<string> keywords)
        {
            var filterer = new KeywordFilterer(filterList);

            return keywords.Where( keyword => !filterer.IsProhibitedKeyword( keyword ) ).ToList();
        }

        public List<string> Contains(IEnumerable<string> keywords)
        {
            var filterCount = filterList.Count;

            //var filteredKeywords = (keywords.Cast<string>()
            //  .SelectMany( keyword => containsFilterList, ( keyword, filter ) => new { keyword, filter } )
            //  .Where( @t => (@t.keyword.IndexOf( @t.filter, StringComparison.InvariantCultureIgnoreCase ) == -1) )
            //  .Select( @t => @t.keyword )).ToList()
            //  .GroupBy( txt => txt )
            //  .Where( grouping => grouping.Count() == filterCount )
            //  .ToList()
            //  .Select( key => key.Key ).ToList();

            var filteredKeywords = (keywords
              .SelectMany(keyword => filterList, (keyword, filter) => new { keyword, filter })
              .Where(@t => (!@t.keyword.Contains(" " + @t.filter + " ")))
              .Select(@t => @t.keyword)).ToList()
              .GroupBy(txt => txt)
              .Where(grouping => grouping.Count() == filterCount)
              .ToList()
              .Select(key => key.Key).ToList();

            return filteredKeywords;
        }

        public void FillFilterList(IEnumerable<string> filters)
        {
            filterList = new List<string>();

            foreach (var filter in filters)
            {
                filterList.Add(filter);
            }
        }

        #endregion
    }
}
