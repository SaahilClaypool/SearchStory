using System;
using System.IO;
namespace SearchStory.App.Services
{
    public class Configuration
    {
        // TODO: could read from environment variables too
        public string DataDirectory =>
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
            Path.DirectorySeparatorChar +
            "SearchStory";
        public string IndexDirectory =>
            DataDirectory + Path.DirectorySeparatorChar + "Index";
        public string DocumentDirectory =>
            DataDirectory + Path.DirectorySeparatorChar + "Documents";
    }
}