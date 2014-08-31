using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using BeyondSearch.Common;
using BeyondSearch.Filters;

namespace BeyondSearch
{
    public class KeywordFilter : IKeywordFilter
    {
        private List<FilteredKeyword> filterList;
        KeywordFilterer filterer;

        #region IKeywordFilter Members

        public List<FilteredKeyword> FilterList
        {
            get
            {
                return filterList;
            }
        }

        public GoodOrBadKeywords Dictionary(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToDictionary();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public GoodOrBadKeywords Contains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToContainsMatch();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public GoodOrBadKeywords Exact(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToExactMatch();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public GoodOrBadKeywords StrictContains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToStrictContainsMatch();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public GoodOrBadKeywords ContainsSansSpaceAndNumber(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToContainsSansSpaceAndNumberMatch();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public GoodOrBadKeywords FuzzyContains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToFuzzyContainsMatch();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public GoodOrBadKeywords LucenePorterStemContains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToLucenePortStem();

            return new GoodOrBadKeywords(filterer.Filter(keywords));
        }

        public void FillFilterList(IEnumerable<FilteredKeyword> filters)
        {
            filterList = filters.Select( x => x ).ToList();

            filterer = new KeywordFilterer(filterList);
        }

        #endregion
    }
}
