using System.Collections.Generic;
using BeyondSearch.Filters;

namespace BeyondSearch.Common
{
    public interface IFilterTermFileReader
    {
        IEnumerable<FilteredKeyword> ReadFilterTerms(string filePath);
    }
}
