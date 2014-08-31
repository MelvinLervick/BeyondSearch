using System.Collections.Generic;
using System.Windows.Controls;
using BeyondSearch.Common;
using BeyondSearch.Filters;

namespace BeyondSearch
{
    interface IKeywordFilter
    {
        List<FilteredKeyword> FilterList { get; }

        GoodOrBadKeywords Dictionary(IEnumerable<string> keywords);
        GoodOrBadKeywords Contains(IEnumerable<string> keywords);
        GoodOrBadKeywords Exact(IEnumerable<string> keywords);
        GoodOrBadKeywords StrictContains(IEnumerable<string> keywords);
        GoodOrBadKeywords ContainsSansSpaceAndNumber(IEnumerable<string> keywords);
        GoodOrBadKeywords FuzzyContains( IEnumerable<string> keywords );
        GoodOrBadKeywords LucenePorterStemContains( IEnumerable<string> keywords );

        void FillFilterList(IEnumerable<FilteredKeyword> filters);
    }
}
