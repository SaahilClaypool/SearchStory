using Microsoft.Extensions.DependencyInjection;

namespace SearchStory.App.UseCases
{
    public static class UseCaseExtensions
    {
        /// <summary>
        /// TODO: make this search the assembly for all relevant use cases
        /// </summary>
        public static IServiceCollection AddUseCases(this IServiceCollection collection)
        {
            collection.AddScoped<AddDocuments>();
            collection.AddScoped<ListDocuments>();
            collection.AddScoped<ClearAll>();
            collection.AddScoped<AddWebpage>();
            collection.AddScoped<IndexDirectoryUseCase>();
            return collection;
        }
    }
}