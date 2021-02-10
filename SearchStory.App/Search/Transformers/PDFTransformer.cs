using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Lucene.Net.Documents;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace SearchStory.App.Search.Transformers
{
    /// <summary>
    /// Return a document by parsing text content form PDF to index
    /// </summary>
    public class PDFTransformer : ITransformer
    {
        public (string Key, Document Document, IEnumerable<IDisposable> Disposables) Transform(FileInfo file)
        {
            using PdfDocument document = PdfDocument.Open(file.FullName);
            var words = document
                .GetPages()
                .SelectMany(page => page.GetWords())
                .Select(w => w.Text);
            var text = string.Join(" ", words);
            FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read);
            var doc = LuceneDocument.New(path: file.FullName, contents: text);
            return (file.FullName, doc, new List<IDisposable> { fs });
        }
    }
}