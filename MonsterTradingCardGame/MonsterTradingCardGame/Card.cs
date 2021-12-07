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
		public int _damage { get; }
		public ElementType _elementType { get; set; }
		public CardType _cardType { get; set; }
        public string _name { get; set; }

		public Card(int damage, ElementType element, CardType card, string name)
		{
			_damage = damage;
			_elementType = element;
			_cardType = card;
            _name = name;
        }
	}
}
