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
    public class DefaultTransformer : ITransformer
    {
        public (string Key, Document Document, IEnumerable<IDisposable> Disposables) Transform(FileInfo file)
        {
            FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read);
            // I wonder if I should make a document type to encapsulate this...
            var doc = new Document()
            {
                new StringField(LuceneDocument.PATH, file.FullName, Field.Store.YES),
                new TextField(LuceneDocument.CONTENTS, new StreamReader(fs, Encoding.UTF8).ReadToEnd(), Field.Store.YES),
                new Int64Field(LuceneDocument.MODIFIED, file.LastWriteTimeUtc.Ticks, Field.Store.NO)
            };

            return (file.FullName, doc, new List<IDisposable> { fs });
        }
    }
}