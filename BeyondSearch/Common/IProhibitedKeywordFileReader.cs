using System.Collections.Generic;

namespace BeyondSearch.Common
{
    public interface IProhibitedKeywordFileReader
    {
        IEnumerable<string> ReadKeywords(string filePath);
    }
}
