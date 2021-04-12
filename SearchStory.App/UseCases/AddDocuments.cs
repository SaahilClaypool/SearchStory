using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Search;
using SearchStory.App.Services;

namespace SearchStory.App.UseCases
{
    public class AddDocuments
    {
        public record Command(List<string> NewFileNames, bool Flush = true);
        public record Response();
        public ILogger<AddDocuments> Logger { get; }
        public DirectoryService DirService { get; }
        public LuceneWriter SearchIndex { get; }

        public AddDocuments(ILogger<AddDocuments> logger, DirectoryService configuration, LuceneWriter searchIndex)
        {
            Logger = logger;
            DirService = configuration;
            SearchIndex = searchIndex;
        }

        public async IAsyncEnumerable<string> Execute(Command input)
        {
            foreach (var newFileName in input.NewFileNames)
            {
                var file = new FileInfo(DirService.DocumentDir.FullName + Path.GetFileName(newFileName));
                Logger.LogInformation($"Moving {newFileName} to {file}");
                File.Move(newFileName, file.ToString(), overwrite: true);
                await SearchIndex.AddFile(file, null, false);
                yield return newFileName;
            }
            SearchIndex.Flush();
        }
    }
}