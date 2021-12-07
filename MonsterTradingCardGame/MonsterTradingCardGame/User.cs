using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class User
    {
        private const string USERNAME = "test";
        private const string PASSWORD = "test";
        private const int _coinStartingValue = 20;

        public List<Card> _stack = new List<Card>();

        public int _userID { get; set; }
        public string _username { get; set; }
        public string _password { get; set; }
        public int _coins { get; set; } = _coinStartingValue;

        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool login(string username, string password)
        {
            return (username.Equals(USERNAME) && password.Equals(PASSWORD));
        }

        public void addCardToStack(Card card)
        {
            _stack.Add(card);
        }

        public void removeCardFromStack(Card card)
        {

        }

        public void printCards()
        {
            foreach (Card card in _stack)
            {
                Console.WriteLine($"{card._name} is from type {card._cardType} and has element {card._elementType} with {card._damage} damage!");
            }
        }
    }
}
