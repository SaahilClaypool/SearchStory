using System.IO;
using System.Threading.Tasks;
using Lucene.Net.Store;
using SearchStory.App.Services;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using SearchStory.App.Search.Transformers;
using Microsoft.Extensions.Logging;
using System;

namespace SearchStory.App.Search
{
    public class LuceneWriter
    {
        public DirectoryService DirectoryService { get; }
        public ILogger<LuceneWriter> Logger { get; }

        public LuceneWriter(DirectoryService directoryService, ILogger<LuceneWriter> logger)
        {
            DirectoryService = directoryService;
            Logger = logger;
        }

        /// <summary>
        /// Runs in a task to avoid blocking.
        /// Transforms the file into a parsable format and saves to index.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task AddFile(FileInfo file)
        {
            return Task.Run(() =>
            {
                using var writer = GetIndexWriter();
                var (key, doc, disposables) = new Transformer().Transform(file);
                writer.UpdateDocument(new Term(LuceneDocument.PATH, doc.Get(LuceneDocument.PATH)), doc);
                foreach (var d in disposables)
                {
                    d.Dispose();
                }
                writer.Flush(true, true);
                Logger.LogInformation($"Flushed index to {writer.Directory}");
            });
        }

        public Task AddWebpage(string localFilename, string url, string textContent)
        {
            return Task.Run(() =>
            {
                using var writer = GetIndexWriter();
                var localFile = new FileInfo(localFilename);
                var doc = LuceneDocument.Web(localFilename, textContent, url);
                writer.UpdateDocument(new Term(LuceneDocument.PATH, doc.Get(LuceneDocument.PATH)), doc);
                writer.Flush(true, true);
                Logger.LogInformation($"Flushed index to {writer.Directory}");
            });
        }

        public async Task RemoveFile(FileInfo file)
        {
            using var writer = GetIndexWriter();
            writer.DeleteDocuments(new Term(LuceneDocument.PATH, file.FullName));
        }

        public async Task RemoveAll()
        {
            using var writer = GetIndexWriter();
            writer.DeleteAll();
        }

        public IndexWriter GetIndexWriter()
        {
            var dir = FSDirectory.Open(DirectoryService.IndexDir);
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND
            };
            return new(dir, iwc);
        }
    }
}