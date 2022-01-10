using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Trade
    {
        public int _tradeID { get; set; }
        public int _fromUserID { get; set; }
        public int _cardID { get; set; }
        public CardType? _cardType { get; set; }
        public ElementType? _elementType { get; set; }
        public int? _minDamage { get; set; }

        public Trade(int tradeID, int fromUserID, int cardID, CardType? cardType, ElementType? elementType, int? minDamage)
        {
            _tradeID = tradeID;
            _fromUserID = fromUserID;
            _cardID = cardID;
            _cardType = cardType;
            _elementType = elementType;
            _minDamage = minDamage;
        }
    }
}
