using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Menu
    {
        private const int MENU_OPTIONS_BEFORE_LOGIN = 3;
        private const int MENU_OPTIONS_AFTER_LOGIN = 7;
        private const int MENU_OPTIONS_CARD_MANAGEMENT = 5;
        private const int MENU_OPTIONS_USER = 5;
        private const int MENU_OPTIONS_BATTLE = 3;
        private const int MENU_OPTIONS_FRIENDS = 5;
        private const int MENU_OPTIONS_TRADING = 5;

        CardManagement cardManagement = new CardManagement();
        Battle battleArena = new Battle();
        UserManagement userManagement = new UserManagement();
        private TradeManagement tradeManagement = new TradeManagement();
        private Friendship friendship = new Friendship();

        User user1 = null, user2 = null;
        private bool _login = false;
        private int currentMenuPoint = 0;

        public void startMainMenu()
        {
            bool finished = true;

            while (finished)
            {
                if (_login)
                {
                    switch (currentMenuPoint)
                    {
                        case 0:
                            // main menu
                            printMenuOptionsAfterLogin();
                            finished = processUserInputAfterLogin();
                            break;
                        case 1:
                            // card management
                            cardManagementMenu();
                            cardManagementUserInput();
                            break;
                        case 2:
                            // user
                            userMenu();
                            userUserInput();
                            break;
                        case 3:
                            // battle
                            battleMenu();
                            battleUserInput();
                            break;
                        case 4:
                            // trading
                            tradingMenu();
                            tradingUserInput();
                            break;
                        case 5:
                            // friends
                            friendsMenu();
                            friendsUserInput();
                            break;
                        default:
                            Console.WriteLine("\nSomething went wrong! Try again.");
                            break;
                    }
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
            Console.WriteLine("1: CARD MANAGEMENT");
            Console.WriteLine("2: USER");
            Console.WriteLine("3: BATTLE");
            Console.WriteLine("4: TRADING");
            Console.WriteLine("5: FRIENDS");
            Console.WriteLine("6: LOGOUT");
            Console.WriteLine("7: QUIT");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private void cardManagementMenu()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: BUY PACKAGE");
            Console.WriteLine("2: SHOW STACK");
            Console.WriteLine("3: SHOW DECK");
            Console.WriteLine("4: CHANGE DECK");
            Console.WriteLine("5: RETURN TO MENU");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private void userMenu()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: SHOW PROFILE PAGE");
            Console.WriteLine("2: EDIT PROFILE PAGE");
            Console.WriteLine("3: SCOREBOARD");
            Console.WriteLine("4: SHOW PROFILE PAGE FROM OTHER USER");
            Console.WriteLine("5: RETURN TO MENU");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private void battleMenu()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: BATTLE AGAINST RANDOM PLAYER");
            Console.WriteLine("2: BATTLE AGAINST FRIEND");
            Console.WriteLine("3: RETURN TO MENU");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private void friendsMenu()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: SHOW FRIEND LIST");
            Console.WriteLine("2: ADD FRIEND");
            Console.WriteLine("3: OPEN FRIEND REQUESTS");
            Console.WriteLine("4: DELETE FRIENDSHIP");
            Console.WriteLine("5: RETURN TO MENU");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
        }
        private void tradingMenu()
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Please select one of the following commands:");
            Console.WriteLine("1: OPEN TRADES");
            Console.WriteLine("2: CREATE TRADE");
            Console.WriteLine("3: DELETE TRADE");
            Console.WriteLine("4: TRADE HISTORY");
            Console.WriteLine("5: RETURN TO MENU");
            Console.WriteLine("----------------------------------------------------------");
            Console.Write("Your choice: ");
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

        private bool processUserInputBeforeLogin()
        {
            int userInput = getUserInput(MENU_OPTIONS_BEFORE_LOGIN);

            switch (userInput)
            {
                case 1:
                    // login user
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
                    }

                    break;
                case 2:
                    // register user
                    User newUser = userManagement.getNewUser();
                    if (newUser == null)
                        break;

                    bool status = userManagement.registerUser(newUser);

                    if (!status)
                        Console.WriteLine("Something went wrong while registering your user! Please try again!");
                    break;
                case 3:
                    // quit
                    Console.WriteLine("The programm will be stopped! Thanks for playing!");
                    return false;
                default:
                    Console.WriteLine("An error occured!");
                    return false;
            }

            return true;
        }

        private bool processUserInputAfterLogin()
        {
            int userInput = getUserInput(MENU_OPTIONS_AFTER_LOGIN);

            switch (userInput)
            {
                case 1:
                    // card management
                    currentMenuPoint = 1;
                    break;
                case 2:
                    // user
                    currentMenuPoint = 2;
                    break;
                case 3:
                    // battle
                    currentMenuPoint = 3;
                    break;
                case 4:
                    // trading
                    currentMenuPoint = 4;
                    break;
                case 5:
                    // friends
                    currentMenuPoint = 5;
                    break;
                case 6:
                    // logout
                    _login = false;
                    break;
                case 7:
                    // quit
                    Console.WriteLine("The programm will be stopped! Thanks for playing!");
                    return false;
                default:
                    Console.WriteLine("An error occured!");
                    return false;
            }

            return true;
        }

        private void cardManagementUserInput()
        {
            int userInput = getUserInput(MENU_OPTIONS_CARD_MANAGEMENT);

            switch (userInput)
            {
                case 1:
                    // buy package
                    cardManagement.buyPackage(user1);
                    break;
                case 2:
                    // show stack
                    cardManagement.showStack(user1);
                    break;
                case 3:
                    // show deck
                    cardManagement.showDeck(user1);
                    break;
                case 4:
                    // change deck
                    cardManagement.setDeck(user1);
                    break;
                case 5:
                    // return to menu
                    currentMenuPoint = 0;
                    break;
                default:
                    Console.WriteLine("An error occured!");
                    break;
            }
        }

        private void userUserInput()
        {
            int userInput = getUserInput(MENU_OPTIONS_USER);

            switch (userInput)
            {
                case 1:
                    // show profile page
                    userManagement.printProfilePage(user1);
                    break;
                case 2:
                    // edit profile page
                    userManagement.editProfilPage(user1);
                    break;
                case 3:
                    // scoreboard
                    userManagement.printScoreboard();
                    break;
                case 4:
                    // show profile page from other user
                    userManagement.printProfilePageOtherUsers(user1);
                    break;
                case 5:
                    // return to menu
                    currentMenuPoint = 0;
                    break;
                default:
                    Console.WriteLine("An error occured!");
                    break;
            }
        }

        private void battleUserInput()
        {
            int userInput = getUserInput(MENU_OPTIONS_BATTLE);

            switch (userInput)
            {
                case 1:
                    // battle against random player

                    break;
                case 2:
                    // battle against friend

                    break;
                case 3:
                    // return to menu
                    currentMenuPoint = 0;
                    break;
                default:
                    Console.WriteLine("An error occured!");
                    break;
            }
        }


        private void friendsUserInput()
        {
            int userInput = getUserInput(MENU_OPTIONS_FRIENDS);

            switch (userInput)
            {
                case 1:
                    // show friend list
                    friendship.printFriendList(user1);
                    break;
                case 2:
                    // add friend
                    friendship.makeFriendRequest(user1);
                    break;
                case 3:
                    // open friend requests
                    friendship.acceptFriendRequest(user1);
                    break;
                case 4:
                    // delete friendship
                    friendship.deleteFriend(user1);
                    break;
                case 5:
                    // return to menu
                    currentMenuPoint = 0;
                    break;
                default:
                    Console.WriteLine("An error occured!");
                    break;
            }
        }

        private void tradingUserInput()
        {
            int userInput = getUserInput(MENU_OPTIONS_TRADING);

            switch (userInput)
            {
                case 1:
                    // open trades
                    tradeManagement.acceptTrade(user1);
                    break;
                case 2:
                    // create trade
                    tradeManagement.createTrade(user1);
                    break;
                case 3:
                    // delete trade

                    break;
                case 4:
                    // trade history
                    break;
                case 5:
                    // return to menu
                    currentMenuPoint = 0;
                    break;
                default:
                    Console.WriteLine("An error occured!");
                    break;
            }
        }


        /*private bool processUserInputAfterLogin()
        {
            while (true)
            {
                int userInput = getUserInput();

                if (userInput > 0 && userInput <= MENU_OPTIONS_AFTER_LOGIN)
                {
                    switch (userInput)
                    {
                        case 1:
                            // card management
                            cardManagementMenu();
                            break;
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
                            // edit user page
                            userManagement.editProfilPage(user1);
                            break;
                        case 11:
                            // show user page
                            userManagement.printProfilePage(user1);
                            break;
                        case 12:
                            // create trade
                            tradeManagement.createTrade(user1);
                            break;
                        case 13:
                            // show open trades
                            tradeManagement.acceptTrade(user1);
                            break;
                        case 14:
                            // trade history
                            break;
                        case 15:
                            _login = false;
                            break;
                        case 16:
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
            */


    }
}
