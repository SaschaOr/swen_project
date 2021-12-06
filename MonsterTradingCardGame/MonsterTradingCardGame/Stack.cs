using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Stack
    {
        private List<ICard> _stack = new List<ICard>();

        public void buyPackage()
        {
            if (_priceOfPackage > _coins)
            {
                Console.WriteLine("You have not enough coins to buy a card package");
                return;
            }

            // generate new card
            int cardTypeCount = Enum.GetNames(typeof(CardType)).Length;
            int elementTypeCount = Enum.GetNames(typeof(ElementType)).Length;

            Random random = new Random();
            int randomCardType = random.Next(cardTypeCount);
            int randomElementType = random.Next(elementTypeCount);

            if ()
        }

        public void addCardToStack(ICard card)
        {
            _stack.Add(card)
        }

        public void removeCardFromStack
    }
}
