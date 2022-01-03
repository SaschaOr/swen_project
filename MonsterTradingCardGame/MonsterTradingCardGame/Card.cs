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
		public MonsterType _specialType { get; set; }

        public Card(string cardName, int damage, ElementType elementType, CardType cardType, MonsterType specialType)
		{
            _cardName = cardName;
			_damage = damage;
			_elementType = elementType;
			_cardType = cardType;
            _specialType = specialType;
        }

        public Card(int cardID, string cardName, int damage, ElementType elementType, CardType cardType, MonsterType specialType)
        {
            _cardID = cardID;
            _cardName = cardName;
            _damage = damage;
            _elementType = elementType;
            _cardType = cardType;
            _specialType = specialType;
        }
	}
}
