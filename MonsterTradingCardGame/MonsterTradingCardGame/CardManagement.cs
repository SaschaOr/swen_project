using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    public class CardManagement
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
                    int damage = random.Next(MAX_DAMAGE);

                    string cardName;

                    // if card is a monster card, it gets a specific monster type
                    if ((CardType)randomCardType == CardType.Monster)
                    {
                        MonsterType tmpMonster = (MonsterType) randomMonsterType;
                        ElementType tmpElement = (ElementType) randomElementType;
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
                    //Console.WriteLine($"The new crafted card {newCard._name} is of type {newCard._cardType} and has the element {newCard._elementType} with a damage of {newCard._damage}");

                    if (!userObject._username.Equals("Bot"))
                    {
                        Card newCard = saveCard(tmpCard, userObject);
                        userObject.addCardToStack(newCard);
                    }
                    else
                    {
                        userObject.addCardToStack(tmpCard);
                    }
                }

                _userManagement.updateCoins(userObject, PRICE_OF_PACKAGE);
                _userManagement.updateCoins(userObject, -PRICE_OF_PACKAGE);
                Console.WriteLine("Successfully bought a card package!");
            }
            else
            {
                Console.WriteLine("Packages can only be acquired by logged in users!");
            }
        }

        /*public void buyPackage(User userObject)
        {
            if (!userObject._username.Equals("Bot"))
            {
                userObject.addCardToStack(new Card(1, "Water Spell", 10, ElementType.Water, CardType.Spell, MonsterType.Spell));
                userObject.addCardToStack(new Card(1, "Water Kraken", 10, ElementType.Water, CardType.Monster, MonsterType.Kraken));
                userObject.addCardToStack(new Card(1, "Fire Elf", 10, ElementType.Fire, CardType.Monster, MonsterType.Elf));
                userObject.addCardToStack(new Card(1, "Normal Goblin", 10, ElementType.Normal, CardType.Monster, MonsterType.Goblin));
                userObject.addCardToStack(new Card(1, "Normal Ork", 10, ElementType.Normal, CardType.Monster, MonsterType.Ork));

            }
            else
            {
                userObject.addCardToStack(new Card(1, "Water Dragon", 10, ElementType.Water, CardType.Monster, MonsterType.Dragon));
                userObject.addCardToStack(new Card(1, "Water Wizzard", 10, ElementType.Water, CardType.Monster, MonsterType.Wizzard));
                userObject.addCardToStack(new Card(1, "Fire Knight", 10, ElementType.Fire, CardType.Monster, MonsterType.Knight));
                userObject.addCardToStack(new Card(1, "Normal Spell", 10, ElementType.Normal, CardType.Spell, MonsterType.Spell));
                userObject.addCardToStack(new Card(1, "Normal Dragon", 10, ElementType.Normal, CardType.Monster, MonsterType.Dragon));
            }
        }
        */

        private Card saveCard(Card card, User userObject)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"card\" (card_name, card_type, element_type, special_type, damage, user_id, in_deck) VALUES (@card_name, @card_type, @element_type, @special_type, @damage, @user_id, @in_deck) RETURNING card_id;", conn);
            cmd.Parameters.AddWithValue("card_name", card._cardName.ToString());
            cmd.Parameters.AddWithValue("card_type", card._cardType.ToString());
            cmd.Parameters.AddWithValue("element_type", card._elementType.ToString());
            cmd.Parameters.AddWithValue("special_type", card._specialType.ToString());
            cmd.Parameters.AddWithValue("damage", card._damage);
            cmd.Parameters.AddWithValue("user_id", userObject._userID);
            cmd.Parameters.AddWithValue("in_deck", false);

            // new card ID
            Object cardID = cmd.ExecuteScalar();

            conn = database.closeConnection();

            return new Card((int) cardID, card._cardName, card._damage, card._elementType, card._cardType, card._specialType);
        }

        public void loadStackOfUser(User user)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"card\" WHERE user_id = @user_id AND in_deck = @in_deck;", conn);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.Parameters.AddWithValue("in_deck", false);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Card cardToAdd = new Card(
                    (int) dr[0],
                    dr[1].ToString(),
                    (int) dr[5],
                    (ElementType) Enum.Parse(typeof(ElementType), (string) dr[3]),
                    (CardType) Enum.Parse(typeof(CardType), (string) dr[2]),
                    (MonsterType) Enum.Parse(typeof(MonsterType), (string) dr[4]));

                user.addCardToStack(cardToAdd);
            }

            conn = database.closeConnection();
        }

        public void loadDeckOfUser(User user)
        {
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

        public void setDeck(User user)
        {
            bool finished = false;

            while (!finished)
            {
                Console.WriteLine("Your current deck:");
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

                Console.WriteLine("Your cards, which can be taken into the deck:");
                Console.WriteLine("=============================================");

                for (int i = 0; i < user._stack.Count; i++)
                {
                    Card currentCard = user._stack[i];
                    Console.WriteLine($"{i + 1}. {currentCard._cardName} - Type: {currentCard._cardType.ToString()} - Element: {currentCard._elementType.ToString()} - Damage: {currentCard._damage}");
                }

                Console.WriteLine("To change cards, please state the position of the card in the deck and the stack");
                Console.Write("Deck: ");
                int deckPos = getUserInput(DECK_SIZE);
                Console.Write("Stack: ");
                int stackPos = getUserInput(user._stack.Count);

                // swap function returning status of swap to end while loop
            }
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

                    if (choice > 0 && choice <= maxSize)
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

        private void swapCardsInDeckAndStack()
        {

        }
    }
}
