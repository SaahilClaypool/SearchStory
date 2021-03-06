
using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;

namespace SearchStory.App.Search.Transformers
{
    public class Transformer : ITransformer
    {
        public (string Key, Document Document, IEnumerable<IDisposable> Disposables) Transform(FileInfo file, string? username)
            => file.Extension.ToLower() switch
            {
                ".pdf" => new PDFTransformer().Transform(file, username),
                _ => new TextTransformer().Transform(file, username)
            };
    }
}