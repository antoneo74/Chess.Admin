using Avalonia.Controls;
using Chess.Admin.Parser;
using Chess.Admin.ViewModels;
using Chess.Core;

namespace Chess.Admin.Views
{
    public partial class CheckView : UserControl
    {
        public CheckView()
        {
            InitializeComponent();
            Design.SetDataContext(this, new CheckViewModel(new FenParser(), new BoardAnalysis()));
        }
    }
}
