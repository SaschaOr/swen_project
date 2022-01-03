using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Security.Cryptography;

namespace MonsterTradingCardGame
{
    public class UserManagement
    {
        Database database = new Database();
        public User getNewUser()
        {
            Console.Write("Username: ");
            string name = Console.ReadLine();
            Console.Write("Password: ");
            string password = readPassword();
            User newUser = new User(name, password);
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
                    Console.WriteLine($"Successfully logged in as: {name}");
                    return userObject;
                }
            }
            conn = database.closeConnection();
            return null; 
        }

        public bool registerUser(string name, string password, int coins, int elo)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT user_id FROM \"user\" WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
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
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                string passwordHash = Convert.ToBase64String(hashBytes);

                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("password", passwordHash);
                cmd.Parameters.AddWithValue("coins", coins);
                cmd.Parameters.AddWithValue("elo", elo);

                Object responseInsert = cmd.ExecuteScalar();

                // check if insert is not correct
            }

            conn = database.closeConnection();
            return true;
        }

        public void updateElo(User user)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET elo = @elo WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("elo", user._elo);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        public void updateCoins(User user)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET coins = @coins WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("coins", user._coins);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

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

