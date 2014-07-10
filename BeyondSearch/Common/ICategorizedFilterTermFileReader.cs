using System.Collections.Generic;
using BeyondSearch.Common.CategorizedFilterReader;

namespace BeyondSearch.Common
{
    public interface ICategorizedFilterTermFileReader
    {
        IEnumerable<CategorizedFilterTerm> ReadFilterTerms(string filePath);
    }
}
