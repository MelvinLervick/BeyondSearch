using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using BeyondSearch.Filters;

namespace BeyondSearch
{
    public class KeywordFilter : IKeywordFilter
    {
        private List<string> filterList;
        KeywordFilterer filterer;

        #region IKeywordFilter Members

        public List<string> FilterList
        {
            get
            {
                return filterList;
            }
        }

        public List<string> Contains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToContainsMatch();
            var filteredList = filterer.Filter(keywords);

            return (from item in filteredList where item.Value == null select item.Key).ToList();
        }

        public List<string> Contains1(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToContainsMatch();
            return keywords.Where(keyword => !filterer.IsProhibitedKeyword(keyword)).ToList();
        }

        public List<string> Exact(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToExactMatch();
            var filteredList = filterer.Filter(keywords);

            return ( from item in filteredList where item.Value == null select item.Key ).ToList();
        }

        public List<string> Exact1(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToExactMatch();
            return keywords.Where(keyword => !filterer.IsProhibitedKeyword(keyword)).ToList();
        }

        public List<string> StrictContains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToStrictContainsMatch();
            var filteredList = filterer.Filter(keywords);

            return (from item in filteredList where item.Value == null select item.Key).ToList();
        }

        public List<string> StrictContains1( IEnumerable<string> keywords )
        {
            filterer.SetMatchmakerToStrictContainsMatch();
            return keywords.Where(keyword => !filterer.IsProhibitedKeyword(keyword)).ToList();
        }

        public List<string> ContainsSansSpaceAndNumber(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToContainsSansSpaceAndNumberMatch();
            var filteredList = filterer.Filter(keywords);

            return (from item in filteredList where item.Value == null select item.Key).ToList();
        }

        public List<string> ContainsSansSpaceAndNumber1(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToContainsSansSpaceAndNumberMatch();
            return keywords.Where(keyword => !filterer.IsProhibitedKeyword(keyword)).ToList();
        }

        public List<string> FuzzyContains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToFuzzyContainsMatch();
            var filteredList = filterer.Filter(keywords);

            return (from item in filteredList where item.Value == null select item.Key).ToList();
        }

        public List<string> FuzzyContains1(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToFuzzyContainsMatch();
            return keywords.Where(keyword => !filterer.IsProhibitedKeyword(keyword)).ToList();
        }

        public List<string> LucenePorterStemContains(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToLucenePortStem();
            var filteredList = filterer.Filter(keywords);

            return (from item in filteredList where item.Value == null select item.Key).ToList();
        }

        public List<string> LucenePorterStemContains1(IEnumerable<string> keywords)
        {
            filterer.SetMatchmakerToLucenePortStem();
            return keywords.Where(keyword => !filterer.IsProhibitedKeyword(keyword)).ToList();
        }

        public void FillFilterList(IEnumerable<string> filters)
        {
            filterList = new List<string>();

            foreach (var filter in filters)
            {
                filterList.Add(filter);
            }

            filterer = new KeywordFilterer(filterList);
        }

        #endregion
    }
}
