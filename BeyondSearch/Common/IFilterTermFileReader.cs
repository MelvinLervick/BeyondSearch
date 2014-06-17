using System.Collections.Generic;

namespace BeyondSearch.Common
{
    public interface IFilterTermFileReader
    {
        IEnumerable<string> ReadFilterTerms(string filePath);
    }
}
