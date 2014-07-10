using LINQtoCSV;

namespace BeyondSearch.Common.CategorizedFilterReader
{
    public class CategorizedFilterTerm
    {
        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public string Term { get; set; }
        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public byte CategoryBit { get; set; }
        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public string Category { get; set; }
    }
}
