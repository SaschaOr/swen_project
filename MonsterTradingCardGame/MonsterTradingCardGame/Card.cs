using System;
using System.Collections.Generic;
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

		public Card(int damage, ElementType element, CardType card)
		{
			_damage = damage;
			_elementType = element;
			_cardType = card;
		}
	}
}
