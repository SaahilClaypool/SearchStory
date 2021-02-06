using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;

namespace SearchStory.App.Search.Transformers
{
    public interface ITransformer
    {
        (string Key, Document Document, IEnumerable<IDisposable> Disposables) Transform(FileInfo file);
    }
}