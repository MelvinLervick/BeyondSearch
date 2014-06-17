using System.Collections.Generic;

namespace BeyondSearch.Common.ProhibitedKeywordFilter
{
    public interface IFilteredKeywordMatchmaker
    {
        IDictionary<string, FilteredKeyword> AssociateMatchedFilteredKeywords(Dictionary<string,FilteredKeyword> suspects); // (IEnumerable<string> suspects);
    }
}
