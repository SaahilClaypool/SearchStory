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
                var transformer = file.Extension switch
                {
                    _ => new DefaultTransformer()
                };
                using var writer = GetIndexWriter();
                var (key, doc, disposables) = transformer.Transform(file);
                writer.AddDocument(doc);
                foreach (var d in disposables)
                {
                    d.Dispose();
                }
                writer.Flush(true, true);
                Logger.LogInformation($"Flushed index to {writer.Directory}");
            });
        }

        public Task RemoveFile(FileInfo file)
        {
            throw new System.NotImplementedException();
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