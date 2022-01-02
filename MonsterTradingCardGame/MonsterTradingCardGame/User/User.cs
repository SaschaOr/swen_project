﻿using System;
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
        private const int COINS_INITIAL = 20;
        private const int ELO_INITIAL = 100;

        public List<Card> _stack = new List<Card>();

        public int _userID { get; set; }
        public string _username { get; set; }
        public string _password { get; set; }
        public int _coins { get; set; } = COINS_INITIAL;
        public int _elo { get; set; } = ELO_INITIAL;

        // bot
        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public User(int userID, string username, string password)
        {
            _userID = userID;
            _username = username;
            _password = password;
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