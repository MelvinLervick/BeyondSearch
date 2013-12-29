using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeyondSearch
{
  public class KeywordFilter : IKeywordFilter
  {
    private List<string> exactFilterList;
    private List<string> containsFilterList;
 
    #region IKeywordFilter Members

    public List<string> ExactFilterList
    {
      get
      {
        return exactFilterList;
      }
    }

    public List<string> ContainsFilterList
    {
      get
      {
        return containsFilterList;
      }
    }

    public List<string> Exact( IEnumerable<string> keywords )
    {
      var filteredKeywords = (from keyword in keywords
        join filter1 in exactFilterList.AsEnumerable() on keyword.Trim() equals filter1 into filterList
        from filter2 in filterList.DefaultIfEmpty()
        where filter2 == null
        select keyword).ToList();

      return filteredKeywords;
    }

    public List<string> Contains( IEnumerable<string> keywords )
    {
      var filterCount = containsFilterList.Count;

      //var filteredKeywords = (keywords.Cast<string>()
      //  .SelectMany( keyword => containsFilterList, ( keyword, filter ) => new { keyword, filter } )
      //  .Where( @t => (@t.keyword.IndexOf( @t.filter, StringComparison.InvariantCultureIgnoreCase ) == -1) )
      //  .Select( @t => @t.keyword )).ToList()
      //  .GroupBy( txt => txt )
      //  .Where( grouping => grouping.Count() == filterCount )
      //  .ToList()
      //  .Select( key => key.Key ).ToList();

      var filteredKeywords = (keywords
        .SelectMany( keyword => containsFilterList, ( keyword, filter ) => new {keyword, filter} )
        .Where( @t => (!@t.keyword.Contains( " "+@t.filter+" " )) )
        .Select( @t => @t.keyword )).ToList()
        .GroupBy(txt => txt)
        .Where(grouping => grouping.Count() == filterCount)
        .ToList()
        .Select( key => key.Key ).ToList();

      return filteredKeywords;
    }

    public void FillExactFilterList( IEnumerable<string> filters )
    {
      exactFilterList = new List<string>();

      foreach ( var filter in filters )
      {
        exactFilterList.Add( filter );
      }
    }

    public void FillContainsFilterList( IEnumerable<string> filters )
    {
      containsFilterList = new List<string>();

      foreach (var filter in filters)
      {
        containsFilterList.Add( filter );
      }
    }

    #endregion
  }
}
