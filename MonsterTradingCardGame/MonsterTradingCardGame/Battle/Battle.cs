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
        private const int NO_DAMAGE = 0;
        private const int ELO_WIN = 3;
        private const int ELO_LOSS = -5;
        private const int COINS_AFTER_WIN = 1;
        private UserManagement _userManagement = new UserManagement();
        private EffectivenessDictionaries _dictionaries = new EffectivenessDictionaries();

         public void fight(User user1, User user2)
        {
            if (user1 != null && user2 != null)
            {
                // check if players have enough cards in their stack
                if (user1._deck.Count < DECK_SIZE || user2._deck.Count < DECK_SIZE)
                {
                    Console.WriteLine($"PLAYER {((user1._stack.Count < DECK_SIZE) ? 1 : 2)} has not enough cards in the stack to form a deck!");
                    return;
                }

                List<Card> deckUser1 = user1._deck;
                List<Card> deckUser2 = user2._deck;

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

                    Console.WriteLine($"Card 1:\n    Name: {cardUser1._cardName}\n    Element: {cardUser1._elementType}\n    Type: {cardUser1._cardType}\n    Damage: {cardUser1._damage}\n    New Damage: {calculatedDamageUser1}\n");
                    Console.WriteLine($"Card 2:\n    Name: {cardUser2._cardName}\n    Element: {cardUser2._elementType}\n    Type: {cardUser2._cardType}\n    Damage: {cardUser2._damage}\n    New Damage: {calculatedDamageUser2}\n");

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

                        // Player 2 won
                        if (deckUser1.Count <= 0)
                        {
                            _userManagement.updateElo(user1, ELO_LOSS);
                            _userManagement.updateElo(user2, ELO_WIN);
                            _userManagement.userLosesBattle(user1);
                            _userManagement.userWinsBattle(user2);
                            _userManagement.updateCoins(user2, COINS_AFTER_WIN);
                        }
                        // Player 1 won
                        else
                        {
                            _userManagement.updateElo(user2, ELO_LOSS);
                            _userManagement.updateElo(user1, ELO_WIN);
                            _userManagement.userLosesBattle(user2);
                            _userManagement.userWinsBattle(user1);
                            _userManagement.updateCoins(user1, COINS_AFTER_WIN);
                        }

                        break;
                    }

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

        public int calculateDamage(Card playerCard, Card opponentCard)
        {
            int damage = playerCard._damage;

            // special type fights
            if (checkSpecialType(playerCard, opponentCard))
            {
                Console.WriteLine($"SPECIAL FIGHT: {opponentCard._cardName} vs. {playerCard._cardName}");
                return NO_DAMAGE;
            }

            // pure monster fight -> damage does not change
            if (playerCard._cardType == CardType.Monster && opponentCard._cardType == CardType.Monster)
            {
                return damage;
            }

            // both normal type -> pure damage comparison
            if (playerCard._elementType == ElementType.Normal && opponentCard._elementType == ElementType.Normal)
            {
                return damage;
            }

            // increase or reduce damage based on the effectiveness list
            if (_dictionaries.spellEffectiveness[playerCard._elementType.ToString()].Equals(opponentCard._elementType.ToString()))
            {
                damage *= DOUBLE_CONSTANT;
            }

            if (_dictionaries.spellEffectiveness[opponentCard._elementType.ToString()].Equals(playerCard._elementType.ToString()))
            {
                damage /= DOUBLE_CONSTANT;
            }

            return damage;
        }

        public void transferCardFromDeckToDeck(List<Card> deckWinner, List<Card> deckLoser, Card cardToRemove)
        {
            for (int i = 0; i < deckLoser.Count; i++)
            {
                if (deckLoser[i]._cardID.Equals(cardToRemove._cardID))
                {
                    deckLoser.RemoveAt(i);
                    deckWinner.Add(cardToRemove);
                }
            }
        }

        private bool checkSpecialType(Card playerCard, Card opponentCard)
        {
            // special type in card name
            if (_dictionaries.specialTypeEffectiveness.ContainsKey(opponentCard._cardName))
            {
                if (_dictionaries.specialTypeEffectiveness[opponentCard._cardName].Equals(playerCard._cardName))
                {
                    return true;
                }
                if (_dictionaries.specialTypeEffectiveness[opponentCard._cardName].Equals(playerCard._specialType.ToString()))
                {
                    return true;
                }
            }

            // special type in type
            if (_dictionaries.specialTypeEffectiveness.ContainsKey(opponentCard._specialType.ToString()))
            {
                if (_dictionaries.specialTypeEffectiveness[opponentCard._specialType.ToString()].Equals(playerCard._cardName))
                {
                    return true;
                }
                if (_dictionaries.specialTypeEffectiveness[opponentCard._specialType.ToString()].Equals(playerCard._specialType.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkSpecialTypeValue(Card cardToTest)
        {
            if (_dictionaries.specialTypeEffectiveness.ContainsKey(cardToTest._cardName))
            {
                return true;
            }

            if (_dictionaries.specialTypeEffectiveness.ContainsKey(cardToTest._specialType.ToString()))
            {
                return true;
            }

            return false;
        }

        public void printCards(List<Card> list)
        {
            foreach (Card card in list)
            {
                Console.WriteLine($"{card._cardName} is from type {card._cardType} and has element {card._elementType} with {card._damage} damage!");
            }
        }
    }
}
