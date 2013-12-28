using System.Collections.Generic;
using System.Windows.Controls;

namespace BeyondSearch
{
  interface IKeywordFilter
  {
    List<string> ExactFilterList { get; }
    List<string> ContainsFilterList { get; }
    
    List<string> Exact( ItemCollection keywords );
    List<string> Contains( ItemCollection keywords );

    void FillExactFilterList( ItemCollection filters );
    void FillContainsFilterList( ItemCollection filters );
  }
}
