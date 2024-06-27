using Avalonia.Controls;
using Chess.Admin.Parser;
using Chess.Admin.ViewModels;
using Chess.Core;

namespace Chess.Admin.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Design.SetDataContext(this, new MainViewModel(new FenParser(), new BoardAnalysis()));
    }
}
