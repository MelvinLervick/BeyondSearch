using System.Collections.Generic;
using System.Windows.Controls;

namespace BeyondSearch
{
  interface IKeywordFilter
  {
    List<string> FilterList { get; }

    List<string> Exact( IEnumerable<string> keywords );
    List<string> StrictContains( IEnumerable<string> keywords );

    void FillFilterList( IEnumerable<string> filters );
  }
}
