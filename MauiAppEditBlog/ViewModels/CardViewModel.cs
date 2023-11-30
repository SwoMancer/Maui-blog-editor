using MauiAppEditBlog.DB;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
//using Windows.ApplicationModel.VoiceCommands;

namespace MauiAppEditBlog.ViewModels
{
    public class CardViewModelConfig
    {
        public int Id { get; set; } 
        public DateTime Date { get; set; } 
        public string Title { get; set; } 
        public string Text { get; set; }
        public string ConnectionString { get; set; }
    }
    public class CardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MariaDBHandler _mariaDB;
        private int _id;
        private DateTime _date;
        private string _title;
        private string _text;
        private string[] _imagesLinks;
        private bool _isThisCardReal = false;
        private bool _IsThisCardNew = false;

        public CardViewModel(string connectionString) 
        {
            _mariaDB = new MariaDBHandler(connectionString);
            _IsThisCardNew = true;
        }

        public CardViewModel(CardViewModelConfig config)
        {
            _id = config.Id;
            _date = config.Date;
            _title = config.Title;
            _text = config.Text;
            _mariaDB = new MariaDBHandler(config.ConnectionString);
            _isThisCardReal = true;
            _IsThisCardNew = false;
        }
        public bool IsThisCardReal { get => _isThisCardReal; }
        public bool IsThisCardNew { get => _IsThisCardNew; }
        public int Id{ get => _id; }
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public string Links
        {
            get
            {
                try
                {
                    return string.Join("\n", _imagesLinks);
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                _imagesLinks = value.Split("\n");
                OnPropertyChanged(nameof(Links));
            }
        }
        public async Task LoadLinksAsync()
        {
            _imagesLinks = await _mariaDB.GetAllImagesForCardAsync(this);
            OnPropertyChanged(nameof(Links));
        }
        public void ActiveState() => _mariaDB.Open();
        public Task ActiveStateAsync() => _mariaDB.OpenAsync();
        public void SlumberState() => _mariaDB.Close();
        public Task SlumberStateAsync() => _mariaDB.CloseAsync();
        public async Task UpdateAsync() => await _mariaDB.UpdateCardItemInDatabaseAsync(this);
        public async Task CreateAsync() 
        { 
            await _mariaDB.CreateNewCardItemToDatabaseAsync(this);
            _IsThisCardNew = true;
            _isThisCardReal = true;
        }
        public async Task DeleteAsync()
        {
            await _mariaDB.DeleteCardItemFromDatabaseAsync(this);
            _isThisCardReal = false;
        }
        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
