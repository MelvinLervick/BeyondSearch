using System.Collections.Generic;
using BeyondSearch.Filters;

namespace BeyondSearch.Common
{
    public interface IBeyondSearchFileReader
    {
        IEnumerable<FilteredKeyword> ReadTerms( string filePath, RecordFormat recordFormat );
    }
}
