using System.Collections.Generic;
using System.Windows.Controls;

namespace BeyondSearch
{
    interface IKeywordFilter
    {
        List<string> FilterList { get; }

        List<string> Contains(IEnumerable<string> keywords);
        List<string> Contains1(IEnumerable<string> keywords);
        List<string> Exact(IEnumerable<string> keywords);
        List<string> Exact1(IEnumerable<string> keywords);
        List<string> StrictContains(IEnumerable<string> keywords);
        List<string> StrictContains1(IEnumerable<string> keywords);
        List<string> ContainsSansSpaceAndNumber(IEnumerable<string> keywords);
        List<string> ContainsSansSpaceAndNumber1(IEnumerable<string> keywords);
        List<string> FuzzyContains( IEnumerable<string> keywords );
        List<string> FuzzyContains1( IEnumerable<string> keywords );
        List<string> LucenePorterStemContains( IEnumerable<string> keywords );
        List<string> LucenePorterStemContains1( IEnumerable<string> keywords );

        void FillFilterList(IEnumerable<string> filters);
    }
}
