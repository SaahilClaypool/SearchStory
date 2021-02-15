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
using System.Text.Json;
using System;

namespace SearchStory.App.Search
{
    public class LuceneReader
    {
        private const int PAGE_SIZE = 10;
        private const int FragmentSize = 80;

        public DirectoryService DirectoryService { get; }
        public LuceneWriter Writer { get; }
        public ILogger<LuceneReader> Logger { get; }

        public LuceneReader(DirectoryService directoryService, LuceneWriter writer, ILogger<LuceneReader> logger)
        {
            DirectoryService = directoryService;
            Writer = writer;
            Logger = logger;
        }

        public record SearchResult(string Filename, IEnumerable<string> HighlightedSnippets, string? OriginalURL, DateTime LastWrite)
        {
            public string BaseName =>
                Path.GetFileNameWithoutExtension(Filename)
                    .Replace("_", " ");
            public string BasePath => Path.GetFileName(Filename);
            public bool HasOriginal => OriginalURL is not null;
        };

        /// <summary>
        /// TODO: add search order parameter
        /// https://github.com/Mpdreamz/lucene.net/blob/master/C%23/contrib/Highlighter.Net/Test/HighlighterTest.cs
        /// </summary>
        /// <param name="term">query to search lucene index for</param>
        public IEnumerable<SearchResult> Search(string term)
        {
            if (term == "") return new List<SearchResult>();
            // using var reader = Writer.IndexWriter.GetReader(applyAllDeletes: false);
            using var reader = DirectoryReader.Open(FSDirectory.Open(DirectoryService.IndexDir));
            var searcher = new IndexSearcher(reader);

            try
            {
                return DoQuery(reader, searcher, term).ToList();
            }
            catch
            {
                return new List<SearchResult>();
            }
        }

        private IEnumerable<SearchResult> DoQuery(IndexReader reader, IndexSearcher searcher, string term)
        {
            var query = Parser.Parse(term);
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            System.Console.WriteLine($"Term is {term}");


            // todo make search after for proper paging
            var hits = searcher.Search(query, PAGE_SIZE);
            Highlighter highlighter = new(new SimpleHTMLFormatter("<B class='highlight'>", "</B>"), new QueryScorer(query));
            highlighter.TextFragmenter = new SimpleFragmenter(FragmentSize);
            for (int i = 0; i < hits.ScoreDocs.Length; i++)
            {
                var doc = reader.Document(hits.ScoreDocs[i].Doc);
                int maxFragments = 10;
                var fragments = highlighter.GetBestFragments(
                        analyzer,
                        LuceneDocument.CONTENTS,
                        doc.Get(LuceneDocument.CONTENTS),
                        maxFragments
                    );
                var url = doc.Get(LuceneDocument.URL);
                var path = doc.Get(LuceneDocument.PATH);
                var modified = doc.Get(LuceneDocument.MODIFIED);
                yield return new(path, fragments, url, new DateTime(long.Parse(modified)));
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