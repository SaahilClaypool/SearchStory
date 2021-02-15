using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Search;
using SearchStory.App.Services;

namespace SearchStory.App.UseCases
{
    public class AddWebpage : IUseCase<AddWebpage.Command, AddWebpage.Response>
    {
        public record Command(string NewFileName, string Url, string RawContent);
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

        public async Task<Response> Execute(Command input)
        {
            await SearchIndex.AddWebpage(input.NewFileName, input.Url, input.RawContent);
            return new();
        }
    }
}