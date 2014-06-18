using System.Collections.Generic;
using BeyondSearch.Common.ProhibitedKeywordFilter;

namespace BeyondSearch.Filters
{
    public interface IKeywordMatchmaker
    {
        IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string,FilteredKeyword> suspects);
    }
}
