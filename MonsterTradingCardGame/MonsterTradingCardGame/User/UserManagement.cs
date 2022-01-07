using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Security.Cryptography;

namespace MonsterTradingCardGame
{
    public class UserManagement
    {
        private const int COINS_INITIAL = 20;
        private const int ELO_INITIAL = 100;

        Database database = new Database();
        public User getNewUser()
        {
            Console.Write("Username: ");
            string name = Console.ReadLine();
            Console.Write("Password: ");
            string password = readPassword();
            User newUser = new User(name, password, COINS_INITIAL, ELO_INITIAL);
            return newUser;
        }

        public User loginUser()
        {
            Console.Write("Username: ");
            string name = Console.ReadLine();
            Console.Write("Password: ");
            string password = readPassword();

            // check if user exists in database
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT password FROM \"user\" WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            Object passwordResponse = cmd.ExecuteScalar();
            
            

            if (passwordResponse != null)
            {
                bool passwordCheck = true;

                // https://stackoverflow.com/questions/4181198/how-to-hash-a-password
                byte[] hashBytes = Convert.FromBase64String((string)passwordResponse);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
                byte[] hash = pbkdf2.GetBytes(20);
                // compare the results
                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        passwordCheck = false;

                if (passwordCheck)
                {
                    cmd = new NpgsqlCommand("SELECT * FROM \"user\" WHERE name = @name;", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    User userObject = null;

                    while (dr.Read())
                    {
                        userObject = new User((int) dr[0], (string) dr[1], password, (int) dr[3], (int) dr[4]);
                    }

                    conn = database.closeConnection();
                    Console.WriteLine($"Successfully logged in as: {userObject._username}");
                    Console.WriteLine($"TESTDATEN: {userObject._userID} - {userObject._username} - {userObject._password} - {userObject._coins} - {userObject._elo}");
                    return userObject;
                }
            }
            conn = database.closeConnection();
            return null; 
        }

        public bool registerUser(User newUser)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT user_id FROM \"user\" WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", newUser._username);
            Object response = cmd.ExecuteScalar();

            // user does not exist
            if (response == null)
            {
                // create new user
                cmd = new NpgsqlCommand("INSERT INTO \"user\" (name, password, coins, elo) VALUES (@name, @password, @coins, @elo);", conn);

                // hashing users password with salt value
                // https://stackoverflow.com/questions/4181198/how-to-hash-a-password
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                var pbkdf2 = new Rfc2898DeriveBytes(newUser._password, salt, 100000);
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                string passwordHash = Convert.ToBase64String(hashBytes);

                cmd.Parameters.AddWithValue("name", newUser._username);
                cmd.Parameters.AddWithValue("password", passwordHash);
                cmd.Parameters.AddWithValue("coins", newUser._coins);
                cmd.Parameters.AddWithValue("elo", newUser._elo);

                Object responseInsert = cmd.ExecuteScalar();

                // check if insert is not correct
            }

            conn = database.closeConnection();
            return true;
        }

        public void updateElo(User user, int eloChange)
        {
            NpgsqlConnection conn = database.openConnection();
            user._elo += eloChange;

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET elo = @elo WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("elo", user._elo);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        public void updateCoins(User user, int coinChange)
        {
            NpgsqlConnection conn = database.openConnection();
            user._coins += coinChange;

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET coins = @coins WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("coins", user._coins);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        public void printScoreboard()
        {
            Console.WriteLine("\nScoreboard:");
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT name, elo FROM \"user\" ORDER BY elo DESC;", conn);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            int counter = 1;
            while (dr.Read())
            {
                Console.WriteLine($"    {counter}. {dr[0]} - {dr[1]}");
                counter++;
            }

            conn = database.closeConnection();
        }

        // https://stackoverflow.com/questions/29201697/hide-replace-when-typing-a-password-c/29201791
        public string readPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
    }
}

