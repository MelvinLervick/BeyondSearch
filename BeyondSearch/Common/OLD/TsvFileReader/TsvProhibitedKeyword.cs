using LINQtoCSV;

namespace BeyondSearch.Common.TsvFileReader
{
    public class TsvProhibitedKeyword
    {
        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public int Id { get; set; }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string Term { get; set; }

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public int Level { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public int Sponsored { get; set; }

    }
}
