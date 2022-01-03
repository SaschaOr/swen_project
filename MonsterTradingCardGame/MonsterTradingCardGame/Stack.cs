using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    public class Stack
    {
        private const int PRICE_OF_PACKAGE = 5;
        private const int CARDS_PER_NEW_PACKAGE = 5;
        private const int MAX_DAMAGE = 100;

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
                    int randomMonsterType = random.Next(monsterTypeCount);
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
                        cardName = tmpElement.ToString() + " Spell";
                    }

                    // card has no ID -> after insert statement
                    Card tmpCard = new Card(cardName, damage, (ElementType)randomElementType, (CardType)randomCardType);
                    //Console.WriteLine($"The new crafted card {newCard._name} is of type {newCard._cardType} and has the element {newCard._elementType} with a damage of {newCard._damage}");

                    if (!userObject._username.Equals("Bot"))
                    {
                        Card newCard;
                        newCard = saveCard(tmpCard, userObject);
                        userObject.addCardToStack(newCard);
                    }
                    else
                    {
                        userObject.addCardToStack(tmpCard);
                    }
                }

                userObject._coins -= PRICE_OF_PACKAGE;
                Console.WriteLine("Successfully bought a card package!");
            }
            else
            {
                Console.WriteLine("Packages can only be acquired by logged in users!");
            }
        }

        private Card saveCard(Card card, User userObject)
        {
            Database database = new Database();
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"card\" (card_name, card_type, element_type, damage, user_id) VALUES (@card_name, @card_type, @element_type, @damage, @user_id) RETURNING card_id;", conn);
            cmd.Parameters.AddWithValue("card_name", card._cardName.ToString());
            cmd.Parameters.AddWithValue("card_type", card._cardType.ToString());
            cmd.Parameters.AddWithValue("element_type", card._elementType.ToString());
            cmd.Parameters.AddWithValue("damage", card._damage);
            cmd.Parameters.AddWithValue("user_id", userObject._userID);

            // new card ID
            Object response = cmd.ExecuteScalar();

            Console.WriteLine($"Card ID: {response}");

            //cmd = new NpgsqlCommand("INSERT INTO \"user_card\" (cardtype, elementtype, damage) VALUES (@cardtype, @elementtype, @damage);", conn);

            conn = database.closeConnection();

            return new Card((int) response, card._cardName, card._damage, card._elementType, card._cardType);
        }
    }
}
