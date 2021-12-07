using System;
using System.Linq.Expressions;

namespace MonsterTradingCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Stack cardManagement = new Stack();
            Battle battleArena = new Battle();
            User user1 = new User("Sascha", "test123");
            User user2 = new User("Jakob", "test123");
            cardManagement.buyPackage(user1);
            cardManagement.buyPackage(user2);

            Console.WriteLine("");

            user1.printCards();
            Console.WriteLine("---------------------------");
            user2.printCards();

            Console.WriteLine("");

            battleArena.fight(user1, user2);
        }
    }
}
