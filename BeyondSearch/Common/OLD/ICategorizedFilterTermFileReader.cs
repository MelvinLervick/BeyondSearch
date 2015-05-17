using System.Collections.Generic;
using BeyondSearch.Filters;

namespace BeyondSearch.Common
{
    public interface ICategorizedFilterTermFileReader
    {
        IEnumerable<FilteredKeyword> ReadFilterTerms(string filePath);
    }
}
