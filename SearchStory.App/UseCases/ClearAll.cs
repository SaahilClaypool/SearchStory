using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Services;
using System.Linq;
using System.Collections.Generic;
using SearchStory.App.Search;

namespace SearchStory.App.UseCases
{
    public class ClearAll : IUseCase<ClearAll.Command, ClearAll.Response>
    {
        public record Command();
        public record Response();
        public ILogger<ClearAll> Logger { get; }
        public DirectoryService DirService { get; }
        public LuceneWriter Writer { get; }

        public ClearAll(ILogger<ClearAll> logger, DirectoryService configuration, LuceneWriter writer)
        {
            Logger = logger;
            DirService = configuration;
            Writer = writer;
        }

        public async Task<Response> Execute(Command input)
        {
            Logger.LogWarning("Clearing all!");
            var dir = DirService.DocumentDir;
            foreach (var file in dir.GetFiles())
            {
                await Writer.RemoveFile(file);
                file.Delete();
            }
            await Writer.RemoveAll();
            Writer.Flush();

            return new();
        }
    }
}