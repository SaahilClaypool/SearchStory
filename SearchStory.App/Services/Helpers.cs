namespace SearchStory.App.Services
{
    public static class Helper
    {
        public static string Random() => Random(8);
        public static string Random(int length) => System.Guid.NewGuid().ToString()[..length];
    }
}