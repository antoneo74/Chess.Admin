using Chess.Admin.Parser;
using Chess.Admin.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;

namespace Chess.Admin.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentPage;

    private IParser _parser = null!;

    private IAnalysis _analysis = null!;

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

    public MainViewModel(IParser parser, IAnalysis analysis)
    {
        _parser = parser;

        _analysis = analysis;

        Pages = new List<ViewModelBase>()
        {
            new AddPageViewModel(_parser),
            new CreatePageViewModel(),
            new CheckViewModel(_parser, _analysis),
            new StatisticViewModel()
        };

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

    private List<ViewModelBase> Pages;

    public ReactiveCommand<Unit, Unit> OpenPanel { get; }

    public ReactiveCommand<string, Unit> ContextPageCommand { get; }
}
