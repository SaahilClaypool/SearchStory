using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lucene.Net.Documents;

namespace SearchStory.App.Search.Transformers
{
    /// <summary>
    /// Return the file without changing contents.
    /// Useful if the file is already text-like.
    /// Files like PDF will need their contents cleaned.
    /// </summary>
    public class TextTransformer : ITransformer
    {
        public (string Key, Document Document, IEnumerable<IDisposable> Disposables) Transform(FileInfo file, string? username)
        {
            FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read);
            var doc = LuceneDocument.New(path: file.FullName, contents: new StreamReader(fs, Encoding.UTF8).ReadToEnd(), username: username);
            return (file.FullName, doc, new List<IDisposable> { fs });
        }
    }
}