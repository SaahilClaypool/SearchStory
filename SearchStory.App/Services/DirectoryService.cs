using System;
using System.IO;


namespace SearchStory.App.Services
{
    public class DirectoryService
    {
        readonly char Slash = Path.DirectorySeparatorChar;
        // TODO: could read from environment variables too
        private string DataPath =>
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
            Slash +
            "SearchStory" +
            Slash;
        private string IndexPath =>
            DataPath + "Index" + Slash;
        private string DocumentPath =>
            DataPath + "Documents" + Slash;
        private string TempPath =>
            DataPath + "Temp" + Slash;


        /// <summary>
        /// Root directory for all application data.
        /// See the helpers below for the useful folders
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo DataDir => new(DataPath);
        public DirectoryInfo IndexDir => new(IndexPath);
        public DirectoryInfo DocumentDir => new(DocumentPath);
        public DirectoryInfo TempDir => new(TempPath);

        public void EnsurePathsExist()
        {
            if (!DataDir.Exists)
            {
                DataDir.Create();
                new DirectoryInfo(IndexPath).Create();
                new DirectoryInfo(DocumentPath).Create();
                new DirectoryInfo(TempPath).Create();
            }
        }
    }
}