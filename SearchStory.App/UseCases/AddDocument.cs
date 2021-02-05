using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Services;

namespace SearchStory.App.UseCases
{
    public class AddDocument : IUseCase<AddDocument.Command, AddDocument.Response>
    {
        public record Command(string NewFileName);
        public record Response();
        public ILogger<AddDocument> Logger { get; }
        public DirectoryService DirService { get; }

        public AddDocument(ILogger<AddDocument> logger, DirectoryService configuration)
        {
            Logger = logger;
            DirService = configuration;
        }

        /// <summary>
        /// TODO: add to index
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response> Exectute(Command input)
        {
            var fileName = DirService.DocumentDir.FullName + Path.GetFileName(input.NewFileName);
            Logger.LogInformation($"Moving {input.NewFileName} to {fileName}");
            File.Copy(input.NewFileName, fileName, overwrite: true);
            return new();
        }
    }
}