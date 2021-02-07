using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lucene.Net.Documents;

namespace SearchStory.App.Search.Transformers
{
    /// <summary>
    /// Return a document by parsing text content form PDF to index
    /// </summary>
    public class PDFTransformer : ITransformer
    {
        public (string Key, Document Document, IEnumerable<IDisposable> Disposables) Transform(FileInfo file)
        {
            return new DefaultTransformer().Transform(file);
        }
    }
}