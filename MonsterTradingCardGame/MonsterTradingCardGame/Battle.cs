﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Battle
    {
        private const int DECK_SIZE = 4;
        private const int MAX_ROUNDS = 100;
        private const int DOUBLE_CONSTANT = 2;

        public void fight(User user1, User user2)
        {
            //create decks = 4 best cards of each user
            List<Card> deckUser1 = getDeck(user1);
            List<Card> deckUser2 = getDeck(user2);

            printCards(deckUser1);
            Console.WriteLine("---------------------------");
            printCards(deckUser2);

            int roundCounter = 0;

            while (roundCounter < MAX_ROUNDS)
            {
                Console.WriteLine($"Round {roundCounter + 1} starts!");
                Console.WriteLine("----------------------------------------------------------");

                // draw one card from each deck
                Random random = new Random();

                int randomCardUser1 = random.Next(deckUser1.Count - 1);
                int randomCardUser2 = random.Next(deckUser2.Count - 1);

                Card cardUser1 = deckUser1[randomCardUser1];
                Card cardUser2 = deckUser2[randomCardUser2];

                // calculate damage based on the card damage, type and element
                int calculatedDamageUser1 = calculateDamage(cardUser1, cardUser2);
                int calculatedDamageUser2 = calculateDamage(cardUser2, cardUser1);

                // higher damage wins and gets the opponents card
                if (calculatedDamageUser1 > calculatedDamageUser2)
                {
                    Console.WriteLine("PLAYER 1 won this round");
                }
                else if (calculatedDamageUser2 > calculatedDamageUser1)
                {
                    Console.WriteLine("PLAYER 2 won this round");
                }
                else
                {
                    Console.WriteLine("DRAW, nothing happens this round");
                }

                if (deckUser1.Count <= 0 || deckUser2.Count <= 0)
                {
                    break;
                }

                Console.WriteLine("----------------------------------------------------------");
                roundCounter++;
            }

        }

        private List<Card> getDeck(User userObject)
        {
            List<Card> tmpList = new List<Card>();
            List<Card> returnList = new List<Card>();
            tmpList = userObject._stack.OrderBy(card => card._damage).ToList();

            int cardCount = userObject._stack.Count - 1;

            for (int i = cardCount; i > (cardCount - DECK_SIZE); i--)
            {
                returnList.Add(tmpList[i]);
            }

            return returnList;
        }

        private int calculateDamage(Card playerCard, Card opponentCard)
        {
            int damage = playerCard._damage;

            // pure monster fight -> damage does not change
            if (playerCard._cardType == CardType.monster && opponentCard._cardType == CardType.monster)
            {
                return damage;
            }

            if (playerCard._elementType == ElementType.water && opponentCard._elementType == ElementType.fire)
            {
                damage *= DOUBLE_CONSTANT;
            }

            if (playerCard._elementType == ElementType.fire && opponentCard._elementType == ElementType.water)
            {
                damage /= DOUBLE_CONSTANT;
            }

            // both normal type -> pure damage comparison
            if (playerCard._elementType == ElementType.normal && opponentCard._elementType == ElementType.normal)
            {
                return damage;
            }

            return damage;
        }

        public void printCards(List<Card> list)
        {
            foreach (Card card in list)
            {
                Console.WriteLine($"{card._name} is from type {card._cardType} and has element {card._elementType} with {card._damage} damage!");
            }
        }
    }
}