using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SearchStory.App.Search;
using SearchStory.App.Services;
using SearchStory.App.Shared;

namespace SearchStory.App.UseCases
{
    public class IndexDirectoryUseCase
    {
        public record Command(string DirectoryName);
        public record Response();
        public ILogger<AddDocuments> Logger { get; }
        public DirectoryService DirService { get; }
        public LuceneWriter SearchIndex { get; }
        public DirectoryService Dirs { get; }

        public IndexDirectoryUseCase(ILogger<AddDocuments> logger, DirectoryService configuration, LuceneWriter searchIndex, DirectoryService dirs)
        {
            Logger = logger;
            DirService = configuration;
            SearchIndex = searchIndex;
            Dirs = dirs;
        }

        public async IAsyncEnumerable<float> Execute(Command command)
        {
            var dir = new DirectoryInfo(command.DirectoryName);
            var validExtensions = new HashSet<string>
            {
                ".pdf"
            };
            var validFiles = EnumerateFilesRecursive(dir).Where(f => validExtensions.Contains(f.Extension.ToLower()));
            var count = validFiles.Count();
            foreach (var (newFile, index) in validFiles.WithIndex())
            {
                var localFilePath = Dirs.TempDir.FullName + newFile.Name;
                Console.WriteLine($"Writing {localFilePath}\n");
                using var localFile = File.OpenWrite(localFilePath);
                using var remoteFile = File.OpenRead(newFile.FullName);
                await remoteFile.CopyToAsync(localFile);
                yield return ((float)(index + 1)) / count;
            }
        }


        private static IEnumerable<FileInfo> EnumerateFilesRecursive(DirectoryInfo directory)
        {
            foreach(var file in directory.EnumerateFiles())
            {
                yield return file;
            }
            foreach(var subDir in directory.EnumerateDirectories())
            {
                foreach(var subFile in EnumerateFilesRecursive(subDir))
                {
                    yield return subFile;
                }
            }
        }
    }
}