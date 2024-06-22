using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Chess.Admin.Converter
{
    public static class FuncValueConverters
    {
        public static FuncValueConverter<string, IBrush?> GetForeground { get; } =
        new FuncValueConverter<string, IBrush?>(s =>
        {
            if (s == null) return null;

            if (s.ToLower().Contains("успешно")) return new SolidColorBrush(Colors.Green);

            return new SolidColorBrush(Colors.Red);
        });
    }
}
