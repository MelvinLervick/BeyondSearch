using System.Collections.Generic;
using System.Linq;
using BeyondSearch.Common.FilterFileReader;
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

        public IEnumerable<CategorizedFilterTerm> ReadFilterTerms(string filePath)
        {
            var fileContext = new CsvContext();
            var keywords = fileContext.Read<CategorizedFilterTerm>(filePath, fileDescription);
            return keywords
                .Select(x => x)
                .ToList();
        }
    }
}
