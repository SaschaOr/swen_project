using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
	public class Card
	{
		public int _cardID { get; }
        public string _cardName { get; set; }
		public int _damage { get; }
		public ElementType _elementType { get; set; }
		public CardType _cardType { get; set; }

        public Card(string cardName, int damage, ElementType elementType, CardType cardType)
		{
            _cardName = cardName;
			_damage = damage;
			_elementType = elementType;
			_cardType = cardType;
        }

        public Card(int cardID, string cardName, int damage, ElementType elementType, CardType cardType)
        {
            _cardID = cardID;
            _cardName = cardName;
            _damage = damage;
            _elementType = elementType;
            _cardType = cardType;
        }
	}
}
