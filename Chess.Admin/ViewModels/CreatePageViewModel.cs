using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Chess.Admin.Extensions;
using ChessDB;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;

namespace Chess.Admin.ViewModels
{
    public class CreatePageViewModel : ViewModelBase
    {
        #region private members

        private DateTimeOffset _currentDate;
        private int _strategy;
        private int _tactics;
        private int _score;
        private int _technique;
        private int _grade;
        private int _count;
        private ObservableCollection<string> _fenList;
        private string _message = string.Empty;

        #endregion

        #region public members
        public string Message
        {
            get => _message;

            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public ObservableCollection<string> FenList
        {
            get { return _fenList; }

            set { this.RaiseAndSetIfChanged(ref _fenList, value); }
        }

        public int Count
        {
            get { return _count; }

            set { this.RaiseAndSetIfChanged(ref _count, value); }
        }

        public DateTimeOffset CurrentDate
        {
            get { return _currentDate; }

            set { this.RaiseAndSetIfChanged(ref _currentDate, value); }
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

        public int Technique
        {
            get { return _technique; }

            set { this.RaiseAndSetIfChanged(ref _technique, value); }
        }

        public int Grade
        {
            get { return _grade; }

            set { this.RaiseAndSetIfChanged(ref _grade, value); }
        }

        #endregion

        #region reactive commands
        public ReactiveCommand<Unit, Unit> Clear { get; }

        public ReactiveCommand<Unit, Unit> CreateList { get; }

        public ReactiveCommand<Unit, Unit> CreateFile { get; }

        #endregion

        #region constructor

        public CreatePageViewModel()
        {
            CurrentDate = DateTimeOffset.Now;

            Strategy = Tactics = Score = Technique = Grade = -1;

            _fenList = new ObservableCollection<string>();

            Clear = ReactiveCommand.Create(PageClear);

            CreateList = ReactiveCommand.Create(CreateExercisesList, this.WhenAnyValue(x => x.Count, count => count != 0));

            CreateFile = ReactiveCommand.Create(CreateExercisesFile, this.WhenAnyValue(x => x.FenList.Count, count => count != 0));
        }
        #endregion

        #region save file with exercise

        /// <summary>
        /// Save file with exercises
        /// </summary>        
        private async void CreateExercisesFile()
        {
            try
            {
                if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                                 desktop.MainWindow?.StorageProvider is not { } provider)

                    throw new NullReferenceException("Missing StorageProvider instance.");

                var title = $"{CurrentDate.Day}_{CurrentDate.Month}_{CurrentDate.Year}.txt";

                var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Cохранить задание",

                    DefaultExtension = "txt",

                    SuggestedFileName = title
                });

                if (file is null) return;

                var text = CreateReportText();

                await using var stream = await file.OpenWriteAsync();

                using StreamWriter writer = new(stream);

                await writer.WriteAsync(text);

                Message = "Задание успешно сформировано";
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }
        #endregion

        #region helpers functions

        private string CreateReportText()
        {
            StringBuilder sb = new();
            foreach (var item in FenList)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }

        private void CreateExercisesList()
        {
            try
            {
                using (ChessDbContext context = new())
                {
                    var list = context.Fens.Select(x => x);
                    if (Strategy != -1)
                    {
                        list = list.Where(x => x.Strategy == Strategy);
                    }
                    if (Tactics != -1)
                    {
                        list = list.Where(x => x.Tactics == Tactics);
                    }
                    if (Score != -1)
                    {
                        list = list.Where(x => x.Score == Score);
                    }
                    if (Technique != -1)
                    {
                        list = list.Where(x => x.Technique == Technique);
                    }
                    if (Grade != -1)
                    {
                        list = list.Where(x => x.Grade == Grade);
                    }

                    var filter = list.Select(x => x.Description).ToList();

                    filter.Shuffle();

                    FenList = new ObservableCollection<string>(filter.Take(Count));

                    Message = FenList.Count != 0 ? "Список успешно подготовлен" : "Список пуст";
                }
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }

        private void PageClear()
        {
            CurrentDate = DateTimeOffset.Now;

            FenList.Clear();

            Strategy = Tactics = Score = Technique = Grade = -1;

            Message = string.Empty;

            Count = 0;
        }
        #endregion
    }
}