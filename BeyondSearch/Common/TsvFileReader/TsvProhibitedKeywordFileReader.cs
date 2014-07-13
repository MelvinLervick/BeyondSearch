using System.Collections.Generic;
using System.Linq;
using BeyondSearch.Filters;
using LINQtoCSV;

namespace BeyondSearch.Common.TsvFileReader
{
    public class TsvProhibitedKeywordFileReader : IProhibitedKeywordFileReader
    {
        private readonly CsvFileDescription fileDescription;

        public TsvProhibitedKeywordFileReader()
        {
            fileDescription = new CsvFileDescription
            {
                SeparatorChar = '\t',
                FirstLineHasColumnNames = true
            };
        }

        public IEnumerable<FilteredKeyword> ReadKeywords(string filePath)
        {
            var fileContext = new CsvContext();
            var keywords = fileContext.Read<TsvProhibitedKeyword>(filePath, fileDescription);
            return keywords
                .Where(x => x.Level == 3 || x.Level == 2)
                .Select(x => new FilteredKeyword {Keyword = x.Term, CategoryBit = 64, Category = "7"})
                .ToList();
        }
    }
}
