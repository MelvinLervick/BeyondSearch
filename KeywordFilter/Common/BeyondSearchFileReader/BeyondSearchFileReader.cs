using System;
using System.Collections.Generic;
using System.IO;
using KeywordFilter.Filters;

namespace KeywordFilter.Common.BeyondSearchFileReader
{
    public class BeyondSearchFileReader : IBeyondSearchFileReader
    {
        private readonly bool firstLineHasColumnNames;
        private readonly char separatorChar;

        public BeyondSearchFileReader()
        {
            separatorChar = '\t';
            firstLineHasColumnNames = true;
        }

        public IEnumerable<FilteredKeyword> ReadTerms( string filePath, RecordFormat recordFormat )
        {
            if (File.Exists(filePath))
            {
                using (var sr = File.OpenText(filePath))
                {
                    switch (recordFormat)
                    {
                        case RecordFormat.TermOnly:
                            return ReadTermsOnly( sr );
                        case RecordFormat.CategorizedTerm:
                            return ReadCategorizedTerms(sr);
                        case RecordFormat.Tsv:
                            return ReadTsvTerms(sr);
                    }
                }
            }

            return new List<FilteredKeyword>();
        }

        private IEnumerable<FilteredKeyword> ReadCategorizedTerms(StreamReader sr)
        {
            var terms = new List<FilteredKeyword>();

            string s;
            var first = firstLineHasColumnNames;
            while ( ( s = sr.ReadLine() ) != null )
            {
                if ( !first )
                {
                    var fields = s.Split( separatorChar );
                    byte bit;
                    Byte.TryParse( fields[1], out bit );
                    terms.Add( new FilteredKeyword {Keyword = fields[0], CategoryBit = bit, Category = fields[2]} );
                }
                first = false;
            }

            return terms;
        }

        private IEnumerable<FilteredKeyword> ReadTsvTerms(StreamReader sr)
        {
            var terms = new List<FilteredKeyword>();

            string s;
            var first = firstLineHasColumnNames;
            while ((s = sr.ReadLine()) != null)
            {
                if (!first)
                {
                    var fields = s.Split(separatorChar);
                    int level;
                    Int32.TryParse(fields[2], out level);
                    if ( level == 2 || level == 3 )
                    {
                        terms.Add( new FilteredKeyword {Keyword = fields[1], CategoryBit = 64, Category = "7"} );
                    }
                }
                first = false;
            }

            return terms;
        }

        private IEnumerable<FilteredKeyword> ReadTermsOnly(StreamReader sr)
        {
            var terms = new List<FilteredKeyword>();

            string s;
            var first = firstLineHasColumnNames;
            while ( ( s = sr.ReadLine() ) != null )
            {
                if ( !first )
                {
                    var fields = s.Split( separatorChar );
                    terms.Add( new FilteredKeyword {Keyword = fields[0], CategoryBit = 0, Category = "0"} );
                }
                first = false;
            }

            return terms;
        }
    }
}
