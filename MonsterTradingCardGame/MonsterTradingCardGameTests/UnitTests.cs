using MonsterTradingCardGame;
using NUnit.Framework;

namespace MonsterTradingCardGameTests
{
    [TestFixture]
    public class Tests
    {
        private Battle _battle;
        private const int DOUBLE_CONSTANT = 2;
        private const int NO_DAMAGE = 0;

        [SetUp]
        public void Setup()
        {
            _battle = new Battle();
        }
        
        [Test]
        public void monsterFight_Goblin90VsOrk05_NoDamageChange()
        {
            // Arrange
            int damageCard1 = 90;
            int damageCard2 = 5;
            Card card1 = new Card(0, "Fire Goblin", damageCard1, ElementType.Fire, CardType.Monster, MonsterType.Goblin);
            Card card2 = new Card(0, "Fire Ork", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard1 == calcDamageCard1);
            Assert.IsTrue(damageCard2 == calcDamageCard2);
        }

        [Test]
        public void monsterFight_Goblin05VsOrk90_NoDamageChange()
        {
            // Arrange
            int damageCard1 = 5;
            int damageCard2 = 90;
            Card card1 = new Card(0, "Fire Goblin", damageCard1, ElementType.Fire, CardType.Monster, MonsterType.Goblin);
            Card card2 = new Card(0, "Fire Ork", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard1 == calcDamageCard1);
            Assert.IsTrue(damageCard2 == calcDamageCard2);
        }

        [Test]
        public void monsterFight_Goblin90VsOrk05_GoblinWins()
        {
            // Arrange
            int damageCard1 = 90;
            int damageCard2 = 5;
            Card card1 = new Card(0, "Fire Goblin", damageCard1, ElementType.Fire, CardType.Monster, MonsterType.Goblin);
            Card card2 = new Card(0, "Fire Ork", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 > calcDamageCard2);
        }

        [Test]
        public void monsterFight_Goblin05VsOrk90_OrkWins()
        {
            // Arrange
            int damageCard1 = 5;
            int damageCard2 = 90;
            Card card1 = new Card(0, "Fire Goblin", damageCard1, ElementType.Fire, CardType.Monster, MonsterType.Goblin);
            Card card2 = new Card(0, "Fire Ork", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsFalse(calcDamageCard1 > calcDamageCard2);
        }

        [Test]
        public void spellFight_Water30VsFire20_WaterDoubles()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Water Spell", damageCard1, ElementType.Water, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Fire Spell", damageCard2, ElementType.Fire, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard1 * DOUBLE_CONSTANT == calcDamageCard1);
        }

        [Test]
        public void spellFight_Water30VsFire20_WaterWins()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Water Spell", damageCard1, ElementType.Water, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Fire Spell", damageCard2, ElementType.Fire, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 > calcDamageCard2);
        }

        [Test]
        public void spellFight_Fire30VsWater20_FireHalves()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Fire Spell", damageCard1, ElementType.Fire, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Water Spell", damageCard2, ElementType.Water, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard1 / DOUBLE_CONSTANT == calcDamageCard1);
        }

        [Test]
        public void spellFight_Fire30VsWater20_WaterWins()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Fire Spell", damageCard1, ElementType.Fire, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Water Spell", damageCard2, ElementType.Water, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsFalse(calcDamageCard1 > calcDamageCard2);
        }

        [Test]
        public void spellFight_Normal30VsNormal20_NoDamageChange()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Normal Spell", damageCard1, ElementType.Normal, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Normal Spell", damageCard2, ElementType.Normal, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard1 == calcDamageCard1);
        }

        [Test]
        public void spellFight_Normal30VsNormal30_Draw()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 30;
            Card card1 = new Card(0, "Normal Spell", damageCard1, ElementType.Normal, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Normal Spell", damageCard2, ElementType.Normal, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 == calcDamageCard2);
        }

        [Test]
        public void spellFight_Water30VsIce20_IceDoubles()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Water Spell", damageCard1, ElementType.Water, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Ice Spell", damageCard2, ElementType.Ice, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard2 * DOUBLE_CONSTANT == calcDamageCard2);
        }

        [Test]
        public void spellFight_Wind30VsIce20_WindDoubles()
        {
            // Arrange
            int damageCard1 = 30;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Wind Spell", damageCard1, ElementType.Wind, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Ice Spell", damageCard2, ElementType.Ice, CardType.Spell, MonsterType.Spell);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(damageCard1 * DOUBLE_CONSTANT == calcDamageCard1);
        }

        [Test]
        public void specialFight_Dragon10VsGoblin20_GoblinNoDamage()
        {
            // Arrange
            int damageCard1 = 10;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Fire Dragon", damageCard1, ElementType.Fire, CardType.Monster, MonsterType.Dragon);
            Card card2 = new Card(0, "Water Goblin", damageCard2, ElementType.Water, CardType.Monster, MonsterType.Goblin);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard2 == NO_DAMAGE);
        }

        [Test]
        public void specialFight_Wizzard10VsOrk20_OrkNoDamage()
        {
            // Arrange
            int damageCard1 = 10;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Fire Wizzard", damageCard1, ElementType.Fire, CardType.Monster, MonsterType.Wizzard);
            Card card2 = new Card(0, "Water Ork", damageCard2, ElementType.Water, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard2 == NO_DAMAGE);
        }

        [Test]
        public void specialFight_WaterSpell10VsKnight20_KnightNoDamage()
        {
            // Arrange
            int damageCard1 = 10;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Water Spell", damageCard1, ElementType.Water, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Fire Knight", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Knight);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard2 == NO_DAMAGE);
        }

        [Test]
        public void specialFight_WaterSpell20VsKraken10_WaterSpellNoDamage()
        {
            // Arrange
            int damageCard1 = 20;
            int damageCard2 = 10;
            Card card1 = new Card(0, "Water Spell", damageCard1, ElementType.Water, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Fire Kraken", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Kraken);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 == NO_DAMAGE);
        }

        [Test]
        public void specialFight_FireSpell20VsKraken10_FireSpellNoDamage()
        {
            // Arrange
            int damageCard1 = 20;
            int damageCard2 = 10;
            Card card1 = new Card(0, "Fire Spell", damageCard1, ElementType.Fire, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Fire Kraken", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Kraken);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 == NO_DAMAGE);
        }

        [Test]
        public void specialFight_NormalSpell20VsKraken10_FireSpellNoDamage()
        {
            // Arrange
            int damageCard1 = 20;
            int damageCard2 = 10;
            Card card1 = new Card(0, "Normal Spell", damageCard1, ElementType.Normal, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Fire Kraken", damageCard2, ElementType.Fire, CardType.Monster, MonsterType.Kraken);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 == NO_DAMAGE);
        }

        [Test]
        public void mixedFight_FireSpell20VsWaterOrk05_Draw()
        {
            // Arrange
            int damageCard1 = 20;
            int damageCard2 = 05;
            Card card1 = new Card(0, "Fire Spell", damageCard1, ElementType.Fire, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Water Ork", damageCard2, ElementType.Water, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsTrue(calcDamageCard1 == calcDamageCard2);
        }

        [Test]
        public void mixedFight_FireSpell20VsWaterOrk20_WaterOrkWins()
        {
            // Arrange
            int damageCard1 = 20;
            int damageCard2 = 20;
            Card card1 = new Card(0, "Fire Spell", damageCard1, ElementType.Fire, CardType.Spell, MonsterType.Spell);
            Card card2 = new Card(0, "Water Ork", damageCard2, ElementType.Water, CardType.Monster, MonsterType.Ork);

            // Act
            int calcDamageCard1 = _battle.calculateDamage(card1, card2);
            int calcDamageCard2 = _battle.calculateDamage(card2, card1);

            // Assert
            Assert.IsFalse(calcDamageCard1 > calcDamageCard2);
        }
    }
}