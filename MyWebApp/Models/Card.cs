namespace MyWebApp.Models
{
    public class Card
    {
        // Enum for card type (CX or DMG)
        public enum CardType
        {
            CX,
            DMG
        }

        // Properties for card's type, level, and soul trigger
        public CardType Type { get; set; }
        public int Level { get; set; }
        public int SoulTrigger { get; set; }

        // Constructor to initialize a card
        public Card(CardType type, int level, int soulTrigger)
        {
            Type = type;
            Level = level;
            SoulTrigger = soulTrigger;
        }
    }
}
