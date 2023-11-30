using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppEditBlog.DB
{
    public class MySqlConnectorRetriever
    {
        private static string _connectionString;
        public string ConnectionString
        {
            get 
            {
                return _connectionString;
            }
        }
        public MySqlConnectorRetriever()
        {
            _connectionString = Task.Run(() => LoadMySqlInformationAsync()).Result;
        }
        private static async Task<string> LoadMySqlInformationAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("MySqlInformation.txt");
            using var reader = new StreamReader(stream);

            var contents = reader.ReadToEnd();

            return contents;
        }
    }
}
