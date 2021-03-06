using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchStory.App.Search;
using SearchStory.App.Services;
using SearchStory.App.UseCases;

namespace SearchStory.App.API
{
    [EnableCors("LocalBrowser")]
    public class AddPageFromBrowser : ControllerBase
    {
        public record TRequest(string Url, string Content, string Title);
        public record TResponse();
        public LuceneWriter Writer { get; set; }
        public AddWebpage WebPageAdder { get; }
        public DirectoryService Dirs { get; }
        public ILogger<AddPageFromBrowser> Logger { get; }
        public LoginService Login { get; }

        public AddPageFromBrowser(LuceneWriter writer, AddWebpage webPageAdder, DirectoryService dirs, ILogger<AddPageFromBrowser> logger, LoginService login)
        {
            Writer = writer;
            WebPageAdder = webPageAdder;
            Dirs = dirs;
            Logger = logger;
            Login = login;
        }

        [HttpPost("/api/browser")]
        public async Task<ActionResult<TResponse>> HandleAsync(
            [FromBody] TRequest request,
            CancellationToken cancellationToken = default
        )
        {
            if (await Login.RequiresPassword())
            {
                Logger.LogInformation($"Password required");
                return BadRequest("Password required");
            }
            Logger.LogInformation($"Browser has {request.Title}");
            var newFileName = PathifyUrl(request.Title);
            Logger.LogInformation($"turned {request.Title} into {newFileName}");
            Logger.LogInformation($"new file name {newFileName}");
            var localFilePath = Dirs.DocumentDir.FullName + newFileName + ".html";
            Logger.LogInformation($"local file {localFilePath}");
            await System.IO.File.WriteAllTextAsync(localFilePath, request.Content, cancellationToken);
            var rawText = TextifyHtml(request.Content);
            // TODO: username here
            await WebPageAdder.Execute(new(localFilePath, request.Url, rawText, null));
            return Ok(new());
        }

        private static string TextifyHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var chunks = doc
                .DocumentNode
                .DescendantsAndSelf()
                .Where(item => item.NodeType == HtmlNodeType.Text)
                .Where(item => item.InnerText.Trim() != "")
                .Select(item => item.InnerText.Trim())
                .ToList();
            return string.Join(" ", chunks);
        }

        private static string PathifyUrl(string url) =>
            new(url
                .Replace("/", "_")
                .Replace(" ", "_")
                .Where(character =>
                    character == '_' ||
                    character == '.' ||
                    (character >= 'a' && character <= 'z') ||
                    (character >= 'A' && character <= 'Z')
                ).ToArray());
    }
}