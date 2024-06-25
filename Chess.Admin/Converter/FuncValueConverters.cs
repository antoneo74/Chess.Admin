﻿using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Chess.Admin.Models;

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


        public static FuncValueConverter<Exercise?, string?> GetComboboxItem { get; } =
            new FuncValueConverter<Exercise?, string?>(s =>
            {
                // define output variable
                string? title = null;
                if (s != null)
                {
                    title = $"Упражнение {s.Id}";
                }
                return title;
            });

        public static FuncValueConverter<Cell, Bitmap?> GetBitmap { get; } =
        new FuncValueConverter<Cell, Bitmap?>(s =>
        {
            // define output variable
            Bitmap? image = null;
            if (s != null)
            {
                switch (s.Figure)
                {
                    case Figure.Empty:
                        break;
                    case Figure.King:
                        image = (s.Color == FigureColor.White) ?
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/whiteKing.png"))) :
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/blackKing.png")));
                        break;
                    case Figure.Queen:
                        image = (s.Color == FigureColor.White) ?
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/whiteQueen.png"))) :
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/blackQueen.png")));
                        break;
                    case Figure.Rook:
                        image = (s.Color == FigureColor.White) ?
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/whiteRook.png"))) :
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/blackRook.png")));
                        break;
                    case Figure.Knight:
                        image = (s.Color == FigureColor.White) ?
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/whiteHorse.png"))) :
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/blackHorse.png")));
                        break;
                    case Figure.Bishop:
                        image = (s.Color == FigureColor.White) ?
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/whiteBishop.png"))) :
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/blackBishop.png")));
                        break;
                    case Figure.Pawn:
                        image = (s.Color == FigureColor.White) ?
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/whitePawn.png"))) :
                        new Bitmap(AssetLoader.Open(new System.Uri("avares://Chess.Admin/Assets/blackPawn.png")));
                        break;
                    default:
                        break;
                }
            }
            return image;
        });
    }
}
