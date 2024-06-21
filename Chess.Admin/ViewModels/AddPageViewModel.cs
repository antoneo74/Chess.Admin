using Chess.Admin.Extensions;
using ChessDB;
using ChessDB.Model;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive;

namespace Chess.Admin.ViewModels
{
    public class AddPageViewModel : ViewModelBase
    {
        private string _fen = string.Empty;
        private int _strategy;
        private int _tactics;
        private int _score;
        private int _techique;
        private int _grade;
        private string _addMessage = string.Empty;

        public string Fen
        {
            get { return _fen; }

            set { this.RaiseAndSetIfChanged(ref _fen, value); }
        }

        public int Strategy
        {
            get { return _strategy; }

            set { this.RaiseAndSetIfChanged(ref _strategy, value); }
        }

        public int Tactics
        {
            get { return _tactics; }

            set { this.RaiseAndSetIfChanged(ref _tactics, value); }
        }

        public int Score
        {
            get { return _score; }

            set { this.RaiseAndSetIfChanged(ref _score, value); }
        }

        public int Techique
        {
            get { return _techique; }

            set { this.RaiseAndSetIfChanged(ref _techique, value); }
        }

        public int Grade
        {
            get { return _grade; }

            set { this.RaiseAndSetIfChanged(ref _grade, value); }
        }

        public string AddMessage
        {
            get { return _addMessage; }

            set { this.RaiseAndSetIfChanged(ref _addMessage, value); }
        }

        public ReactiveCommand<Unit, Unit> Add { get; }

        public ReactiveCommand<Unit, Unit> ClearAdd { get; }

        public AddPageViewModel()
        {
            Add = ReactiveCommand.Create(AddFen, this.WhenAnyValue(x => x.Fen, (fen) => !string.IsNullOrWhiteSpace(fen)));

            ClearAdd = ReactiveCommand.Create(ClearAddFields);
        }

        private void ClearAddFields()
        {
            Fen = AddMessage = string.Empty;

            Strategy = Tactics = Score = Techique = Grade = 0;
        }

        private async void AddFen()
        {
            Fen = Fen.Normalization();

            if (!Fen.IsCorrect())
            {
                AddMessage = "Неудача";
                return;
            }

            try
            {
                using (ChessDbContext context = new())
                {
                    await context.Fens.AddAsync(new Fen
                    {
                        Description = Fen,
                        Strategy = Strategy,
                        Tactics = Tactics,
                        Score = Score,
                        Technique = Techique,
                        Grade = Grade,
                    });
                    await context.SaveChangesAsync();
                }
                AddMessage = "Успех";
            }
            catch (Exception)
            {
                AddMessage = "Неудача";
            }
        }
    }
}