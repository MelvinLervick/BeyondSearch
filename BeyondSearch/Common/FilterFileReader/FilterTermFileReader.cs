using System.Collections.Generic;
using System.Linq;
using BeyondSearch.Filters;
using LINQtoCSV;

namespace BeyondSearch.Common.FilterFileReader
{
    public class FilterTermFileReader : IFilterTermFileReader
    {
        private readonly CsvFileDescription fileDescription;

        public FilterTermFileReader()
        {
            fileDescription = new CsvFileDescription
            {
                SeparatorChar = '\t',
                FirstLineHasColumnNames = true
            };
        }

        public IEnumerable<FilteredKeyword> ReadFilterTerms(string filePath)
        {
            var fileContext = new CsvContext();
            var keywords = fileContext.Read<FilterTerm>(filePath, fileDescription);
            return keywords
                .Select(x => new FilteredKeyword { Keyword = x.Term, CategoryBit = 0, Category = "0" })
                .ToList();
        }
    }
}
