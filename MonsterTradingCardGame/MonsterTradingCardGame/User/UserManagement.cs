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
        private const int COINS_SPENT_INITIAL = 0;
        private const int ELO_INITIAL = 100;
        private const int WIN_INITIAL = 0;
        private const int LOSE_INITIAL = 0;

        Database database = new Database();
        public User getNewUser()
        {
            Console.Write("Username: ");
            string name = Console.ReadLine();
            Console.Write("Password: ");
            string password = readPassword();

            if (name.Equals("") || password.Equals(""))
            {
                Console.WriteLine("\nYou need to fill in any value! Please repeat.");
                return null;
            }

            User newUser = new User(name, password, COINS_INITIAL, COINS_SPENT_INITIAL, ELO_INITIAL, WIN_INITIAL, LOSE_INITIAL);
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
                        userObject = new User((int) dr[0], (string) dr[1], password, (int) dr[3], (int) dr[7], (int) dr[4], (int) dr[5], (int) dr[6]);
                    }

                    conn = database.closeConnection();
                    Console.WriteLine($"Successfully logged in as: {userObject._username}");
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

        public void updateCoinsSpent(User user, int coinChange)
        {
            NpgsqlConnection conn = database.openConnection();
            user._coinsSpent += coinChange;

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET coins_spent = @coins_spent WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("coins_spent", user._coinsSpent);
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

        public void editProfilPage(User user)
        {
            printProfilePage(user);

            Console.Write("\nDo you want to change your username (1) or your password (2) - press (0) to cancel: ");
            int choiceCount = 2;
            int choice = getUserInput(choiceCount);
            Status status = Status.SUCCESS;

            if (choice == 0)
                return;

            if(choice == 1)
                status = changeUsername(user);

            if(choice == 2)
                status = changePassword(user);

            if(status == Status.SUCCESS)
                Console.WriteLine("\nSuccessfully updated your profile!");
        }

        public void printProfilePage(User user)
        {
            Console.WriteLine("\nYour profile:");
            Console.WriteLine($"    Username: {user._username}");
            Console.WriteLine($"    Coins: {user._coins}");
            Console.WriteLine($"    Coins spent: {user._coinsSpent}");
            Console.WriteLine($"    Elo: {user._elo}");
            Console.WriteLine($"    Games played: {user._win + user._lose}");
            Console.WriteLine($"    Wins: {user._win}");
            Console.WriteLine($"    Losses: {user._lose}");

            double winLoseRation = 0;

            if (user._lose != 0)
            {
                winLoseRation = (double) user._win / user._lose;
                Math.Round(winLoseRation, 4);
                winLoseRation *= 100;

            }

            Console.WriteLine($"    W/L-Ratio: {(user._win == 0 && user._lose == 0 ? "-" : winLoseRation.ToString("0.##") + "%")}");
            
        }

        private Status changePassword(User user)
        {
            Console.Write("\nEnter your new password: ");
            string password = readPassword();
            Console.Write("Please repeat your new password: ");
            string passwordRepeat = readPassword();

            // fail
            if (password.Equals("") || passwordRepeat.Equals(""))
            {
                Console.WriteLine("\nPlease enter any value! Please repeat.");
                return Status.FAILURE;
            }

            if (!password.Equals(passwordRepeat))
            {
                Console.WriteLine("\nYour passwords don't match! Please repeat.");
                return Status.FAILURE;
            }

            NpgsqlConnection conn = database.openConnection();

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

            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET password = @password WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("password", passwordHash);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

            conn = database.closeConnection();

            return Status.SUCCESS;
        }

        private Status changeUsername(User user)
        {
            Console.Write("\nEnter new username: ");
            string input = Console.ReadLine();

            // fail
            if (input.Equals(""))
            {
                Console.WriteLine("\nPlease enter any value! Please repeat.");
                return Status.FAILURE;
            }

            if (input.Equals(user._username))
            {
                Console.WriteLine("\nThat's your current username! Please repeat.");
                return Status.FAILURE;
            }

            NpgsqlConnection conn = database.openConnection();

            // check if username already exists
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT user_id FROM \"user\" WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", input);
            Object response = cmd.ExecuteScalar();

            // username does not exist
            if (response == null)
            {
                cmd = new NpgsqlCommand("UPDATE \"user\" SET name = @name WHERE user_id = @user_id", conn);
                cmd.Parameters.AddWithValue("name", input);
                cmd.Parameters.AddWithValue("user_id", user._userID);
                cmd.ExecuteNonQuery();
            }

            conn = database.closeConnection();

            // success
            return Status.SUCCESS;
        }

        public void userWinsBattle(User user)
        {
            NpgsqlConnection conn = database.openConnection();
            user._win++;

            // check if username already exists
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET win = @win WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("win", user._win);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

            conn = database.closeConnection();
        }

        public void userLosesBattle(User user)
        {
            NpgsqlConnection conn = database.openConnection();
            user._lose++;

            // check if username already exists
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE \"user\" SET lose = @lose WHERE user_id = @user_id", conn);
            cmd.Parameters.AddWithValue("lose", user._lose);
            cmd.Parameters.AddWithValue("user_id", user._userID);
            cmd.ExecuteNonQuery();

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

