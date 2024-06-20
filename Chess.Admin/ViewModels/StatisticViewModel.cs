using ReactiveUI;

namespace Chess.Admin.ViewModels
{
    public class StatisticViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;
        public ViewModelBase CurrentPage
        {
            get { return _currentPage; }
            set { this.RaiseAndSetIfChanged(ref _currentPage, value); }
        }
    }
}