using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Chess.Admin.Models;
using Chess.Admin.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Avalonia;
using Chess.Admin.Parser;

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
        public int Index
        {
            get => _index;

            set
            {
                this.RaiseAndSetIfChanged(ref _index, value);

                //if (_index != -1)
                //{
                //    Load();
                //}
                //AutoFillAnswersFields();
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

        public CheckViewModel(IParser parser)
        {
            _parser = parser;
            _board = new Board();
            _cells = new ObservableCollection<Cell>(_board.BoardToList());
            _listItems = new ObservableCollection<Exercise>();
        }

        #region Open file
        private async Task OpenFile(CancellationToken token)
        {
            try
            {
                var file = await DoOpenFilePickerAsync() ?? throw new Exception("Файл не выбран или имеет недопустимый формат");

                // Limit the text file to 1MB so that the demo won't lag.
                if ((await file.GetBasicPropertiesAsync()).Size <= 1024 * 1024 * 1)
                {
                    int index = 1;

                    await using var readStream = await file.OpenReadAsync();

                    using var reader = new StreamReader(readStream);

                    ListItems.Clear();

                    while (!reader.EndOfStream)
                    {
                        string? s;

                        if ((s = await reader.ReadLineAsync(token)) != null)
                        {
                            var item = new Exercise()
                            {
                                FenItem = s,
                                Id = index++
                            };

                            ListItems.Add(item);
                        }
                    }

                    Index = ListItems.Count == 0 ? -1 : 0;

                    FileIsLoaded = true;

                    Message = "Файл успешно загружен";
                }
                else
                {
                    throw new Exception("File exceeded 1MB limit.");
                }
            }
            catch (Exception e)
            {
                Message = e.Message;
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

            if (regex.IsMatch(name))
            {
                //_exercisesName = name;

                return files[0];
            }
            return null;
        }
        #endregion
    }
}