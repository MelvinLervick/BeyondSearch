using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeyondSearch.Common;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace BeyondSearch.Filters
{
    public class LucenePorterStemMatchmaker : IKeywordMatchmaker
    {
        private readonly IList<FilteredKeyword> filteredKeywords;
        //private readonly IDictionary<string, IEnumerable<PorterStemTokenizer<FilteredKeyword>>> filterMap;
        private readonly RAMDirectory filterIndex;

        public LucenePorterStemMatchmaker(IList<FilteredKeyword> filteredKeywords)
        {
            if (filteredKeywords == null)
            {
                throw new ArgumentNullException("filteredKeywords");
            }

            this.filteredKeywords = filteredKeywords;

            if (filterIndex == null)
            {
                filterIndex = new RAMDirectory();
                //var writer = new IndexWriter(filterIndex, new StandardAnalyzer(Version.LUCENE_29), IndexWriter.MaxFieldLength.UNLIMITED);
                var writer = new IndexWriter(filterIndex, new SimpleAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED);

                foreach ( var keyword in filteredKeywords )
                {
                    writer.AddDocument( CreateDocument( keyword.Keyword ) );
                }

                writer.Optimize();
                writer.Dispose();
            }
        }

        public IDictionary<string, FilteredKeyword> FilterKeywords(Dictionary<string, FilteredKeyword> suspects)
        {
            if (suspects == null)
            {
                return new Dictionary<string, FilteredKeyword>();
            }

            var matchedFilters = new Dictionary<string, FilteredKeyword>();
            foreach (var suspect in suspects)
            {
                if (matchedFilters.ContainsKey(suspect.Key)) continue;

                if (suspect.Value == null)
                {
                    Searcher searcher = new IndexSearcher(filterIndex);

                    //var suspectToCheck = new Dictionary<string, FilteredKeyword>
                    //{
                    //    {suspect.Key, null}
                    //};
                    //var matches =
                    //    suspectToCheck.Keys.Select(x => new PorterStemTokenizer<string>(x, y => y))
                    //        .Select(x => new {suspect = x.Source, filter = x.GetFirstMatch(filterMap)}).Distinct()
                    //        .ToDictionary(x => x.suspect, y => y.filter == null ? null : y.filter.Source);
                    var suspectTokens = suspect.Key.ToPorterStemNormalizedWithStopWords();
                    if ( Search( searcher, suspectTokens ) )
                    {
                        matchedFilters[suspect.Key] = new FilteredKeyword();
                    }
                    else
                    {
                        matchedFilters[suspect.Key] = suspect.Value;
                    }
                }
                else
                {
                    matchedFilters[suspect.Key] = suspect.Value;
                }
            }

            return matchedFilters;
        }

        private static Document CreateDocument(String keywords)
        {
            var doc = new Document();

            // Add the keywords as an indexed field. Note that indexed
            // Text fields are constructed using a Reader. Lucene can read
            // and index very large chunks of text, without storing the
            // entire content verbatim in the index. In this example we
            // can just wrap the content string in a StringReader."\"" + queryString + "\""
            var tokens = string.Format( "\"{0}\"", keywords.ToPorterStemNormalizedWithStopWords() );
            doc.Add(new Field("keywords", keywords, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("tokens", tokens, Field.Store.YES, Field.Index.ANALYZED));

            return doc;
        }

        private static bool Search(Searcher searcher, String queryString)
        {
            // Build a Query object
            //var qp = new QueryParser(Version.LUCENE_29, "keywords", new StandardAnalyzer(Version.LUCENE_29));
            //var qp = new QueryParser(Version.LUCENE_29, "tokens", new StandardAnalyzer(Version.LUCENE_29));
            var qp = new QueryParser(Version.LUCENE_29, "tokens", new SimpleAnalyzer());
            qp.FuzzyMinSim = 0.9F;
            var query = qp.Parse(queryString); //, "keywords", new StandardAnalyzer(Version.LUCENE_29));

            // Search for the query
            var hitCount = searcher.Search(query,null,1).TotalHits;

            if (hitCount == 0) {
                return false;
            }

            return true;
        }
    }
}
