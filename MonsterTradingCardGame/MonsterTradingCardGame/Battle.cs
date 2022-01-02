using System;
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
            if (user1 != null && user2 != null)
            {
                // check if players have enough cards in their stack
                if (user1._stack.Count < DECK_SIZE || user2._stack.Count < DECK_SIZE)
                {
                    Console.WriteLine($"PLAYER {((user1._stack.Count < DECK_SIZE) ? 1 : 2)} has not enough cards in the stack to form a deck!");
                    return;
                }

                // create decks = 4 best cards of each user
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

                    Console.WriteLine($"Card 1:\n    Element: {cardUser1._elementType}\n    Type: {cardUser1._cardType}\n    Damage: {cardUser1._damage}\n    Name: {cardUser1._name}\n    New Damage: {calculatedDamageUser1}\n");
                    Console.WriteLine($"Card 2:\n    Element: {cardUser2._elementType}\n    Type: {cardUser2._cardType}\n    Damage: {cardUser2._damage}\n    Name: {cardUser2._name}\n    New Damage: {calculatedDamageUser2}\n");

                    // higher damage wins and gets the opponents card
                    if (calculatedDamageUser1 > calculatedDamageUser2)
                    {
                        Console.WriteLine("PLAYER 1 won this round");
                        transferCardFromDeckToDeck(deckUser1, deckUser2, cardUser2);
                    }
                    else if (calculatedDamageUser2 > calculatedDamageUser1)
                    {
                        Console.WriteLine("PLAYER 2 won this round");
                        transferCardFromDeckToDeck(deckUser2, deckUser1, cardUser1);
                    }
                    else
                    {
                        Console.WriteLine("DRAW, nothing happens this round");
                    }

                    if (deckUser1.Count <= 0 || deckUser2.Count <= 0)
                    {
                        Console.WriteLine("==========================================================");
                        Console.WriteLine($"PLAYER {((deckUser1.Count <= 0) ? 2 : 1)} won this battle!\nCongrats!");
                        Console.WriteLine("==========================================================");
                        break;
                    }

                    //printCards(deckUser1);
                    //Console.WriteLine("---------------------------");
                    //printCards(deckUser2);

                    Console.WriteLine("----------------------------------------------------------");
                    roundCounter++;
                }
            }
            else
            {
                Console.WriteLine("For taking a battle, you must login first!");
            }

        }

        private List<Card> getDeck(User userObject)
        {
            List<Card> tmpList = new List<Card>();
            List<Card> returnList = new List<Card>();
            // list is sorted ascending
            tmpList = userObject._stack.OrderBy(card => card._damage).ToList();

            int cardCount = userObject._stack.Count - 1;

            // last cards are the best -> make it the deck
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
            if (playerCard._cardType == CardType.Monster && opponentCard._cardType == CardType.Monster)
            {
                return damage;
            }

            if (playerCard._elementType == ElementType.Water && opponentCard._elementType == ElementType.Fire)
            {
                damage *= DOUBLE_CONSTANT;
            }

            if (playerCard._elementType == ElementType.Fire && opponentCard._elementType == ElementType.Water)
            {
                damage /= DOUBLE_CONSTANT;
            }

            // both normal type -> pure damage comparison
            if (playerCard._elementType == ElementType.Normal && opponentCard._elementType == ElementType.Normal)
            {
                return damage;
            }

            return damage;
        }

        public void transferCardFromDeckToDeck(List<Card> deckWinner, List<Card> deckLoser, Card cardToRemove)
        {
            for (int i = 0; i < deckLoser.Count; i++)
            {
                if (deckLoser[i]._name.Equals(cardToRemove._name))
                {
                    deckLoser.RemoveAt(i);
                    deckWinner.Add(cardToRemove);
                }
            }
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
