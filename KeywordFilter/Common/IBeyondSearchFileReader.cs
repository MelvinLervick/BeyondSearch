using System.Collections.Generic;
using KeywordFilter.Filters;

namespace KeywordFilter.Common
{
    public interface IBeyondSearchFileReader
    {
        IEnumerable<FilteredKeyword> ReadTerms( string filePath, RecordFormat recordFormat );
    }
}
