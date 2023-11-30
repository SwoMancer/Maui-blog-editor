using MauiAppEditBlog.ViewModels;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MauiAppEditBlog.DB
{

    public class MariaDBHandler
    {
        private MySqlConnection _connection;
        private string _connectionString;

        public MariaDBHandler(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connectionString = connectionString;
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void Close() => _connection.Close();
        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public Task CloseAsync() => _connection.CloseAsync();
        /// <summary>
        /// Opens a database connection.
        /// </summary>
        public void Open() => _connection.Open();
        /// <summary>
        /// Opens a database connection.
        /// </summary>
        public Task OpenAsync() => _connection.OpenAsync();
        /// <summary>
        /// Retrieves all rows from the card table.
        /// </summary>
        public async Task<ObservableCollection<CardViewModel>> GetAllCardsAsync()
        {
            try
            {
                ObservableCollection<CardViewModel> cards = new ObservableCollection<CardViewModel>();

                using var command = new MySqlCommand("SELECT * FROM card;", _connection);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CardViewModelConfig config = new CardViewModelConfig
                    {
                        Id = reader.GetInt32(0),
                        Date = reader.GetDateTime(1),
                        Title = reader.GetString(2),
                        Text = reader.GetString(3),
                        ConnectionString = _connectionString
                    };

                    cards.Add(new CardViewModel(config));
                }
                return cards;
            }
            catch (Exception exception)
            {
                ObservableCollection<CardViewModel> cards = new ObservableCollection<CardViewModel>();
                CardViewModelConfig config = new CardViewModelConfig()
                {
                    Date = DateTime.Now,
                    Id = -1,
                    Title = "Unable To Connect To Database, Please Try Again Later.",
                    Text = exception.Message,
                    ConnectionString = _connectionString
                };
                cards.Add(new CardViewModel(config));
                return cards;
            }
        }
        /// <summary>
        /// Retrieves rows from the links table related to selected card.
        /// </summary>
        public async Task<string[]> GetAllImagesForCardAsync(CardViewModel card)
        {
            using (var command = new MySqlCommand())
            {
                string[] linksToImage;

                command.Connection = _connection;
                command.CommandText = "SELECT link FROM image WHERE Id IN (SELECT `image-Id` FROM `card-to-image` WHERE `card-Id` = (@i));";
                command.Parameters.AddWithValue("i", card.Id);
                await using var reader = await command.ExecuteReaderAsync();
         
                List<string> linksToImageTemporary = new List<string>();

                while (await reader.ReadAsync())
                    linksToImageTemporary.Add(reader.GetString(0));

                linksToImage = linksToImageTemporary.ToArray();
                
                return linksToImage;
            }
        }
        /// <summary>
        /// Updates the selected card item to the card table.
        /// </summary>
        public async Task UpdateCardItemInDatabaseAsync(CardViewModel card)
        {
            try
            {
                using (var command = new MySqlCommand())
                {
                    command.Connection = _connection;
                    command.CommandText = "UPDATE card SET Date = (@d), Text = (@t), Title = (@u) WHERE Id = (@i);";
                    command.Parameters.AddWithValue("d", card.Date);
                    command.Parameters.AddWithValue("t", card.Text);
                    command.Parameters.AddWithValue("u", card.Title);
                    command.Parameters.AddWithValue("i", card.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch
            { 
                
            }
        }
        /// <summary>
        /// Add a new card to the card table.
        /// </summary>
        /// <param name="card"></param>
        public async Task CreateNewCardItemToDatabaseAsync(CardViewModel card)
        {
            try
            {
                using (var command = new MySqlCommand())
                {
                    command.Connection = _connection;
                    command.CommandText = "INSERT INTO card (Id, Date, Title, Text) VALUES ((@i), (@d), (@u), (@t));";
                    command.Parameters.AddWithValue("d", card.Date);
                    command.Parameters.AddWithValue("t", card.Text);
                    command.Parameters.AddWithValue("u", card.Title);
                    command.Parameters.AddWithValue("i", card.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch 
            {
                
            }
        }

        public async Task DeleteCardItemFromDatabaseAsync(CardViewModel cardViewModel)
        {
            try
            {
                using (var command = new MySqlCommand())
                {
                    command.Connection = _connection;
                    command.CommandText = "DELETE FROM card WHERE Id=(@i);";
                    command.Parameters.AddWithValue("i", cardViewModel.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch
            {

            }
        }
    }
}
