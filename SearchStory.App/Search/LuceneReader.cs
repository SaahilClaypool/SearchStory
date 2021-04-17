using System.IO;
using Lucene.Net.Store;
using SearchStory.App.Services;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System;
using Lucene.Net.Analysis.En;
using Lucene.Net.QueryParsers.ComplexPhrase;

namespace SearchStory.App.Search
{
    public class LuceneReader
    {
        private const int NUMBER_OF_FRAGMENTS = 15;
        private const int FRAGMENT_SIZE = 180;

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
        public IEnumerable<SearchResult> Search(string term, string? username)
        {
            if (term == "") return new List<SearchResult>();
            using var reader = DirectoryReader.Open(FSDirectory.Open(DirectoryService.IndexDir));
            var searcher = new IndexSearcher(reader);

            try
            {
                return DoQuery(reader, searcher, term, username).ToList();
            }
            catch(Exception e)
            {
                Logger.LogError($"Error: {e.Message}");
                return new List<SearchResult>(); 
            }
        }

        private IEnumerable<SearchResult> DoQuery(IndexReader reader, IndexSearcher searcher, string term, string? username)
        {
            Parser.PhraseSlop = 0;
            var query = Parser.Parse(term);
            Parser.PhraseSlop = 5;
            var sloppyQuery = Parser.Parse(term);
            Parser.PhraseSlop = 0;

            var multiQuery = new BooleanQuery
            {
                { query, Occur.SHOULD },
                { sloppyQuery, Occur.SHOULD }
            };
            Console.WriteLine($"Term is {term} - {username}");
            if (username is not null)
            {
                multiQuery.Add(new TermQuery(new Term(LuceneDocument.USERNAME, username)), Occur.MUST);
            }


            // todo make search after for proper paging
            var hits = searcher.Search(multiQuery, NUMBER_OF_FRAGMENTS);
            Highlighter highlighter = new(new SimpleHTMLFormatter("<B class='highlight'>", "</B>"), new QueryScorer(query));
            highlighter.TextFragmenter = new SimpleFragmenter(FRAGMENT_SIZE);
            for (int i = 0; i < hits.ScoreDocs.Length; i++)
            {
                var doc = reader.Document(hits.ScoreDocs[i].Doc);
                int maxFragments = NUMBER_OF_FRAGMENTS;
                var fragments = highlighter.GetBestFragments(
                        Parser.Analyzer,
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

        private ComplexPhraseQueryParser? _Parser;
        private ComplexPhraseQueryParser Parser
        {
            get
            {
                if (_Parser is null)
                {
                    _Parser = new ComplexPhraseQueryParser(LuceneVersion.LUCENE_48, LuceneDocument.CONTENTS, new EnglishAnalyzer(LuceneVersion.LUCENE_48));
                }
                return _Parser;
            }
        }
    }
}