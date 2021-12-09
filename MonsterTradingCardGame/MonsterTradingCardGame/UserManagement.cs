using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT 'password' FROM user WHERE 'name' = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            Object passwordResponse = cmd.ExecuteScalar();

            // hashing users input
            string hashedUserInput;

            if(hashedUserInput.Equals((string)passwordResponse))
            {
                User userObject = new User(name, password);
                return userObject;
            }
            return null; 
        }

        public bool registerUser(string name, string password, int elo)
        {
            NpgsqlConnection conn = database.openConnection();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT 'userid' FROM user WHERE 'name' = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            Object response = cmd.ExecuteScalar();

            // user does not exist
            if (response == null)
            {
                // create new user
                NpgsqlCommand cmdNew = new NpgsqlCommand("INSERT INTO \"user\" (name, password, elo) VALUES (@name, @password, @elo);", conn);
                cmdNew.Parameters.AddWithValue("name", name);
                cmdNew.Parameters.AddWithValue("password", password);
                cmdNew.Parameters.AddWithValue("elo", elo);

                //https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-6.0
                // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
                byte[] salt = new byte[128 / 8];
                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetNonZeroBytes(salt);
                }
                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                Console.WriteLine($"Hashed: {hashed}");


                Object responseNew = cmdNew.ExecuteScalar();

                // check if insert is not correct
            }

            conn = database.closeConnection();
            return true;
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

