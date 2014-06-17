using LINQtoCSV;

namespace BeyondSearch.Common.FilterFileReader
{
    public class FilterTerm
    {
        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string Term { get; set; }
    }
}
