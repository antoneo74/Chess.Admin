using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Chess.Admin.Extensions;
using Chess.Admin.Models;
using Chess.Admin.Parser;
using Chess.Admin.Services;
using ChessDB;
using ChessDB.Model;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Admin.ViewModels
{
    public class CheckViewModel : ViewModelBase
    {
        public static IEnumerable<char> Numbers => "87654321";

        public static IEnumerable<char> Letters => "ABCDEFGH";

        #region Private members

        private readonly IParser _parser;

        private readonly IAnalysis _analysis;

        private int _index;

        private string _message = string.Empty;

        private User? _user;

        private Board? _board;

        private bool _fileIsLoaded;

        private ObservableCollection<Cell> _cells;

        private ObservableCollection<Exercise> _listItems;

        private int _total;

        private int _captureError;

        private int _weaknessError;
        #endregion

        #region Public members

        public bool FileIsLoaded
        {
            get => _fileIsLoaded;

            set => this.RaiseAndSetIfChanged(ref _fileIsLoaded, value);
        }

        public int Total
        {
            get => _total;

            set => this.RaiseAndSetIfChanged(ref _total, value);
        }

        public int CaptureError
        {
            get => _captureError;

            set => this.RaiseAndSetIfChanged(ref _captureError, value);
        }

        public int WeaknessError
        {
            get => _weaknessError;

            set => this.RaiseAndSetIfChanged(ref _weaknessError, value);
        }

        public int Index
        {
            get => _index;

            set
            {
                this.RaiseAndSetIfChanged(ref _index, value);

                if (_index != -1)
                {
                    Load();
                }
            }
        }

        public User? User
        {
            get => _user;

            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        public ObservableCollection<Exercise> ListItems
        {
            get => _listItems;

            set => this.RaiseAndSetIfChanged(ref _listItems, value);
        }

        public Board? Board
        {
            get => _board;

            set => this.RaiseAndSetIfChanged(ref _board, value);
        }

        public ObservableCollection<Cell> Cells
        {
            get => _cells;

            set => this.RaiseAndSetIfChanged(ref _cells, value);
        }

        public string Message
        {
            get => _message;

            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
        #endregion

        #region ReactiveCommands
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        public ReactiveCommand<Unit, Unit> DataBaseCommand { get; }

        #endregion

        #region Constructor

        public CheckViewModel(IParser parser, IAnalysis analysis)
        {
            _index = -1;

            _parser = parser;

            _analysis = analysis;

            _board = new Board();

            _cells = new ObservableCollection<Cell>(_board.BoardToList());

            _listItems = [];

            LoadCommand = ReactiveCommand.CreateFromTask(OpenFileAsync);

            ClearCommand = ReactiveCommand.Create(Clear);

            DataBaseCommand = ReactiveCommand.Create(AddResultToDataBaseAsync, this.WhenAnyValue(x => x.ListItems.Count, (count) => count != 0));
        }

        #endregion

        #region Open file
        private async Task OpenFileAsync(CancellationToken token)
        {
            try
            {
                var file = await DoOpenFilePickerAsync() ?? throw new Exception("Файл не выбран или имеет недопустимый формат");

                await using var readStream = await file.OpenReadAsync();

                using var reader = new StreamReader(readStream);

                string? s = await reader.ReadToEndAsync(token);

                AnswerParser.Parse(s, ref _user, ref _listItems);

                GetAnswer();

                Index = ListItems.Count == 0 ? -1 : 0;

                Message = "Файл успешно загружен";
            }
            catch (Exception)
            {
                Clear();

                Message = "Файл имеет неправильный формат";
            }
        }

        private static async Task<IStorageFile?> DoOpenFilePickerAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
                || desktop.MainWindow?.StorageProvider is not { } provider)

                throw new NullReferenceException("Missing StorageProvider instance.");

            var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Выбери файл на проверку",

                AllowMultiple = false
            });

            if (files.Count == 0) return null;

            return files[0];
        }
        #endregion

        /// <summary>
        /// Load new board from fen
        /// </summary>
        private void Load()
        {
            try
            {
                Message = string.Empty;

                if (Index != -1 && ListItems.Count != 0)
                {
                    _board = _parser.Parse(ListItems[Index].FenItem);

                    FileIsLoaded = true;
                }
                else
                {
                    _board = new Board();
                }

                if (_board != null)
                {
                    Cells = new ObservableCollection<Cell>(_board.BoardToList());
                }
                else
                {
                    Message = "Неправильный формат входной строки";
                }
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }

        /// <summary>
        /// Clear all fields
        /// </summary>
        private void Clear()
        {
            Message = string.Empty;

            Index = -1;

            ListItems.Clear();

            FileIsLoaded = false;

            _board = new Board();

            Total = CaptureError = WeaknessError = 0;

            Cells = new ObservableCollection<Cell>(_board.BoardToList());
        }

        private void GetAnswer()
        {
            if (ListItems.Count == 0) return;

            var temp = ListItems;

            ListItems = [];

            foreach (var item in temp)
            {
                var currentBoard = _parser.Parse(item.FenItem);

                if (currentBoard != null)
                {
                    currentBoard = _analysis.Analysis(currentBoard);

                    if (currentBoard.GetWhiteAttacks() == item.WhiteCapture) item.WCError = true;

                    if (currentBoard.GetWeaknessWhite() == item.WhiteWeakness) item.WWError = true;

                    if (currentBoard.GetBlackAttacks() == item.BlackCapture) item.BCError = true;

                    if (currentBoard.GetWeaknessBlack() == item.BlackWeakness) item.BWError = true;
                }
                ListItems.Add(item);
            }

            // each exercise has 4 questions            
            Total = ListItems.Count * 4;

            CaptureError = ListItems.Where(x => x.BCError == false).Count() + ListItems.Where(x => x.WCError == false).Count();

            WeaknessError = ListItems.Where(x => x.BWError == false).Count() + ListItems.Where(x => x.WWError == false).Count();
        }

        private async void AddResultToDataBaseAsync()
        {
            if (User == null) return;
            try
            {
                var name = User.FirstName[..1].ToUpper() + User.FirstName[1..].ToLower();

                var surname = User.LastName[..1].ToUpper() + User.LastName[1..].ToLower();

                using (var context = new ChessDbContext())
                {
                    var person = await context.Persons.Where(p => p.FirstName == name
                                                               && p.LastName == surname).FirstOrDefaultAsync();
                    if (person != null)
                    {
                        person.TotalExercises += Total;

                        person.CaptureError += CaptureError;

                        person.WeaknessError += WeaknessError;
                    }
                    else
                    {
                        await context.AddAsync(new Person
                        {
                            FirstName = name,
                            LastName = surname,
                            TotalExercises = Total,
                            CaptureError = CaptureError,
                            WeaknessError = WeaknessError
                        });
                    }
                    await context.SaveChangesAsync();
                }

                Message = "Результаты успешно сохранены";
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }
    }
}