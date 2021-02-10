using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Search;
using SearchStory.App.Services;

namespace SearchStory.App.UseCases
{
    public class AddWebpage : IUseCase<AddWebpage.Command, AddWebpage.Response>
    {
        public record Command(string NewFileName, string Url);
        public record Response();
        public ILogger<AddWebpage> Logger { get; }
        public DirectoryService DirService { get; }
        public LuceneWriter SearchIndex { get; }

        public AddWebpage(ILogger<AddWebpage> logger, DirectoryService configuration, LuceneWriter searchIndex)
        {
            Logger = logger;
            DirService = configuration;
            SearchIndex = searchIndex;
        }

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