namespace MyWebApp.Models
{
    public class DeckInfo
    {
        // Number of cards in each level in the deck
        public int Lv0InDeck { get; set; }
        public int Lv1InDeck { get; set; }
        public int Lv2InDeck { get; set; }
        public int Lv3InDeck { get; set; }
        public int CXInDeck { get; set; }

        // Total number of cards in the deck
        public int TotalCardsInDeck { get; set; }

        // Number of soul triggers in the deck
        public int SoulTriggersInDeck { get; set; }

    }
}
