using System.Collections.Generic;
using System.Windows.Controls;

namespace BeyondSearch
{
  interface IKeywordFilter
  {
    List<string> ExactFilterList { get; }
    List<string> ContainsFilterList { get; }

    List<string> Exact( IEnumerable<string> keywords );
    List<string> Contains( IEnumerable<string> keywords );

    void FillExactFilterList( IEnumerable<string> filters );
    void FillContainsFilterList( IEnumerable<string> filters );
  }
}
