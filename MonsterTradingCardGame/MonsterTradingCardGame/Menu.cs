using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Menu
    {
        private const int MENU_OPTIONS = 5;

        Stack cardManagement = new Stack();
        Battle battleArena = new Battle();
        UserManagement userManagement = new UserManagement();
        User user1 = new User("Sascha", "test123");
        User user2 = new User("Jakob", "test123");
        
        public void startMainMenu()
        {
            bool finished = true;
            while (finished)
            {
                printMenuOptions();

                finished = processUserInput();

                user1.printCards();
                Console.WriteLine("---------------------------");
                user2.printCards();

                if (finished)
                {
                    Console.WriteLine("Please enter any command...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private void printMenuOptions()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: LOGIN");
            Console.WriteLine("2: REGISTER");
            Console.WriteLine("3: BATTLE");
            Console.WriteLine("4: BUY PACKAGE");
            Console.WriteLine("5: QUIT");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private int getUserInput()
        {
            int choice = 0;

            while (true)
            {
                try
                {
                    string value = Console.ReadLine();
                    choice = Convert.ToInt32(value);
                    break;
                }
                catch
                {
                    Console.WriteLine("Something went wrong with your input! Try again.");
                }
            }

            return choice;
        }

        private bool processUserInput()
        {
            while(true)
            {
                int userInput = getUserInput();

                if(userInput > 0 && userInput <= MENU_OPTIONS)
                {
                    switch(userInput)
                    {
                        case 1:
                            break;
                        case 2:
                            User newUser = userManagement.getNewUser();
                            bool status = userManagement.registerUser(newUser._username, newUser._password, newUser._elo);
                            if (!status) Console.WriteLine("Something went wrong while registering your user! Please try again!");
                            break;
                        case 3:
                            battleArena.fight(user1, user2);
                            break;
                        case 4:
                            cardManagement.buyPackage(user1);
                            cardManagement.buyPackage(user2);
                            break;
                        case 5:
                            Console.WriteLine("The programm will be stopped! Thanks for playing!");
                            return false;
                        default:
                            Console.WriteLine("An error occured!");
                            return false;
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Please check your choice, it may be wrong!");
                }
            }

            return true;
        }
    }
}
