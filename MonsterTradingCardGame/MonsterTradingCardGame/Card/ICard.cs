using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public interface ICard
    {
        protected int _damage { get; }
        protected ElementType _elementType { get; set; }
        protected CardType _cardType { get; set; }
    }
}
