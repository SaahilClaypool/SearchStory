using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Services;
using System.Linq;
using System.Collections.Generic;

namespace SearchStory.App.UseCases
{
    public class ListDocuments : IUseCase<ListDocuments.Command, ListDocuments.Response>
    {
        public record Command(int Page = 0, int PageSize = 100);
        public record Response(IEnumerable<FileInfo> Files);
        public ILogger<ListDocuments> Logger { get; }
        public DirectoryService DirService { get; }

        public ListDocuments(ILogger<ListDocuments> logger, DirectoryService configuration)
        {
            Logger = logger;
            DirService = configuration;
        }

        /// <summary>
        /// TODO: add to index
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response> Execute(Command input)
        {
            var dir = DirService.DocumentDir;
            return new(dir.GetFiles()
                .OrderByDescending(f => f.CreationTime) // Most recent first
                .Skip(input.Page * input.PageSize).Take(input.PageSize));
        }
    }
}