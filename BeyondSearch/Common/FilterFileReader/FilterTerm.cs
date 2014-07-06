using LINQtoCSV;

namespace BeyondSearch.Common.FilterFileReader
{
    public class FilterTerm
    {
        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public string Term { get; set; }
    }
}
