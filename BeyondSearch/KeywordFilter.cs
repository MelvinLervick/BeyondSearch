using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

    public List<string> Exact( ItemCollection keywords )
    {
      var filteredKeywords = new List<string>();

      foreach (var keyword in keywords)
      {
        filteredKeywords.Add( keyword.ToString() );
      }

      return filteredKeywords;
    }

    public List<string> Contains( ItemCollection keywords )
    {
      var filteredKeywords = new List<string>();

      foreach (var keyword in keywords)
      {
        filteredKeywords.Add( keyword.ToString() );
      }

      return filteredKeywords;
    }

    public void FillExactFilterList( ItemCollection filters )
    {
      exactFilterList = new List<string>();

      foreach ( var filter in filters )
      {
        exactFilterList.Add( filter.ToString() );
      }
    }

    public void FillContainsFilterList( ItemCollection filters )
    {
      containsFilterList = new List<string>();

      foreach (var filter in filters)
      {
        containsFilterList.Add( filter.ToString() );
      }
    }

    #endregion
  }
}
