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

        /// <summary>
        /// TODO: add to index
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response> Exectute(Command input)
        {
            var dir = DirService.DocumentDir;
            foreach (var file in dir.GetFiles())
            {
                using var writer = Writer.RemoveFile(file);
                file.Delete();
            }

            return new();
        }
    }
}