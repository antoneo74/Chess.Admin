using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Chess.Admin.Models;
using Chess.Admin.Parser;
using Chess.Admin.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private int _index;

        private string _message = string.Empty;

        private User? _user;

        private Board? _board;

        private ObservableCollection<Cell> _cells;

        private ObservableCollection<Exercise> _listItems;

        private bool _fileIsLoaded;

        // private string _exercisesName;
        #endregion

        #region Public members
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

        public bool FileIsLoaded
        {
            get => _fileIsLoaded;

            set => this.RaiseAndSetIfChanged(ref _fileIsLoaded, value);
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

        #endregion

        #region Constructor

        public CheckViewModel(IParser parser)
        {
            _index = -1; 

            _parser = parser;

            _board = new Board();

            _cells = new ObservableCollection<Cell>(_board.BoardToList());

            _listItems = new ObservableCollection<Exercise>();

            LoadCommand = ReactiveCommand.CreateFromTask(OpenFileAsync);

            ClearCommand = ReactiveCommand.Create(Clear);
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

                Index = ListItems.Count == 0 ? -1 : 0;

                FileIsLoaded = true;

                Message = "Файл успешно загружен";
            }
            catch (Exception)
            {
                Clear();

                Message = "Что-то пошло не так";
            }
        }

        private async Task<IStorageFile?> DoOpenFilePickerAsync()
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

            string pattern = @"^[a-zA-Zа-яА-Я]+_[a-zA-Zа-яА-Я]+_[1-9][0-9]?_[1-9][0-9]?_20[0-9]{2}.txt$";

            Regex regex = new(pattern);

            var name = files[0].Name;

            if (regex.IsMatch(name)) return files[0];

            return null;
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

        private void Clear()
        {
            Message = string.Empty;

            Index = -1;

            ListItems.Clear();

            _board = new Board();

            Cells = new ObservableCollection<Cell>(_board.BoardToList());
        }
    }
}