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
        public List<Card> _stack = new List<Card>();
        public List<Card> _deck = new List<Card>();


        public int _userID { get; set; }
        public string _username { get; set; }
        public string _password { get; set; }
        public int _coins { get; set; }
        public int _elo { get; set; }

        // bot
        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public User(int userID, string username, string password, int coins, int elo)
        {
            _userID = userID;
            _username = username;
            _password = password;
            _coins = coins;
            _elo = elo;
        }

        // register new user
        public User(string username, string password, int coins, int elo)
        {
            _username = username;
            _password = password;
            _coins = coins;
            _elo = elo;
        }

        public void addCardToStack(Card card)
        {
            _stack.Add(card);
        }

        public void removeCardFromStack(Card card)
        {
            _stack.Remove(card);
        }

        public void addCardToDeck(Card card)
        {
            _deck.Add(card);
        }

        public void removeCardFromDeck(Card card)
        {
            _deck.Remove(card);
        }
        public void printCards()
        {
            foreach (Card card in _stack)
            {
                Console.WriteLine($"{card._cardName} is from type {card._cardType} and has element {card._elementType} with {card._damage} damage!");
            }

            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            foreach (Card card in _deck)
            {
                Console.WriteLine($"{card._cardName} is from type {card._cardType} and has element {card._elementType} with {card._damage} damage!");
            }
        }
    }
}
