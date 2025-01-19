using Chess.Admin.DependencyInjection;
using Chess.Admin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace Chess.Admin.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Private members
    private readonly List<ViewModelBase> Pages;

    private readonly IParser? _parser;

    private ViewModelBase? _currentPage;

    private bool _isOpen;
    #endregion

    #region Public members
    public ViewModelBase? CurrentPage
    {
        get { return _currentPage; }

        set { this.RaiseAndSetIfChanged(ref _currentPage, value); }
    }

    public bool IsOpen
    {
        get { return _isOpen; }

        set => this.RaiseAndSetIfChanged(ref _isOpen, value);
    }
    public ReactiveCommand<Unit, Unit> OpenPanel { get; }

    public ReactiveCommand<string, Unit> ContextPageCommand { get; }
    #endregion

    #region Constructor (Fabric method)
    private MainViewModel()
    {
        var services = new ServiceCollection();

        services.AddCommonServices();

        using var serviceProvider = services.BuildServiceProvider();

        var check = serviceProvider.GetService<CheckViewModel>();

        var statistic = serviceProvider.GetService<StatisticViewModel>();

        var create = serviceProvider.GetService<CreatePageViewModel>();

        _parser = serviceProvider.GetService<IParser>();

        Pages =
        [
            create,
            check,
            statistic
        ];

        _isOpen = false;

        OpenPanel = ReactiveCommand.Create(() => { IsOpen = IsOpen != true; });

        ContextPageCommand = ReactiveCommand.Create<string>(GetContext);
    }
    #endregion

    #region Methods
    private async Task<MainViewModel> InitializeAsync()
    {
        var add = await AddPageViewModel.CreateAsync(_parser);

        Pages.Insert(0, add);

        _currentPage = Pages[0];

        return this;
    }

    public static Task<MainViewModel> CreateAsync()
    {
        var ret = new MainViewModel();

        return ret.InitializeAsync();
    }

    private void GetContext(string index)
    {
        if (int.TryParse(index, out int i)) CurrentPage = Pages[i];
    }
    #endregion
}
