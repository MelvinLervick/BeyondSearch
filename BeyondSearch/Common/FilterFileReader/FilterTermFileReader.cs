using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<string> ReadFilterTerms(string filePath)
        {
            var fileContext = new CsvContext();
            var keywords = fileContext.Read<FilterTerm>(filePath, fileDescription);
            return keywords
                .Select(x => x.Term)
                .ToList();
        }
    }
}
