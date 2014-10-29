using System.Collections.Generic;

namespace KeywordFilter.Filters
{
    public interface IKeywordMatchmaker
    {
        IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string,FilteredKeyword> suspects);
    }
}
