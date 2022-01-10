using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    public class TradeManagement : Trade
    {
        private Database database = new Database();

        public void createTrade(User user)
        {
            CardType? cardType = null;
            int? minDamage = null;
            ElementType? type = null;

            Console.WriteLine("\nYou are able to trade the following cards:");

            for (int i = 0; i < user._stack.Count; i++)
            {
                Card currentCard = user._stack[i];
                Console.WriteLine($"{i + 1}. {currentCard._cardName} - Type: {currentCard._cardType.ToString()} - Element: {currentCard._elementType.ToString()} - Damage: {currentCard._damage}");
            }

            Console.Write("\nWhich card do you want to trade? Please select the index (take 0 to cancel): ");
            int index = getUserInput(user._stack.Count);

            if (index == 0)
                return;

            // subtract 1, because of list index
            Card cardToTrade = user._stack[index - 1];

            Console.WriteLine("\nWhich type of card do you want: ");

            foreach (CardType card in Enum.GetValues(typeof(CardType)))
            {
                Console.WriteLine($"    {((int)card + 1).ToString()}. {card.ToString()}");
            }

            Console.Write("\nYour choice: ");
            int typeChoice = getUserInput(Enum.GetNames(typeof(CardType)).Length);

            // starts at 0
            cardType = (CardType) typeChoice - 1;

            if(typeChoice == 0)
                return;

            Console.Write("\nDo you want to take a minimum damage (1) or a specific type (2): ");
            int choiceRequirementCount = 2;
            int damageType = getUserInput(choiceRequirementCount);

            if(damageType == 0)
                return;

            // min damage
            if (damageType == 1)
            {
                Console.Write("\nPlease select your minimum damage (1 - 100): ");
                minDamage = getUserInput(100);

                if(minDamage == 0)
                    return;
            }

            // element type
            if (damageType == 2)
            {
                Console.WriteLine("\nPossible types to choose:");

                foreach (ElementType elementType in Enum.GetValues(typeof(ElementType)))
                {
                    Console.WriteLine($"    {((int)elementType + 1).ToString()}. {elementType.ToString()}");
                }

                Console.Write("\nYour choice: ");
                // starts at 0
                int elementChoice = getUserInput(Enum.GetNames(typeof(ElementType)).Length);

                type = (ElementType) elementChoice - 1;

                if(elementChoice == 0)
                    return;
            }

            // save trade
            NpgsqlConnection conn = database.openConnection();
            NpgsqlCommand cmd;

            // min damage
            if (damageType == 1)
            {
                cmd = new NpgsqlCommand("INSERT INTO \"trade\" (from_user_id, card_id, card_type, min_damage) VALUES (@from_user_id, @card_id, @card_type, @min_damage);", conn);
                cmd.Parameters.AddWithValue("from_user_id", user._userID);
                cmd.Parameters.AddWithValue("card_id", cardToTrade._cardID);
                cmd.Parameters.AddWithValue("card_type", cardType.ToString());
                cmd.Parameters.AddWithValue("min_damage", minDamage);

                Object response = cmd.ExecuteScalar();
            }

            // element type
            if (damageType == 2)
            {
                cmd = new NpgsqlCommand("INSERT INTO \"trade\" (from_user_id, card_id, card_type, element_type) VALUES (@from_user_id, @card_id, @card_type, @element_type);", conn);
                cmd.Parameters.AddWithValue("from_user_id", user._userID);
                cmd.Parameters.AddWithValue("card_id", cardToTrade._cardID);
                cmd.Parameters.AddWithValue("card_type", cardType.ToString());
                cmd.Parameters.AddWithValue("element_type", type.ToString());

                Object response = cmd.ExecuteScalar();
            }

            // take card from current stack
            user.removeCardFromStack(cardToTrade);

            // set card in_trade to true
            cmd = new NpgsqlCommand("UPDATE \"card\" SET in_trade = @in_trade WHERE card_id = @card_id;", conn);
            cmd.Parameters.AddWithValue("in_trade", true);
            cmd.Parameters.AddWithValue("card_id", cardToTrade._cardID);

            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        public void showOpenTrades(User user)
        {
            List<Trade> openTrades = getOpenTrades(user);

            if (openTrades.Count == 0)
            {
                Console.WriteLine("\nThere are currently no open trades!");
                return;
            }
            else
            {
                for (int i = 0; i < openTrades.Count; i++)
                {
                    Trade tmp = openTrades[i];

                    // min damage
                    if (tmp._elementType == null)
                        Console.WriteLine(
                            $"{i + 1}. Card-Type: {tmp._cardType.ToString()} - Minimum damage: {tmp._minDamage} - From user: {getUsernameByUserID(tmp._fromUserID)}");

                    // element type
                    else
                        Console.WriteLine(
                            $"{i + 1}. Card-Type: {tmp._cardType.ToString()} - Element type: {tmp._elementType} - From user: {getUsernameByUserID(tmp._fromUserID)}");
                }
            }
        }

        private List<Trade> getOpenTrades(User user)
        {
            List<Trade> trades = new List<Trade>();

            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"trade\";", conn);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // don't show users own trade offers
                if ((int) dr[1] != user._userID)
                {
                    Trade tradeToAdd = new Trade(
                        (int) dr[0],
                        (int) dr[1],
                        (int) dr[2],
                        (CardType) Enum.Parse(typeof(CardType), (string) dr[3]),
                        (dr.IsDBNull(dr.GetOrdinal("element_type")) ? null : (ElementType) Enum.Parse(typeof(ElementType), (string) dr[4])),
                        (dr.IsDBNull(dr.GetOrdinal("min_damage")) ? null : (int) dr[5]));


                    trades.Add(tradeToAdd);
                }
            }

            conn = database.closeConnection();

            return trades;
        }

        public void acceptTrade(User user)
        {
            List<Trade> trades = getOpenTrades(user);

            Console.WriteLine("\nPlease select one of the following trades:");
            showOpenTrades(user);

            if (trades.Count == 0)
                return;

            Console.Write("\nYour choice (press 0 to cancel): ");
            int index = getUserInput(trades.Count);

            if (index == 0)
                return;

            // subtract 1, because of list index
            Trade tryTrade = trades[index - 1];

            List<Card> cardsToTrade = getCardsToTrade(user, tryTrade);

            if (cardsToTrade.Count == 0)
            {
                Console.WriteLine("\nYou don't have any matching cards for this trade!");
                return;
            }

            for (int i = 0; i < cardsToTrade.Count; i++)
            {
                Card currentCard = cardsToTrade[i];
                Console.WriteLine($"{i + 1}. {currentCard._cardName} - Type: {currentCard._cardType.ToString()} - Element: {currentCard._elementType.ToString()} - Damage: {currentCard._damage}");
            }

            Console.Write("\nSelect one of your matching cards to trade: ");
            int tradeIndex = getUserInput(cardsToTrade.Count);

            if (tradeIndex == 0)
                return;

            // subtract 1, because of list index
            Card tradeCard = cardsToTrade[tradeIndex - 1];

            // trade
            updateCardOwner(user._userID, tryTrade._cardID);
            updateCardOwner(tryTrade._fromUserID, tradeCard._cardID);

            // set card in_trade to false -> after trade
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"card\" SET in_trade = @in_trade WHERE card_id = @card_id;", conn);
            cmd.Parameters.AddWithValue("in_trade", false);
            cmd.Parameters.AddWithValue("card_id", tryTrade._cardID);

            cmd.ExecuteNonQuery();

            conn = database.closeConnection();

            // load stack for current user
            CardManagement cardManagement = new CardManagement();
            cardManagement.loadStackOfUser(user);

            // delete trade and put it into history
            saveTradeInHistory(tryTrade, user, tradeCard);
            deleteTrade(tryTrade);
            Console.WriteLine("\nSuccessfully traded cards!");

        }

        private void updateCardOwner(int userID, int cardID)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"card\" SET user_id = @user_id WHERE card_id = @card_id;", conn);
            cmd.Parameters.AddWithValue("user_id", userID);
            cmd.Parameters.AddWithValue("card_id", cardID);

            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        private List<Card> getCardsToTrade(User user, Trade trade)
        {
            List<Card> cardsToTrade = new List<Card>();

            foreach (Card card in user._stack)
            {
                // min damage is selected
                if (trade._minDamage != null)
                {
                    if (card._cardType == trade._cardType && card._damage >= trade._minDamage)
                    {
                        cardsToTrade.Add(card);
                    }
                }
                // element type is selected
                else
                {
                    if (card._cardType == trade._cardType && card._elementType == trade._elementType)
                    {
                        cardsToTrade.Add(card);
                    }
                }
            }

            return cardsToTrade;
        }

        public void deleteTrade(Trade trade)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM \"trade\" WHERE trade_id = @trade_id;", conn);
            cmd.Parameters.AddWithValue("trade_id", trade._tradeID);

            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        private void saveTradeInHistory(Trade trade, User user, Card cardTraded)
        {
            NpgsqlConnection conn = database.openConnection();
            NpgsqlCommand cmd;

            

            // min_damage
            if (trade._minDamage != null)
            {
                cmd = new NpgsqlCommand("INSERT INTO \"trade_history\" (trade_id, from_user_id, card_id, card_type, min_damage, trade_user_id, trade_card_id) VALUES (@trade_id, @from_user_id, @card_id, @card_type, @min_damage, @trade_user_id, @trade_card_id);", conn);
                cmd.Parameters.AddWithValue("min_damage", trade._minDamage);
                cmd.Parameters.AddWithValue("trade_id", trade._tradeID);
                cmd.Parameters.AddWithValue("from_user_id", trade._fromUserID);
                cmd.Parameters.AddWithValue("card_id", trade._cardID);
                cmd.Parameters.AddWithValue("trade_user_id", user._userID);
                cmd.Parameters.AddWithValue("trade_card_id", cardTraded._cardID);
                cmd.Parameters.AddWithValue("card_type", cardTraded._cardType.ToString());
                Object response = cmd.ExecuteScalar();
            }

            // element type
            if (trade._minDamage == null)
            {
                cmd = new NpgsqlCommand("INSERT INTO \"trade_history\" (trade_id, from_user_id, card_id, card_type, element_type, trade_user_id, trade_card_id) VALUES (@trade_id, @from_user_id, @card_id, @card_type, @element_type, @trade_user_id, @trade_card_id);", conn);
                cmd.Parameters.AddWithValue("element_type", trade._elementType.ToString());
                cmd.Parameters.AddWithValue("trade_id", trade._tradeID);
                cmd.Parameters.AddWithValue("from_user_id", trade._fromUserID);
                cmd.Parameters.AddWithValue("card_id", trade._cardID);
                cmd.Parameters.AddWithValue("trade_user_id", user._userID);
                cmd.Parameters.AddWithValue("trade_card_id", cardTraded._cardID);
                cmd.Parameters.AddWithValue("card_type", cardTraded._cardType.ToString());
                Object response = cmd.ExecuteScalar();
            }

            conn = database.closeConnection();
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

        private string getUsernameByUserID(int user_id)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT name FROM \"user\" WHERE user_id = @user_id;", conn);
            cmd.Parameters.AddWithValue("user_id", user_id);

            Object response = cmd.ExecuteScalar();

            conn = database.closeConnection();

            return (string)response;
        }
    }
}
