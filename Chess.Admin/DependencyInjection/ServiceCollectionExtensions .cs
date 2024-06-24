using Chess.Admin.Parser;
using Chess.Admin.Services;
using Chess.Admin.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Admin.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<IParser, FenParser>();

            collection.AddTransient<MainViewModel>();

            //collection.AddTransient<CheckViewModel>();
        }        
    }
}
