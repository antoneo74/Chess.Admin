using Avalonia.Controls;
using Chess.Admin.Parser;
using Chess.Admin.ViewModels;

namespace Chess.Admin.Views
{
    public partial class AddPageView : UserControl
    {
        public AddPageView()
        {
            InitializeComponent();
            Design.SetDataContext(this, new AddPageViewModel(new FenParser()));
        }
    }
}
