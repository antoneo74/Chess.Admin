using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Chess.Admin.Extensions;
using ChessDB;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Admin.ViewModels
{
    public class CreatePageViewModel : ViewModelBase
    {
        private DateTimeOffset _currentDate;
        private int _strategy;
        private int _tactics;
        private int _score;
        private int _technique;
        private int _grade;
        private int _count;
        private ObservableCollection<string> _fenList;
        private string _message = string.Empty;

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

        public ReactiveCommand<Unit, Unit> Clear { get; }

        public ReactiveCommand<Unit, Unit> CreateList { get; }

        public ReactiveCommand<Unit, Unit> CreateFile { get; }

        public CreatePageViewModel()
        {
            CurrentDate = DateTimeOffset.Now;

            Strategy = Tactics = Score = Technique = Grade = -1;

            _fenList = new ObservableCollection<string>();

            Clear = ReactiveCommand.Create(PageClear);

            CreateList = ReactiveCommand.Create(CreateExercisesList, this.WhenAnyValue(x => x.Count, count => count != 0));

            CreateFile = ReactiveCommand.CreateFromTask(CreateExercisesFile, this.WhenAnyValue(x=>x.FenList.Count, count => count != 0));
        }

       

        /// <summary>
        /// Save report file
        /// </summary>
        /// <returns></returns>
        private async Task CreateExercisesFile()
        {
            try
            {
                var folder = await DoOpenFolderPickerAsync();

                if (folder is null) return;

                var folderPath = folder.ToList()[0].TryGetLocalPath() ?? throw new Exception("Выбрана некорректная папка");

                var title = $"{CurrentDate.Day}_{CurrentDate.Month}_{CurrentDate.Year}.txt";

                var path = System.IO.Path.Combine(folderPath, title);

                var text = CreateReportText();

                await File.WriteAllTextAsync(path, text);

                Message = "Задание успешно сформировано";
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }

        private string CreateReportText()
        {
            StringBuilder sb = new();
            foreach (var item in FenList)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// SaveFileDialog
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private static async Task<IReadOnlyList<IStorageFolder>> DoOpenFolderPickerAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                 desktop.MainWindow?.StorageProvider is not { } provider)

                throw new NullReferenceException("Missing StorageProvider instance.");

            return await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Выбери куда сохранить задание"
            });
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

                    Message = "Список успешно подготовлен";
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
    }
}