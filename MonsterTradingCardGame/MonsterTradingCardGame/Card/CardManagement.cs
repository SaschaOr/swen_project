using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    public class CardManagement : Card
    {
        private const int PRICE_OF_PACKAGE = 5;
        private const int CARDS_PER_NEW_PACKAGE = 5;
        private const int MAX_DAMAGE = 100;
        private const int DECK_SIZE = 4;

        private UserManagement _userManagement = new UserManagement();
        Database database = new Database();

        public void buyPackage(User userObject)
        {
            if (userObject != null)
            {
                if (PRICE_OF_PACKAGE > userObject._coins)
                {
                    Console.WriteLine("You have not enough coins to buy a card package");
                    return;
                }

                // generate new card
                int cardTypeCount = Enum.GetNames(typeof(CardType)).Length;
                int elementTypeCount = Enum.GetNames(typeof(ElementType)).Length;
                int monsterTypeCount = Enum.GetNames(typeof(MonsterType)).Length;

                for (int i = 0; i < CARDS_PER_NEW_PACKAGE; i++)
                {
                    Random random = new Random();
                    int randomCardType = random.Next(cardTypeCount);
                    int randomElementType = random.Next(elementTypeCount);
                    // subtract 1, because spell can't be a monster type
                    int randomMonsterType = random.Next(monsterTypeCount - 1);
                    // add 1, because the damage must be at least 1 or at most 100
                    int damage = random.Next(MAX_DAMAGE) + 1;

                    string cardName;

                    // if card is a monster card, it gets a specific monster type
                    if ((CardType)randomCardType == CardType.Monster)
                    {
                        MonsterType tmpMonster = (MonsterType)randomMonsterType;
                        ElementType tmpElement = (ElementType)randomElementType;
                        cardName = tmpElement.ToString() + " " + tmpMonster.ToString();
                    }
                    else
                    {
                        ElementType tmpElement = (ElementType)randomElementType;
                        randomMonsterType = -1;
                        cardName = tmpElement.ToString() + " Spell";
                    }

                    // card has no ID -> after insert statement
                    Card tmpCard = new Card(cardName, damage, (ElementType)randomElementType, (CardType)randomCardType, (MonsterType)randomMonsterType);

                    // save card and get new one with id
                    Card newCard = saveCard(tmpCard, userObject);
                    userObject.addCardToStack(newCard);
                }

                _userManagement.updateCoins(userObject, -PRICE_OF_PACKAGE);
                _userManagement.updateCoinsSpent(userObject, PRICE_OF_PACKAGE);
                Console.WriteLine("Successfully bought a card package!");
            }
            else
            {
                Console.WriteLine("Packages can only be acquired by logged in users!");
            }
        }

        private Card saveCard(Card card, User userObject)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"card\" (card_name, card_type, element_type, special_type, damage, user_id, in_deck, in_trade) VALUES (@card_name, @card_type, @element_type, @special_type, @damage, @user_id, @in_deck, @in_trade) RETURNING card_id;", conn);
            cmd.Parameters.AddWithValue("card_name", card._cardName.ToString());
            cmd.Parameters.AddWithValue("card_type", card._cardType.ToString());
            cmd.Parameters.AddWithValue("element_type", card._elementType.ToString());
            cmd.Parameters.AddWithValue("special_type", card._specialType.ToString());
            cmd.Parameters.AddWithValue("damage", card._damage);
            cmd.Parameters.AddWithValue("user_id", userObject._userID);
            cmd.Parameters.AddWithValue("in_deck", false);
            cmd.Parameters.AddWithValue("in_trade", false);

            // new card ID
            Object cardID = cmd.ExecuteScalar();

            conn = database.closeConnection();

            return new Card((int)cardID, card._cardName, card._damage, card._elementType, card._cardType, card._specialType);
        }

        public void loadStackOfUser(User user)
        {
            // clear stack
            user._stack.Clear();

            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"card\" WHERE user_id = @user_id AND in_deck = @in_deck AND in_trade = @in_trade;", conn);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.Parameters.AddWithValue("in_deck", false);
            cmd.Parameters.AddWithValue("in_trade", false);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Card cardToAdd = new Card(
                    (int)dr[0],
                    dr[1].ToString(),
                    (int)dr[5],
                    (ElementType)Enum.Parse(typeof(ElementType), (string)dr[3]),
                    (CardType)Enum.Parse(typeof(CardType), (string)dr[2]),
                    (MonsterType)Enum.Parse(typeof(MonsterType), (string)dr[4]));

                user.addCardToStack(cardToAdd);
            }

            conn = database.closeConnection();
        }

        public void loadDeckOfUser(User user)
        {
            // clear deck
            user._deck.Clear();

            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"card\" WHERE user_id = @user_id AND in_deck = @in_deck;", conn);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.Parameters.AddWithValue("in_deck", true);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Card cardToAdd = new Card(
                    (int)dr[0],
                    dr[1].ToString(),
                    (int)dr[5],
                    (ElementType)Enum.Parse(typeof(ElementType), (string)dr[3]),
                    (CardType)Enum.Parse(typeof(CardType), (string)dr[2]),
                    (MonsterType)Enum.Parse(typeof(MonsterType), (string)dr[4]));
                user.addCardToDeck(cardToAdd);
            }

            conn = database.closeConnection();
        }

        public void showStack(User user)
        {
            EffectivenessDictionaries _dictionaries = new EffectivenessDictionaries();

            Console.WriteLine("\nYour are the owner of following cards:");
            Console.WriteLine("======================================");

            for (int i = 0; i < user._stack.Count; i++)
            {
                Card currentCard = user._stack[i];
                Console.Write($"{i + 1}. {currentCard._cardName}\n" +
                                  $"      Type: {currentCard._cardType.ToString()}\n" +
                                  $"      Element: {currentCard._elementType.ToString()}\n" +
                                  $"      Damage: {currentCard._damage}\n" +
                                  $"      Special Effectiveness: ");

                if (_dictionaries.specialTypeEffectiveness.ContainsKey(currentCard._cardName))
                {

                    Console.Write($"{currentCard._cardName} wins against {_dictionaries.specialTypeEffectiveness[currentCard._cardName]}");
                }

                else if (_dictionaries.specialTypeEffectiveness.ContainsKey(currentCard._specialType.ToString()))
                {
                    Console.Write($"{currentCard._specialType.ToString()} wins against {_dictionaries.specialTypeEffectiveness[currentCard._specialType.ToString()]}");
                }

                else
                {
                    Console.Write("None");
                }

                Console.WriteLine("\n");
            }
        }

        public void showDeck(User user)
        {
            EffectivenessDictionaries _dictionaries = new EffectivenessDictionaries();

            Console.WriteLine("\nYour deck consists of the following cards:");
            Console.WriteLine("============================================");

            for (int i = 0; i < DECK_SIZE; i++)
            {
                if (i + 1 > user._deck.Count)
                {
                    Console.WriteLine($"{i + 1}. Empty");
                }
                else
                {
                    Card currentCard = user._deck[i];
                    Console.Write($"{i + 1}. {currentCard._cardName}\n" +
                                  $"      Type: {currentCard._cardType.ToString()}\n" +
                                  $"      Element: {currentCard._elementType.ToString()}\n" +
                                  $"      Damage: {currentCard._damage}\n" +
                                  $"      Special Effectiveness: ");

                    if (_dictionaries.specialTypeEffectiveness.ContainsKey(currentCard._cardName))
                    {

                        Console.Write(
                            $"{currentCard._cardName} wins against {_dictionaries.specialTypeEffectiveness[currentCard._cardName]}");
                    }

                    else if (_dictionaries.specialTypeEffectiveness.ContainsKey(currentCard._specialType.ToString()))
                    {
                        Console.Write(
                            $"{currentCard._specialType.ToString()} wins against {_dictionaries.specialTypeEffectiveness[currentCard._specialType.ToString()]}");
                    }

                    else
                    {
                        Console.Write("None");
                    }

                    Console.WriteLine("\n");
                }
            }
        }

        public void setDeck(User user)
        {
            Console.WriteLine("\nYour current deck:");
            Console.WriteLine("==================");

            for (int i = 0; i < DECK_SIZE; i++)
            {
                if (i + 1 > user._deck.Count)
                {
                    Console.WriteLine($"{i + 1}. Empty");
                }
                else
                {
                    Card currentCard = user._deck[i];
                    Console.WriteLine($"{i + 1}. {currentCard._cardName} - Type: {currentCard._cardType.ToString()} - Element: {currentCard._elementType.ToString()} - Damage: {currentCard._damage}");
                }
            }

            Console.WriteLine("\nYour cards, which can be taken into the deck:");
            Console.WriteLine("=============================================");

            for (int i = 0; i < user._stack.Count; i++)
            {
                Card currentCard = user._stack[i];
                Console.WriteLine($"{i + 1}. {currentCard._cardName} - Type: {currentCard._cardType.ToString()} - Element: {currentCard._elementType.ToString()} - Damage: {currentCard._damage}");
            }

            Console.WriteLine("\nTo change cards, please state the position of the card in the deck and the stack (press 0 to leave the selection)");
            Console.Write("Deck: ");
            int deckPos = getUserInput(DECK_SIZE);
            if (deckPos == 0)
                return;

            Console.Write("Stack: ");
            int stackPos = getUserInput(user._stack.Count);
            if (stackPos == 0)
                return;

            // subtract one at the positions, because of array index
            swapCardsInDeckAndStack(user, deckPos - 1, stackPos - 1);
        }

        private int getUserInput(int maxSize)
        {
            int choice = 0;
            bool acceptedValue = false;

            while (!acceptedValue)
            {
                try
                {
                    string value = Console.ReadLine();
                    choice = Convert.ToInt32(value);

                    if (choice >= 0 && choice <= maxSize)
                        acceptedValue = true;
                    else
                        Console.WriteLine("Your input is out of range! Try again.");
                }
                catch
                {
                    Console.WriteLine("Something went wrong with your input! Try again.");
                }
            }

            return choice;
        }

        private void swapCardsInDeckAndStack(User user, int deckPos, int stackPos)
        {
            // deck is probably empty
            Card deck = null;
            Card stack = user._stack[stackPos];

            // user can take a position, that is empty -> check size of deck
            if (user._deck.Count > deckPos)
            {
                deck = user._deck[deckPos];
            }

            // swap cards
            user.removeCardFromStack(stack);
            user.addCardToDeck(stack);

            if (deck != null)
            {
                user.removeCardFromDeck(deck);
                user.addCardToStack(deck);
            }

            changeDeckStatus(user, deck, stack);
            Console.WriteLine("\nYour stack and deck has been updated!");
        }

        private void changeDeckStatus(User user, Card fromDeckToStack, Card fromStacktoDeck)
        {
            NpgsqlConnection conn = database.openConnection();

            // change card from stack to deck
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"card\" SET in_deck = @in_deck WHERE user_id = @user_id AND card_id = @card_id;", conn);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.Parameters.AddWithValue("card_id", fromStacktoDeck._cardID);
            cmd.Parameters.AddWithValue("in_deck", true);

            Object response = cmd.ExecuteNonQuery();

            // deck can be empty
            if (fromDeckToStack != null)
            {
                // change card from deck to stack
                cmd = new NpgsqlCommand("UPDATE \"card\" SET in_deck = @in_deck WHERE user_id = @user_id AND card_id = @card_id;", conn);
                cmd.Parameters.AddWithValue("user_id", user._userID);
                cmd.Parameters.AddWithValue("card_id", fromDeckToStack._cardID);
                cmd.Parameters.AddWithValue("in_deck", false);

                response = cmd.ExecuteNonQuery();
            }

            conn = database.closeConnection();
        }
    }
}
