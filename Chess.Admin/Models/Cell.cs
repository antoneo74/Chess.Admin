using Avalonia.Media;
using ReactiveUI;

namespace Chess.Admin.Models
{
    public class Cell : ReactiveObject
    {
        private Figure _figure;

        public Figure Figure
        {
            get => _figure;
            set => this.RaiseAndSetIfChanged(ref _figure, value);
        }

        public IBrush BackgroundColor { get; set; } = new SolidColorBrush(Colors.Gray);

        public FigureColor Color { get; set; }

        public int ProtectionsCount { get; set; }

        public int EnemyAttacksCount { get; set; }

        public int Capture { get; set; }
    }
}
