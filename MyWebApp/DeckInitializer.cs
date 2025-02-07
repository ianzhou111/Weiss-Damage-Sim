using MyWebApp.Models;
using System.Linq;
namespace MyWebApp
{
    public class DeckInitializer
    {
        // Method to initialize the opponent's deck based on DeckInfo
        public static void InitializeOppDeck(List<Card> deck, DeckInfo deckInfo)
        {
            // Add CX cards (always level 0)
            for (int i = 0; i < deckInfo.CXInDeck; i++)
                deck.Add(new Card(Card.CardType.CX, 0, 0));  // CX cards always at level 0 with no soul trigger

            // Add DMG cards, distributed across levels
            for (int i = 0; i < deckInfo.Lv0InDeck; i++)
                deck.Add(new Card(Card.CardType.DMG, 0, 0));  // Lv0 DMG card
            for (int i = 0; i < deckInfo.Lv1InDeck; i++)
                deck.Add(new Card(Card.CardType.DMG, 1, 0));  // Lv1 DMG card
            for (int i = 0; i < deckInfo.Lv2InDeck; i++)
                deck.Add(new Card(Card.CardType.DMG, 2, 0));  // Lv2 DMG card
            for (int i = 0; i < deckInfo.Lv3InDeck; i++)
                deck.Add(new Card(Card.CardType.DMG, 3, 0));  // Lv3 DMG card

            //fisher yates to random 
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Shared.Next(n + 1);
                (deck[k], deck[n]) = (deck[n], deck[k]); // Swap using tuple
            }
        }

        // Method to initialize self's deck based on DeckInfo
        public static void InitializeSelfDeck(List<Card> deck, DeckInfo deckInfo)
        {
            // Prioritize Soul Triggers for level 3, then level 2, then level 1, then level 0
            int soulTriggers = deckInfo.SoulTriggersInDeck;

            for (int i = 0; i < deckInfo.CXInDeck; i++)
                deck.Add(new Card(Card.CardType.CX, 0, 0));  

            // Add Soul Triggers to deck starting from level 3
            for (int i = 0; i < deckInfo.Lv3InDeck; i++)
            {
                if (soulTriggers > 0)
                {
                    deck.Add(new Card(Card.CardType.DMG, 3, 1));
                    soulTriggers--;
                }
                else
                {
                    deck.Add(new Card(Card.CardType.DMG, 3, 0));
                }
            }
            for (int i = 0; i < deckInfo.Lv2InDeck; i++)
            {
                if (soulTriggers > 0)
                {
                    deck.Add(new Card(Card.CardType.DMG, 2, 1));
                    soulTriggers--;
                }
                else
                {
                    deck.Add(new Card(Card.CardType.DMG, 2, 0));
                }
            }
            for (int i = 0; i < deckInfo.Lv1InDeck; i++)
            {
                if (soulTriggers > 0)
                {
                    deck.Add(new Card(Card.CardType.DMG, 1, 1));
                    soulTriggers--;
                }
                else
                {
                    deck.Add(new Card(Card.CardType.DMG, 1, 0));
                }
            }
            for (int i = 0; i < deckInfo.Lv0InDeck; i++)
            {
                if (soulTriggers > 0)
                {
                    deck.Add(new Card(Card.CardType.DMG, 0, 1));
                    soulTriggers--;
                }
                else
                {
                    deck.Add(new Card(Card.CardType.DMG, 0, 0));
                }
            }

            //fisher yates to random 
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Shared.Next(n + 1);
                (deck[k], deck[n]) = (deck[n], deck[k]); // Swap using tuple
            }

        }
    }
}
