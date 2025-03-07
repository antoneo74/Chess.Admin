using ChessDB;
using ChessDB.Model;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Chess.Admin.ViewModels
{
    public class StatisticViewModel : ViewModelBase
    {
        #region Private members
        private ObservableCollection<Person> _people;

        private bool _common;

        private string _name = string.Empty;

        private string _surname = string.Empty;

        private string _message = string.Empty;

        private bool _isFound = false;
        #endregion

        #region Public members
        public bool IsFound
        {
            get => _isFound;

            set => this.RaiseAndSetIfChanged(ref _isFound, value);
        }

        public string Message
        {
            get => _message;

            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public string Name
        {
            get { return _name; }

            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string Surname
        {
            get { return _surname; }

            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }
        public bool Common
        {
            get { return _common; }

            set => this.RaiseAndSetIfChanged(ref _common, value);
        }

        public ObservableCollection<Person> People
        {
            get { return _people; }

            set { this.RaiseAndSetIfChanged(ref _people, value); }
        }
        #endregion

        #region Constructor
        public StatisticViewModel()
        {
            _people = new ObservableCollection<Person>();

            _common = true;

            ShowAll = ReactiveCommand.CreateFromTask(GetPeople);

            ClearCommand = ReactiveCommand.Create(Clear, this.WhenAnyValue(
                x => x.People,
                x => x.Name,
                x => x.Surname,
                (people, name, surname) => people.Count != 0 || !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(surname)));

            UpdateCommand = ReactiveCommand.CreateFromTask(Update, this.WhenAnyValue(x => x.People, p => p.Count != 0));

            SearchPersonCommand = ReactiveCommand.CreateFromTask(SearchPersonAsync, this.WhenAnyValue(
                x => x.Name,
                x => x.Surname,
                (name, surname) => !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname)));

            ClearStatisticCommand = ReactiveCommand.CreateFromTask(ClearStatistic, this.WhenAnyValue(
                x => x.Name,
                x => x.Surname,
                x => x.IsFound,
                (name, surname, isFound) => !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && isFound == true));
        }
        #endregion

        #region Helper functions
        private async Task ClearStatistic()
        {
            try
            {
                using ChessDbContext context = new();

                var name = Name[..1].ToUpper() + Name.Substring(1).ToLower();

                var surname = Surname[..1].ToUpper() + Surname.Substring(1).ToLower();

                var person = await context.Persons.Where(x => x.FirstName == name && x.LastName == surname).FirstOrDefaultAsync();

                if (person != null)
                {
                    person.TotalExercises = 0;

                    person.WeaknessError = 0;

                    person.CaptureError = 0;

                    IsFound = false;

                    await context.SaveChangesAsync();

                    Message = "�������! ����� ������ ��������";
                }
            }
            catch (Exception)
            {
                Message = "���-�� ����� �� ���";
            }
        }

        private async Task SearchPersonAsync()
        {
            try
            {
                using ChessDbContext context = new();

                var name = Name[..1].ToUpper() + Name.Substring(1).ToLower();

                var surname = Surname[..1].ToUpper() + Surname.Substring(1).ToLower();

                var person = await context.Persons.Where(x => x.LastName == surname && x.FirstName == name).FirstOrDefaultAsync();

                if (person != null)
                {
                    People = new ObservableCollection<Person>(new List<Person> { person });

                    IsFound = true;
                }
                else
                {
                    Message = "������ ������� ����������� � ����";
                }
            }
            catch (Exception)
            {
                Message = "���-�� ����� �� ���";
            }
        }

        private async Task Update()
        {
            await GetPeople();
        }

        private void Clear()
        {
            People = new();

            Name = string.Empty;

            Surname = string.Empty;

            Message = string.Empty;

            IsFound = false;
        }

        private async Task GetPeople()
        {
            try
            {
                using ChessDbContext context = new();

                var list = await context.Persons.ToListAsync();

                People = new ObservableCollection<Person>(list);

                Message = People.Count == 0 ? "������ ����" : "�������";
            }
            catch (Exception)
            {
                Message = "���-�� ����� �� ���";
            }
        }
        #endregion

        #region Reactive commands
        public ReactiveCommand<Unit, Unit> ShowAll { get; }

        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }

        public ReactiveCommand<Unit, Unit> SearchPersonCommand { get; }

        public ReactiveCommand<Unit, Unit> ClearStatisticCommand { get; }
        #endregion
    }
}