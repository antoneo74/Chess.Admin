using ReactiveUI;
using System;

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

        public DateTimeOffset CurrentDate
        {
            get { return _currentDate; }
            set { this.RaiseAndSetIfChanged(ref _currentDate, value); _dateString = value.ToString(); }
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



        private string _dateString = string.Empty;

        public string DateString
        {
            get { return _dateString; }
            set => this.RaiseAndSetIfChanged(ref _dateString, value);
        }

        public CreatePageViewModel()
        {
            CurrentDate = DateTimeOffset.Now;

            Strategy = Tactics = Score = Technique = Grade = -1;
        }

    }
}