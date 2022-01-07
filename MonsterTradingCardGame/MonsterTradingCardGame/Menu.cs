using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Menu
    {
        private const int MENU_OPTIONS_BEFORE_LOGIN = 3;
        private const int MENU_OPTIONS_AFTER_LOGIN = 12;

        CardManagement cardManagement = new CardManagement();
        Battle battleArena = new Battle();
        UserManagement userManagement = new UserManagement();
        private Friendship friendship = new Friendship();

        User user1 = null, user2 = null;
        //User user2 = new User("Bot", "test123", 20, 100);
        private bool _login = false;

        public void startMainMenu()
        {
            bool finished = true;

            while (finished)
            {
                if (_login)
                {
                    printMenuOptionsAfterLogin();
                    finished = processUserInputAfterLogin();
                }
                else
                {
                    printMenuOptionsBeforeLogin();
                    finished = processUserInputBeforeLogin();
                }
                

                if (finished)
                {
                    Console.WriteLine("Please enter any command...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private void printMenuOptionsBeforeLogin()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: LOGIN");
            Console.WriteLine("2: REGISTER");
            Console.WriteLine("3: QUIT");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private void printMenuOptionsAfterLogin()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1:  BATTLE");
            Console.WriteLine("2:  BUY PACKAGE");
            Console.WriteLine("3:  SHOW STACK");
            Console.WriteLine("4:  CHANGE DECK");
            Console.WriteLine("5:  ADD FRIEND");
            Console.WriteLine("6:  OPEN FRIEND REQUESTS");
            Console.WriteLine("7:  DELETE FRIENDSHIP");
            Console.WriteLine("8:  PLAY AGAINST FRIEND");
            Console.WriteLine("9:  SCOREBOARD");
            Console.WriteLine("10: EDIT PROFILE PAGE");
            Console.WriteLine("11: LOGOUT");
            Console.WriteLine("12: QUIT");
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

        private bool processUserInputBeforeLogin()
        {
            while(true)
            {
                int userInput = getUserInput();

                if(userInput > 0 && userInput <= MENU_OPTIONS_BEFORE_LOGIN)
                {
                    switch(userInput)
                    {
                        case 1:
                            user1 = userManagement.loginUser();

                            if (user1 == null)
                            {
                                Console.WriteLine("Something went wrong! Please check your credentials!");
                            }
                            else
                            {
                                _login = true;
                                cardManagement.loadDeckOfUser(user1);
                                cardManagement.loadStackOfUser(user1);
                                //user1.printCards();
                            }
                            break;
                        case 2:
                            User newUser = userManagement.getNewUser();
                            bool status = userManagement.registerUser(newUser);
                            if (!status) Console.WriteLine("Something went wrong while registering your user! Please try again!");
                            break;
                        case 3:
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

        private bool processUserInputAfterLogin()
        {
            while (true)
            {
                int userInput = getUserInput();

                if (userInput > 0 && userInput <= MENU_OPTIONS_AFTER_LOGIN)
                {
                    switch (userInput)
                    {
                        case 1:
                            battleArena.fight(user1, user2);
                            break;
                        case 2:
                            cardManagement.buyPackage(user1);
                            cardManagement.buyPackage(user2);
                            break;
                        case 3:
                            // see stack
                            cardManagement.showStack(user1);
                            break;
                        case 4:
                            // set deck
                            cardManagement.setDeck(user1);
                            break;
                        case 5:
                            // add friend
                            friendship.makeFriendRequest(user1);
                            break;
                        case 6:
                            // open friend requests
                            friendship.acceptFriendRequest(user1);
                            break;
                        case 7:
                            // delete friend
                            break;
                        case 8:
                            user2 = friendship.playAgainstUser(user1);

                            // deck of other player 
                            cardManagement.loadDeckOfUser(user2);

                            // start battle
                            battleArena.fight(user1, user2);
                            break;
                        case 9:
                            userManagement.printScoreboard();
                            break;
                        case 10:
                            break;
                        case 11:
                            _login = false;
                            break;
                        case 12:
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
