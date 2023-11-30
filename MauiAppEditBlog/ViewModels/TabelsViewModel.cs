using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MauiAppEditBlog.DB;
using Microsoft.Maui.Controls;
//using static CoreFoundation.DispatchSource;

namespace MauiAppEditBlog.ViewModels
{
    public class TabelsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private MariaDBHandler _mariaDB;

        private ObservableCollection<CardViewModel> _cards = new ObservableCollection<CardViewModel>();

        public ObservableCollection<CardViewModel> Cards
        {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged();
            }
        }

        public TabelsViewModel(string connectionString)
        {
            _mariaDB = new MariaDBHandler(connectionString);
        }
        public async Task PopulateDataFromDatabaseAsync()
        {
            Cards = await _mariaDB.GetAllCardsAsync();
        }
        public Task ActiveStateAsync() => _mariaDB.OpenAsync();
        public void SlumberState() => _mariaDB.Close();
        public Task SlumberStateAsync() => _mariaDB.CloseAsync();

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void UpdateLocalCards()
        {
            foreach (CardViewModel card in _cards.Where(i => i.IsThisCardReal == false).ToList())
            {
                _cards.Remove(card);
            }
        }
    }
}
