using ChessDB;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

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

        public ReactiveCommand<Unit, Unit> Create { get; }



        public CreatePageViewModel()
        {
            CurrentDate = DateTimeOffset.Now;

            Strategy = Tactics = Score = Technique = Grade = -1;

            _fenList = new ObservableCollection<string>();

            Clear = ReactiveCommand.Create(PageClear);

            Create = ReactiveCommand.Create(CreateExercisesList, this.WhenAnyValue(x => x.Count, count => count != 0));
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

                    List<string> list2 = new List<string>();

                    //foreach (var item in list)
                    //{
                    //    list2.Add(item.Description);
                    //}

                    FenList = new ObservableCollection<string>(list.Select(x=>x.Description));
                   
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

            Strategy = Tactics = Score = Technique = Grade = -1;

            Count = 0;
        }
    }
}