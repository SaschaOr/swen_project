using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Stack
    {
        private const int PRICE_OF_PACKAGE = 5;
        private const int CARDS_PER_NEW_PACKAGE = 5;
        private const int MAX_DAMAGE = 100;

        private List<ICard> _stack = new List<ICard>();

        public void buyPackage(int coinsLeft)
        {
            if (PRICE_OF_PACKAGE > coinsLeft)
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
            int damage = random.Next(MAX_DAMAGE);

            for (int i = 0; i < CARDS_PER_NEW_PACKAGE; i++)
            {
                Card newCard = new Card(damage, (ElementType)randomElementType, (CardType)randomCardType);
                Console.WriteLine($"The new crafted card is of type {newCard._cardType} and has the element {newCard._elementType} with a damage of {newCard._damage}");
            }
        }

        public void addCardToStack(ICard card)
        {
            _stack.Add(card);
        }

        public void removeCardFromStack(ICard card)
        {

        }
    }
}
