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
using System.Threading.Tasks;

namespace Chess.Admin.ViewModels
{
    public class StatisticViewModel : ViewModelBase
    {
        private ObservableCollection<Person> _people;

        private bool _common;

        private string _name = string.Empty;

        private string _surname = string.Empty;

        private string _message = string.Empty;

        private bool _isFound = false;

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

        public StatisticViewModel()
        {
            _people = new ObservableCollection<Person>();

            _common = true;

            ShowAll = ReactiveCommand.Create(GetPeople);

            ClearCommand = ReactiveCommand.Create(Clear, this.WhenAnyValue(
                x => x.People,
                x => x.Name,
                x => x.Surname,
                (people, name, surname) => people.Count != 0 || !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(surname)));

            UpdateCommand = ReactiveCommand.Create(Update, this.WhenAnyValue(x => x.People, p => p.Count != 0));

            SearchPersonCommand = ReactiveCommand.Create(SearchPersonAsync, this.WhenAnyValue(
                x => x.Name,
                x => x.Surname,
                (name, surname) => !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname)));

            ClearStatisticCommand = ReactiveCommand.Create(ClearStatistic, this.WhenAnyValue(
                x => x.Name,
                x => x.Surname,
                x => x.IsFound,
                (name, surname, isFound) => !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && isFound == true));
        }

        private async void ClearStatistic()
        {
            try
            {
                using (ChessDbContext context = new())
                {
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

                        Message = "Успешно! Нажми кнопку обновить";
                    }
                }
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }

        private async void SearchPersonAsync()
        {
            try
            {
                using (ChessDbContext context = new())
                {
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
                        Message = "Данный человек отсутствует в базе";
                    }
                }
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }

        private void Update()
        {
            GetPeople();
        }

        private void Clear()
        {
            People = new();

            Name = string.Empty;

            Surname = string.Empty;

            Message = string.Empty;

            IsFound = false;
        }

        private void GetPeople()
        {
            try
            {
                using (ChessDbContext context = new())
                {
                    People = new ObservableCollection<Person>([.. context.Persons]);
                }
            }
            catch (Exception)
            {
                Message = "Что-то пошло не так";
            }
        }

        public ReactiveCommand<Unit, Unit> ShowAll { get; }

        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }

        public ReactiveCommand<Unit, Unit> SearchPersonCommand { get; }

        public ReactiveCommand<Unit, Unit> ClearStatisticCommand { get; }
    }
}