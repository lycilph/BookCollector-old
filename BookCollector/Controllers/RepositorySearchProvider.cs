using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using BookCollector.Model;
using BookCollector.Services;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using NLog;
using Version = Lucene.Net.Util.Version;

namespace BookCollector.Controllers
{
    [Export(typeof(RepositorySearchProvider))]
    public class RepositorySearchProvider
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string directoryname = "lucene";

        private readonly ApplicationSettings application_settings;
        private readonly string[] fields = { "ID", "Title", "ASIN", "ISBN10", "ISBN13", "Description", "Source", "Author", "Narrator" };
        private readonly Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
        private readonly QueryParser query_parser;
        private FSDirectory directory;
        private IndexSearcher index_searcher;

        [ImportingConstructor]
        public RepositorySearchProvider(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;

            query_parser = new MultiFieldQueryParser(Version.LUCENE_30, fields, analyzer);
            query_parser.DefaultOperator = QueryParser.Operator.OR;
        }

        public void Open(CollectionDescription collection)
        {
            var base_dir = Path.Combine(application_settings.DataDir, collection.Id, directoryname);
            logger.Trace("Opening (dir = {0})", base_dir);
            System.IO.Directory.CreateDirectory(base_dir);
            directory = FSDirectory.Open(base_dir);

            if (!IndexReader.IndexExists(directory))
                Clear();
            
            index_searcher = new IndexSearcher(directory);
        }

        public void Close()
        {
            logger.Trace("Closing");
            index_searcher.Dispose();
            directory.Dispose();
        }

        public void Clear()
        {
            using (var index_writer = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
            {
                index_writer.DeleteAll();
                index_writer.Commit();
            }
        }

        public List<string> Search(string query)
        {
            try
            {
                var parsed_query = query_parser.Parse(query);
                var hits = index_searcher.Search(parsed_query, 100);
                return hits.ScoreDocs.Select(hit =>
                {
                    var doc = index_searcher.Doc(hit.Doc);
                    return doc.GetField("ID").StringValue;
                }).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public void Add(IEnumerable<Book> books)
        {
            using (var index_writer = new IndexWriter(directory, analyzer, false, new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
            {
                foreach (var book in books)
                {
                    var doc = new Document();

                    doc.Add(new Field("ID", book.Id, Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("Source", book.ImportSource, Field.Store.NO, Field.Index.ANALYZED));

                    if (!string.IsNullOrWhiteSpace(book.Asin))
                        doc.Add(new Field("ASIN", book.Asin, Field.Store.NO, Field.Index.ANALYZED));
                    if (!string.IsNullOrWhiteSpace(book.ISBN10))
                        doc.Add(new Field("ISBN10", book.ISBN10, Field.Store.NO, Field.Index.ANALYZED));
                    if (!string.IsNullOrWhiteSpace(book.ISBN13))
                        doc.Add(new Field("ISBN13", book.ISBN13, Field.Store.NO, Field.Index.ANALYZED));
                    if (!string.IsNullOrWhiteSpace(book.Description))
                        doc.Add(new Field("Description", book.Description, Field.Store.NO, Field.Index.ANALYZED));

                    if (book.Authors != null && book.Authors.Any())
                    {
                        foreach (var author in book.Authors)
                            doc.Add(new Field("Author", author, Field.Store.NO, Field.Index.ANALYZED));
                    }
                    if (book.Narrators != null && book.Narrators.Any())
                    {
                        foreach (var narrator in book.Narrators)
                            doc.Add(new Field("Narrator", narrator, Field.Store.NO, Field.Index.ANALYZED));
                    }

                    index_writer.AddDocument(doc);
                }

                index_writer.Optimize();
                index_writer.Commit();
            }
        }
    }
}
