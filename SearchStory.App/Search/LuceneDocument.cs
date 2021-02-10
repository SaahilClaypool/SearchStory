using System;
using Lucene.Net.Documents;
namespace SearchStory.App.Search
{
    public static class LuceneDocument
    {
        public const string CONTENTS = "contents";
        public const string MODIFIED = "modified";
        public const string PATH = "path";

        public static Document New(
            string path,
            string contents
        ) =>
            new()
            {
                new StringField(PATH, path, Field.Store.YES),
                new TextField(CONTENTS, contents, Field.Store.YES),
                new Int64Field(MODIFIED, DateTime.UtcNow.Ticks, Field.Store.NO)
            };
    }
}