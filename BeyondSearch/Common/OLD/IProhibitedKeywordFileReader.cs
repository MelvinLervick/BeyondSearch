using System.Collections.Generic;
using BeyondSearch.Filters;

namespace BeyondSearch.Common
{
    public interface IProhibitedKeywordFileReader
    {
        IEnumerable<FilteredKeyword> ReadKeywords(string filePath);
    }
}
