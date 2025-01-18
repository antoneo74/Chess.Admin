using Avalonia;
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

        public void  StrategyIncrement(object sender, AvaloniaPropertyChangedEventArgs args)
        {
            var cell = this.Find<DataGridTextColumn>("Strategy");

            
        }
    }
}
