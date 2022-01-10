using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame
{
    public class Friendship : User
    {
        Database database = new Database();

        private List<string> searchUser(User searchingUser)
        {
            List<string> userList = new List<string>();

            Console.Write("\nPlease enter the username, you want to search for: ");
            string searchTerm = Console.ReadLine();

            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT name FROM \"user\" WHERE name ILIKE @searchTerm;", conn);
            cmd.Parameters.AddWithValue("searchTerm", "%" + searchTerm + "%");

            NpgsqlDataReader dr = cmd.ExecuteReader();

            int counter = 1;

            while (dr.Read())
            {
                // can't add your own user as friend
                if (!searchingUser._username.Equals((string) dr[0]))
                {
                    userList.Add((string) dr[0]);
                    Console.WriteLine($"    {counter}. {dr[0]}");
                    counter++;
                }
            }

            conn = database.closeConnection();

            return userList;
        }

        public void makeFriendRequest(User fromUser)
        {
            List<string> userList = searchUser(fromUser);

            // no such user found
            if (userList.Count == 0)
            {
                Console.WriteLine("\nThere is no such user, please retry!");
                return;
            }

            Console.Write("\nPlease enter the ID of the user you want to add (take 0 to cancel): ");
            int id = getUserInput(userList.Count);

            if(id == 0)
                return;

            // subtract one, because of list index
            int friend_user_id = getUserIDByUsername(userList[id - 1]);

            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"friendship\" (user_id, friend_user_id, friendship_accepted) VALUES (@user_id, @friend_user_id, @friendship_accepted);", conn);
            cmd.Parameters.AddWithValue("user_id", fromUser._userID);
            cmd.Parameters.AddWithValue("friend_user_id", friend_user_id);
            cmd.Parameters.AddWithValue("friendship_accepted", false);

            Object response = cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        public void acceptFriendRequest(User fromUser)
        {
            List<int> userList = openFriendRequests(fromUser);

            if (userList.Count == 0)
            {
                Console.WriteLine("\nYou don't have any pending friend requests at the moment!");
                return;
            }
            else
            {
                Console.WriteLine("\nOpen friend requests, that can be accepted:");
                for (int i = 0; i < userList.Count; i++)
                {
                    string username = getUsernameByUserID(userList[i]);

                    Console.WriteLine($"    {i + 1}. {username}");
                }
            }

            Console.Write("\nPlease enter the ID of the friend request you want to add (take 0 to cancel): ");
            int id = getUserInput(userList.Count);

            if (id == 0)
                return;

            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"friendship\" SET friendship_accepted = @friendship_accepted WHERE user_id = @user_id AND friend_user_id = @friend_user_id;", conn);
            cmd.Parameters.AddWithValue("friendship_accepted", true);
            cmd.Parameters.AddWithValue("user_id", userList[id - 1]);
            cmd.Parameters.AddWithValue("friend_user_id", fromUser._userID);

            Object response = cmd.ExecuteNonQuery();

            conn = database.closeConnection();

            Console.WriteLine("\nSuccessfully added friend to your friend list!");
        }

        private List<int> openFriendRequests(User fromUser)
        {
            List<int> userList = new List<int>();
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT user_id FROM \"friendship\" WHERE friend_user_id = @friend_user_id AND friendship_accepted = @friendship_accepted;", conn);
            cmd.Parameters.AddWithValue("friend_user_id", fromUser._userID);
            cmd.Parameters.AddWithValue("friendship_accepted", false);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                userList.Add((int)dr[0]);
            }

            conn = database.closeConnection();
            return userList;
        }

        public void deleteFriend(User fromUser, string friendToDelete)
        {

        }

        private string getFriendToPlayAgainst(User user, int friend_id)
        {
            List<int> userList = new List<int>();
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT friend_user_id FROM \"friendship\" WHERE user_id = @user_id AND 3 < (SELECT COUNT(*) FROM \"card\" WHERE user_id = @friend_user_id AND in_deck = true);", conn);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.Parameters.AddWithValue("friend_user_id", friend_id);

            Object response = cmd.ExecuteScalar();

            conn = database.closeConnection();

            if (response == null)
            {
                return null;
            }

            return getUsernameByUserID((int) response);
        }

        private List<int> getFriends(User user)
        {
            List<int> userList = new List<int>();
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT friend_user_id FROM \"friendship\" WHERE user_id = @user_id AND friendship_accepted = @friendship_accepted;", conn);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.Parameters.AddWithValue("friendship_accepted", true);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                userList.Add((int)dr[0]);
            }

            conn = database.closeConnection();
            return userList;
        }

        public User playAgainstUser(User user)
        {
            List<int> friends = getFriends(user);
            List<string> friendsToPlayAgainst = new List<string>();

            Console.WriteLine("\nYour friends, that have a full deck:");

            if (friends.Count == 0)
            {
                Console.WriteLine("\nYou have no friends at the moment!");
                return null;
            }

            for (int i = 0; i < friends.Count; i++)
            {
                string username = getFriendToPlayAgainst(user, friends[i]);

                if (username != null)
                {
                    Console.WriteLine($"    {i + 1}. {username}");
                    friendsToPlayAgainst.Add(username);
                }
            }

            if (friendsToPlayAgainst.Count == 0)
            {
                Console.WriteLine("\nThere are no friends, which have enough cards in their deck!");
                return null;
            }

            Console.Write("\nPlease select one of the following friends, to play against (take 0 to cancel): ");
            int id = getUserInput(friendsToPlayAgainst.Count);

            if (id == 0)
                return null;

            int user_id = getUserIDByUsername(friendsToPlayAgainst[id - 1]);

            return new User(user_id, friendsToPlayAgainst[id - 1]);
        }

        private int getUserIDByUsername(string username)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT user_id FROM \"user\" WHERE name = @username;", conn);
            cmd.Parameters.AddWithValue("username", username);

            Object response = cmd.ExecuteScalar();

            conn = database.closeConnection();

            return (int) response;
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
    }
}
