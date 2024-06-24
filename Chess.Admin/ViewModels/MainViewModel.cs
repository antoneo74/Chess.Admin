using Chess.Admin.Parser;
using Chess.Admin.Services;
using ReactiveUI;
using System.Reactive;

namespace Chess.Admin.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentPage;

    private static IParser _parser = null!;

    public ViewModelBase CurrentPage
    {
        get { return _currentPage; }

        set { this.RaiseAndSetIfChanged(ref _currentPage, value); }
    }

    private bool _isOpen;
    public bool IsOpen
    {
        get { return _isOpen; }

        set => this.RaiseAndSetIfChanged(ref _isOpen, value);
    }

    public MainViewModel(IParser parser)
    {
        _parser = parser;

        _isOpen = false;

        _currentPage = Pages[0];

        OpenPanel = ReactiveCommand.Create(() => { IsOpen = IsOpen != true; });

        ContextPageCommand = ReactiveCommand.Create<string>(GetContext);
    }

    private void GetContext(string index)
    {
        int.TryParse(index, out int i);

        CurrentPage = Pages[i];
    }

    private ViewModelBase[] Pages =
    {
        new AddPageViewModel(),
        new CreatePageViewModel(),
        new CheckViewModel(_parser),
        new StatisticViewModel()
    };

    public ReactiveCommand<Unit, Unit> OpenPanel { get; }

    public ReactiveCommand<string, Unit> ContextPageCommand { get; }    
}
