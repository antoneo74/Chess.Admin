using Avalonia.Media;
using Chess.Admin.Extensions;
using Chess.Admin.Models;
using ChessDB;
using ChessDB.Model;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace Chess.Admin.ViewModels
{
    public class AddPageViewModel : ViewModelBase
    {
        public static IEnumerable<char> Numbers => "87654321";

        public static IEnumerable<char> Letters => "ABCDEFGH";

        #region Private members
        private ObservableCollection<Fen> _listItems;
        private string _fen = string.Empty;
        private int _strategy;
        private int _tactics;
        private int _score;
        private int _technique;
        private int _grade;
        private string _addMessage = string.Empty;
        private string _fenEdit = string.Empty;
        private int _strategyEdit;
        private int _tacticsEdit;
        private int _scoreEdit;
        private int _techniqueEdit;
        private int _gradeEdit;
        private string _editMessage = string.Empty;
        private string _searchMessage = string.Empty;

        private string _searchingFen = string.Empty;
        private bool _isFound;
        private Board? _board;
        private ObservableCollection<Cell> _cells;

        #endregion

        #region Public members
        public ObservableCollection<Fen> ListItems
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

        public bool IsFound
        {
            get => _isFound;

            set => this.RaiseAndSetIfChanged(ref _isFound, value);
        }

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

        public string AddMessage
        {
            get { return _addMessage; }

            set { this.RaiseAndSetIfChanged(ref _addMessage, value); }
        }

        public string SearchMessage
        {
            get { return _searchMessage; }

            set { this.RaiseAndSetIfChanged(ref _searchMessage, value); }
        }

        public string FenEdit
        {
            get { return _fenEdit; }

            set { this.RaiseAndSetIfChanged(ref _fenEdit, value); }
        }

        public string SearchingFen
        {
            get { return _searchingFen; }

            set { this.RaiseAndSetIfChanged(ref _searchingFen, value); }
        }

        public int StrategyEdit
        {
            get { return _strategyEdit; }

            set { this.RaiseAndSetIfChanged(ref _strategyEdit, value); }
        }

        public int TacticsEdit
        {
            get { return _tacticsEdit; }

            set { this.RaiseAndSetIfChanged(ref _tacticsEdit, value); }
        }

        public int ScoreEdit
        {
            get { return _scoreEdit; }

            set { this.RaiseAndSetIfChanged(ref _scoreEdit, value); }
        }

        public int TechniqueEdit
        {
            get { return _techniqueEdit; }

            set { this.RaiseAndSetIfChanged(ref _techniqueEdit, value); }
        }

        public int GradeEdit
        {
            get { return _gradeEdit; }

            set { this.RaiseAndSetIfChanged(ref _gradeEdit, value); }
        }

        public string EditMessage
        {
            get { return _editMessage; }

            set { this.RaiseAndSetIfChanged(ref _editMessage, value); }
        }
        #endregion

        #region Reactive commands
        public ReactiveCommand<Unit, Unit> Add { get; }

        public ReactiveCommand<Unit, Unit> ClearAdd { get; }

        public ReactiveCommand<Unit, Unit> Find { get; }

        public ReactiveCommand<Unit, Unit> ClearEdit { get; }

        public ReactiveCommand<Unit, Unit> SaveChanges { get; }

        public ReactiveCommand<Unit, Unit> DeleteFen { get; }
        #endregion

        #region Constructor
        public AddPageViewModel()
        {
            _board = new Board();

            _cells = new ObservableCollection<Cell>(_board.BoardToList());

            _listItems = [];

            GetAllFens();

            Add = ReactiveCommand.Create(AddFenAsync, this.WhenAnyValue(x => x.Fen, (fen) => !string.IsNullOrWhiteSpace(fen)));

            ClearAdd = ReactiveCommand.Create(ClearAddFields);

            Find = ReactiveCommand.Create(FindFen, this.WhenAnyValue(x => x.SearchingFen, (fen) => !string.IsNullOrEmpty(fen)));

            ClearEdit = ReactiveCommand.Create(ClearEditFields);

            SaveChanges = ReactiveCommand.Create(UpdateAsync, this.WhenAnyValue(x => x.IsFound, (found) => found == true));

            DeleteFen = ReactiveCommand.Create(DeleteAsync, this.WhenAnyValue(x => x.IsFound, (found) => found == true));

            StrategyEdit = TacticsEdit = ScoreEdit = TechniqueEdit = GradeEdit = -1;
        }
        #endregion

        #region Helpers functions        

        /// <summary>
        /// all fields Add Block are filling by default
        /// </summary>        
        private void ClearAddFields()
        {
            Fen = AddMessage = string.Empty;

            Strategy = Tactics = Score = Technique = Grade = 0;
        }

        /// <summary>
        /// all fields Edit Block are filling by default
        /// </summary>   
        private void ClearEditFields()
        {
            FenEdit = EditMessage = string.Empty;

            IsFound = false;

            StrategyEdit = TacticsEdit = ScoreEdit = TechniqueEdit = GradeEdit = -1;
        }

        /// <summary>
        /// Add new fen to DB
        /// </summary>
        private async void AddFenAsync()
        {
            Fen = Fen.Normalization();

            if (!Fen.IsCorrect())
            {
                AddMessage = "FEN имеет некорректный формат";
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
                        Technique = Technique,
                        Grade = Grade,
                    });
                    await context.SaveChangesAsync();
                }
                AddMessage = "Fen успешно добавлен в базу";
                GetAllFens();
            }
            catch (Exception)
            {
                AddMessage = "ƒанный FEN уже есть в базе";
            }
        }


        private void GetAllFens()
        {
            try
            {
                using (ChessDbContext context = new())
                {
                    var fens = context.Fens.ToList();
                    ListItems = new ObservableCollection<Fen>(fens);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Find fen by name
        /// </summary>
        private void FindFen()
        {
            SearchingFen = SearchingFen.Normalization();

            if (!SearchingFen.IsCorrect())
            {
                SearchMessage = "FEN имеет некорректный формат";
                return;
            }

            foreach (var item in ListItems)
            {
                if(item.Description == SearchingFen)
                {
                    SearchMessage = "”спешно! FEN доступен дл€ редактировани€";
                    IsFound = true;

                    FenEdit = SearchingFen;

                    StrategyEdit = item.Strategy;

                    TacticsEdit = item.Tactics;

                    ScoreEdit = item.Score;

                    TechniqueEdit = item.Technique;

                    GradeEdit = item.Grade;

                    return;
                }
            }
            SearchMessage = "FEN отсутствует в базе";
            
        }

        /// <summary>
        /// Delete fen 
        /// </summary>
        private async void DeleteAsync()
        {
            try
            {
                using (ChessDbContext context = new())
                {
                    var fen = await context.Fens.Where(x => x.Description == FenEdit).FirstOrDefaultAsync();

                    if (fen == null)
                    {
                        EditMessage = "FEN отсутствует в базе";
                    }
                    else
                    {
                        context.Remove(fen);

                        await context.SaveChangesAsync();

                        IsFound = false;

                        EditMessage = "FEN успешно удален";
                    }
                }
            }
            catch (Exception)
            {
                EditMessage = "„то-то пошло не так";
            }
        }


        /// <summary>
        /// Uptade params of found fen
        /// </summary>
        private async void UpdateAsync()
        {
            try
            {
                using (ChessDbContext context = new())
                {
                    var fen = await context.Fens.Where(x => x.Description == FenEdit).FirstOrDefaultAsync();

                    if (fen == null)
                    {
                        EditMessage = "FEN отсутствует в базе";
                    }
                    else
                    {
                        IsFound = true;

                        fen.Strategy = StrategyEdit;

                        fen.Tactics = TacticsEdit;

                        fen.Score = ScoreEdit;

                        fen.Technique = TechniqueEdit;

                        fen.Grade = GradeEdit;

                        await context.SaveChangesAsync();

                        EditMessage = "ѕараметры FEN успешно изменены";
                    }
                }
            }
            catch (Exception)
            {
                EditMessage = "„то-то пошло не так";
            }
        }
        #endregion
    }
}
