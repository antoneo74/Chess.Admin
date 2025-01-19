using Chess.Admin.Core;
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

            collection.AddSingleton<IAnalysis, BoardAnalysis>();

            collection.AddTransient<CheckViewModel>();

            collection.AddTransient<CreatePageViewModel>();

            collection.AddTransient<StatisticViewModel>();
        }
    }
}
