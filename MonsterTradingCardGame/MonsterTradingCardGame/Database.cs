using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    class Database
    {
        //private static Database _database = new Database();
        private static NpgsqlConnection _connection = new NpgsqlConnection("Server=localhost; User Id=postgres; Password=postgres; Database=postgres;");

        public NpgsqlConnection openConnection()
        {
            _connection.Open();
            return _connection;
        }

        public NpgsqlConnection closeConnection()
        {
            _connection.Close();
            return _connection;
        }
    }
}
