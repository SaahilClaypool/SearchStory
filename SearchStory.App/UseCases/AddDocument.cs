using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Search;
using SearchStory.App.Services;

namespace SearchStory.App.UseCases
{
    public class AddDocument : IUseCase<AddDocument.Command, AddDocument.Response>
    {
        public record Command(string NewFileName);
        public record Response();
        public ILogger<AddDocument> Logger { get; }
        public DirectoryService DirService { get; }
        public LuceneWriter SearchIndex { get; }

        public AddDocument(ILogger<AddDocument> logger, DirectoryService configuration, LuceneWriter searchIndex)
        {
            Logger = logger;
            DirService = configuration;
            SearchIndex = searchIndex;
        }

        /// <summary>
        /// TODO: add to index
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response> Exectute(Command input)
        {
            var file = new FileInfo(DirService.DocumentDir.FullName + Path.GetFileName(input.NewFileName));
            Logger.LogInformation($"Moving {input.NewFileName} to {file}");
            File.Move(input.NewFileName, file.ToString(), overwrite: true);
            await SearchIndex.AddFile(file);
            return new();
        }
    }
}