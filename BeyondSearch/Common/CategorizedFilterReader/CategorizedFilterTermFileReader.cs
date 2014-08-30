using System.Collections.Generic;
using System.Linq;
using BeyondSearch.Filters;
using LINQtoCSV;

namespace BeyondSearch.Common.CategorizedFilterReader
{
    public class CategorizedFilterTermFileReader : ICategorizedFilterTermFileReader
    {
        private readonly CsvFileDescription fileDescription;

        public CategorizedFilterTermFileReader()
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
            var keywords = fileContext.Read<CategorizedFilterTerm>(filePath, fileDescription);
            return keywords
                .Select(x => new FilteredKeyword { Keyword = x.Term, CategoryBit = x.CategoryBit, Category = x.Category })
                .ToList();
        }
    }
}
