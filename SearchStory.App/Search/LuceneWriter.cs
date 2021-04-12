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
using Lucene.Net.Analysis.En;

namespace SearchStory.App.Search
{
    public class LuceneWriter : IDisposable
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
        public Task AddFile(FileInfo file, string? username, bool flush = true)
        {
            return Task.Run(() =>
            {
                var (key, doc, disposables) = new Transformer().Transform(file, username);
                Write(doc, flush);
                foreach (var d in disposables)
                {
                    d.Dispose();
                }
            });
        }

        public void Flush()
        {
            Logger.LogInformation("flushing\n");
            Writer.Flush(true, true);
            Writer.Dispose();
            Writer = null!; // it'll regenerate as needed
        }

        public Task AddWebpage(string localFilename, string url, string textContent, string? username)
        {
            return Task.Run(() =>
            {
                var localFile = new FileInfo(localFilename);
                var doc = LuceneDocument.Web(localFilename, textContent, url, username);
                Write(doc, true);
            });
        }

        public Task RemoveFile(FileInfo file)
        {
            Writer.DeleteDocuments(new Term(LuceneDocument.PATH, file.FullName));
            return Task.CompletedTask;
        }

        public Task RemoveAll()
        {
            Writer.DeleteAll();
            return Task.CompletedTask;
        }

        IndexWriter? _writer = null;
        IndexWriter Writer
        {
            get
            {
                if (_writer is null)
                {
                    _writer = GetIndexWriter();
                }
                return _writer;
            }
            set
            {
                _writer = value;
            }
        }
        public IndexWriter GetIndexWriter()
        {
            var dir = FSDirectory.Open(DirectoryService.IndexDir);
            var analyzer = new EnglishAnalyzer(LuceneVersion.LUCENE_48);
            var iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND
            };
            return new(dir, iwc);
        }

        private void Write(Lucene.Net.Documents.Document doc, bool flush)
        {
            Writer.UpdateDocument(new Term(LuceneDocument.PATH, doc.Get(LuceneDocument.PATH)), doc);
            if (flush)
            {
                Logger.LogInformation($"Flushed index to {Writer.Directory}");
                Writer.Flush(true, true);
            }
            else
            {
                Logger.LogInformation($"Added document without flushing");
            }
        }

        public void Dispose()
        {
            Logger.LogInformation("Disposing lucene writer");
            GC.SuppressFinalize(this);
            Flush();
        }
    }
}