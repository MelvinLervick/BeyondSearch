using System.Collections.Generic;

namespace BeyondSearch.Filters
{
    public interface IKeywordMatchmaker
    {
        IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string,FilteredKeyword> suspects);
    }
}
