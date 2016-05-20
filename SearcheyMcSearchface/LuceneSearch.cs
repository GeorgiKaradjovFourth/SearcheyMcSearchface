using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using LuceneDocument = Lucene.Net.Documents.Document;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using Lucene.Net.Store;
using SearcheyDocument = SearcheyData.Entities.Document;
using LuceneVersion = Lucene.Net.Util.Version;

namespace SearcheyMcSearchface
{
    public static class LuceneSearch
    {
        private static string _luceneDir =
                Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        private static void _addToLuceneIndex(SearcheyDocument data, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", data.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new LuceneDocument();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", data.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Header", data.Header, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Text", data.Text, Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }

        public static void AddUpdateLuceneIndex(IEnumerable<SearcheyDocument> documents)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var document in documents)
                    _addToLuceneIndex(document, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public static void AddUpdateLuceneIndex(SearcheyDocument document)
        {
            AddUpdateLuceneIndex(new List<SearcheyDocument> { document });
        }

        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private static SearcheyDocument _mapLuceneDocumentToData(LuceneDocument doc)
        {
            return new SearcheyDocument
            {
                Id = Convert.ToInt32(doc.Get("Id")),
                Header = doc.Get("Header"),
                Text = doc.Get("Text")
            };
        }

        private static IEnumerable<SearcheyDocument> _mapLuceneToDataList(IEnumerable<LuceneDocument> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<SearcheyDocument> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        private static IEnumerable<SearcheyDocument> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<SearcheyDocument>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(LuceneVersion.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else {
                    var parser = new MultiFieldQueryParser
                        (LuceneVersion.LUCENE_30, new[] { "Id", "Header", "Text" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }

        public static IEnumerable<SearcheyDocument> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<SearcheyDocument>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }

        public static IEnumerable<SearcheyDocument> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<SearcheyDocument>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }

        public static IEnumerable<SearcheyDocument> MoreLikeThis(int documentId, int count)
        {
            var reader = IndexReader.Open(_directory, false);
            var indexSearcher = new IndexSearcher(_directory, false);
            MoreLikeThis mlt = new MoreLikeThis(reader); // Pass the index reader
            mlt.SetFieldNames(new[] { "Id", "Header", "Text" });
            var query = mlt.Like(documentId - 306/* Its hackday after all */);
            var hits = indexSearcher.Search(query, null, count, Sort.RELEVANCE).ScoreDocs;
            var result = _mapLuceneToDataList(hits, indexSearcher);
            return result;
        }

        public static List<Tuple<Term, double>> Terms()
        {
            var reader = IndexReader.Open(_directory, false);
            int docnum = reader.NumDocs();
            var tfIdfs = new List<Tuple<Term, double>>();

            var terms = reader.Terms().ToList();
            foreach (var term in terms)
            {
                String termText = term.Text;
                float docFrequency = reader.DocFreq(term);
                var termDocs = reader.TermDocs(term).ToList();
                float totalFrequency = 0;
                if (termDocs != null)
                {
                    totalFrequency += termDocs.Sum(s => s.Freq);
                }

                double idf = Math.Log(docnum/docFrequency);
                double tfIdf = totalFrequency * idf;

                tfIdfs.Add(new Tuple<Term, double>(term, tfIdf));
            }
            reader.Close();
            tfIdfs = tfIdfs.OrderByDescending(s => s.Item2).ToList();
            return tfIdfs;
        }

        public static void AssosiationWithoutDistance()
        {
            var reader = IndexReader.Open(_directory, false);
            int docnum = reader.NumDocs();
            var terms = Terms().ToList().Take(200).Select(s => s.Item1);
            var result = new List<Tuple<string, string>>();
            foreach (var term1 in terms)
            {
                var otherTerms = terms.Where(s => !s.Equals(term1));
                var term1Docs = reader.TermDocs(term1).ToList();
                foreach (var term2 in otherTerms)
                {
                    var term2Docs = reader.TermDocs(term2).ToList();
                    var colocatingDocuments = term1Docs.Intersect(term2Docs);
                    //result.Add(new Tuple<term1., string>());

                }



            }

        }

        private static List<Term> ToList(this TermEnum input)
        {
            var list = new List<Term>();
            while (input.Next())
            {
                list.Add(input.Term);
            }
            return list;
        }

        private static List<TermDocs> ToList(this TermDocs input)
        {
            var list = new List<TermDocs>();
            while (input.Next())
            {
                list.Add(input);
            }
            return list;
        }
    }
}