using System.Collections.Generic;
using System.IO;
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
            var terms = new List<FilteredKeyword>();
            if (File.Exists(filePath))
            {
                using (var sr = File.OpenText(filePath))
                {
                    var s = string.Empty;
                    var first = true;
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (!first)
                        {
                            //var fields = s.Split('\t')
                            terms.Add(new FilteredKeyword { Keyword = s, CategoryBit = 0, Category = "0" });
                        }
                        first = false;
                    }
                }
            }
            return terms;

            //var fileContext = new CsvContext();
            //var keywords = fileContext.Read<FilterTerm>(filePath, fileDescription);
            //return keywords
            //    .Select(x => new FilteredKeyword { Keyword = x.Term, CategoryBit = 0, Category = "0" })
            //    .ToList();
        }
    }
}
