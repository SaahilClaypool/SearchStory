using Microsoft.Extensions.Logging;
using SearchStory.App.Services;

namespace SearchStory.App.UseCases
{
    public class AddDocument : IUseCase<AddDocument.Command, AddDocument.Response>
    {
        public record Command();
        public record Response();
        public ILogger<AddDocument> Logger { get; }
        public Configuration Configuration { get; }

        public AddDocument(ILogger<AddDocument> logger, Configuration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }
        public Response Exectute(Command input)
        {
            Logger.LogInformation($"Adding to {Configuration.IndexDirectory}");
            return new();
        }
    }
}