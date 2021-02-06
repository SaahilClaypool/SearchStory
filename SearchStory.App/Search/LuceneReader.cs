using System.IO;
using System.Threading.Tasks;
using Lucene.Net.Store;
using SearchStory.App.Services;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using SearchStory.App.Search.Transformers;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Analysis;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

namespace SearchStory.App.Search
{
    public class LuceneReader
    {
        private const int PAGE_SIZE = 10;
        public DirectoryService DirectoryService { get; }
        public LuceneWriter Writer { get; }
        public ILogger<LuceneReader> Logger { get; }

        public LuceneReader(DirectoryService directoryService, LuceneWriter writer, ILogger<LuceneReader> logger)
        {
            DirectoryService = directoryService;
            Writer = writer;
            Logger = logger;
        }

        public record SearchResult(string Filename, IEnumerable<string> HighlightedSnippets);

        /// <summary>
        /// TODO: add search order parameter
        /// https://github.com/Mpdreamz/lucene.net/blob/master/C%23/contrib/Highlighter.Net/Test/HighlighterTest.cs
        /// </summary>
        /// <param name="term">query to search lucene index for</param>
        public IEnumerable<SearchResult> Search(string term)
        {
            if (term == "") yield break;
            // using var reader = Writer.IndexWriter.GetReader(applyAllDeletes: false);
            using var reader = DirectoryReader.Open(FSDirectory.Open(DirectoryService.IndexDir));
            var searcher = new IndexSearcher(reader);

            Logger.LogInformation($"number of docs: {reader.NumDocs}");

            var query = Parser.Parse(term);
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            System.Console.WriteLine($"Term is {term}");


            // todo make search after for proper paging
            var hits = searcher.Search(query, PAGE_SIZE);
            // Highlighter highlighter = new(new QueryScorer(query));
            Highlighter highlighter = new(new SimpleHTMLFormatter("<B class='highlight'>", "</B>"), new QueryScorer(query));
            highlighter.TextFragmenter = new SimpleFragmenter(120);
            for (int i = 0; i < hits.ScoreDocs.Length; i++)
            {
                var doc = reader.Document(hits.ScoreDocs[i].Doc);
                int maxNumFragmentsRequired = 10;
                // TokenStream tokenStream = analyzer.TokenStream(FIELD_NAME, new System.IO.StringReader(text));
                TokenStream tokenStream = analyzer.GetTokenStream(LuceneDocument.CONTENTS, doc.Get(LuceneDocument.CONTENTS));
                var fragmentSeparator = "...";
                var result = highlighter.GetBestFragments(tokenStream, doc.Get(LuceneDocument.CONTENTS), maxNumFragmentsRequired, fragmentSeparator);
                yield return new(doc.Get(LuceneDocument.PATH), result.Split(fragmentSeparator));
            }
        }

        private QueryParser _Parser;
        private QueryParser Parser
        {
            get
            {
                if (_Parser is null)
                {
                    _Parser = new QueryParser(LuceneVersion.LUCENE_48, LuceneDocument.CONTENTS, new StandardAnalyzer(LuceneVersion.LUCENE_48));
                }
                return _Parser;
            }
        }
    }
}