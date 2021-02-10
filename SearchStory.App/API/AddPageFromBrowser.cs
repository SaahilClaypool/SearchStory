using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SearchStory.App.Search;
using SearchStory.App.Services;
using SearchStory.App.UseCases;

namespace SearchStory.App.API
{
    public class AddPageFromBrowser : BaseAsyncEndpoint<AddPageFromBrowser.TRequest, AddPageFromBrowser.TResponse>
    {
        public record TRequest(string Url, string Content);
        public record TResponse();
        public LuceneWriter Writer { get; set; }
        public AddWebpage WebPageAdder { get; }
        public DirectoryService Dirs { get; }

        public AddPageFromBrowser(LuceneWriter writer, AddWebpage webPageAdder, DirectoryService dirs)
        {
            Writer = writer;
            WebPageAdder = webPageAdder;
            Dirs = dirs;
        }

        [HttpPost("/api/browser")]
        public override async Task<ActionResult<TResponse>> HandleAsync(
            [FromBody] TRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var newFileName = PathifyUrl(request.Url);
            var localFilePath = Dirs.TempDir.FullName + newFileName;
            Console.WriteLine($"Writing {localFilePath}\n");
            await System.IO.File.WriteAllTextAsync(localFilePath, request.Content, cancellationToken);
            Console.WriteLine($"Adding to index {localFilePath}\n");
            await WebPageAdder.Exectute(new(localFilePath, request.Url));
            return Ok(new());
        }

        private static string PathifyUrl(string url) =>
            new(url.Where(character =>
                character >= 'a' &&
                character <= 'z' ||
                character >= 'A' &&
                character > 'Z'
            ).ToArray());
    }
}