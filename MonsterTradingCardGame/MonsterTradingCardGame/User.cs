using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    class User
    {
        private const int _coinStartingValue = 20;
        private const int _priceOfPackage = 5;


        private int _userID { get; set; }
        private string _username { get; set; }
        private string _password { get; set; }
        private int _coins { get; set; } = _coinStartingValue;

        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        
    }
}
