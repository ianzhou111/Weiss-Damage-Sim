using System;
using System.Collections.Generic;
using MyWebApp.Models;

namespace MyWebApp
{
    public class Damages
    {
        private List<Card> oppDeck;
        private List<Card> ownDeck;
        private DeckInfo oppDeckInfo;
        private DeckInfo selfDeckInfo;
        private DeckInfo oppSecondDeckInfo;
        public static bool damageCanceled;


        public Damages(List<Card> oppDeck, List<Card> ownDeck, DeckInfo oppDeckInfo, DeckInfo selfDeckInfo, DeckInfo oppSecondDeckInfo)
        {
            this.oppDeck = oppDeck;
            this.ownDeck = ownDeck;
            this.oppDeckInfo = oppDeckInfo;
            this.selfDeckInfo = selfDeckInfo;
            this.oppSecondDeckInfo = oppSecondDeckInfo;
            damageCanceled = false;
        }

        // Deck refresh function
        public int Refresh()
        {
            oppDeck.Clear();
            // Add CX and DMG cards to the opponent's deck based on the DeckInfo
            for (int secondCx = 0; secondCx < oppSecondDeckInfo.CXInDeck; secondCx++)
            {
                oppDeck.Add(new Card(Card.CardType.CX, 0, 0)); // Add CX cards
            }
            for (int secondDmg = 0; secondDmg < (oppSecondDeckInfo.TotalCardsInDeck - oppSecondDeckInfo.CXInDeck); secondDmg++)
            {
                oppDeck.Add(new Card(Card.CardType.DMG, 0, 0)); // Add DMG cards
            }

            // Shuffle the opponent's deck
            Random rand = new Random();
            var shuffledDeck = new List<Card>(oppDeck);
            for (int n = shuffledDeck.Count - 1; n > 0; --n)
            {
                int k = rand.Next(n + 1);
                Card temp = shuffledDeck[n];
                shuffledDeck[n] = shuffledDeck[k];
                shuffledDeck[k] = temp;
            }
            oppDeck = shuffledDeck;
            oppDeck.RemoveAt(0);

            return 1; // refreshDamage
        }

        // Swing function with soul value
        public int Swing(int soul)
        {
            int swingDamage = 0;
            int refreshPenalty = 0;
            damageCanceled = false;

            // Vanilla Swing + Triggercheck
            soul += ownDeck[0].SoulTrigger; // Assuming the ownDeck contains the soul value in the Level field
            ownDeck.RemoveAt(0);

            for (int i = 0; i < soul; i++)
            {
                if (oppDeck.Count == 0)
                {
                    refreshPenalty += Refresh();
                }
                if (oppDeck[0].Type == Card.CardType.DMG)
                {
                    swingDamage += 1;
                    oppDeck.RemoveAt(0);
                }
                else
                {
                    swingDamage = 0;
                    oppDeck.RemoveAt(0);
                    damageCanceled = true;
                    break;
                }
            }

            if (oppDeck.Count == 0)
            {
                refreshPenalty += Refresh();
            }

            return swingDamage + refreshPenalty;
        }

        // Burn function with value
        public int Burn(int value)
        {
            int burnDamage = 0;
            int refreshPenalty = 0;
            damageCanceled = false;

            for (int i = 0; i < value; i++)
            {
                if (oppDeck.Count == 0)
                {
                    refreshPenalty += Refresh();
                }
                if (oppDeck[0].Type == Card.CardType.DMG)
                {
                    burnDamage += 1;
                    oppDeck.RemoveAt(0);
                }
                else
                {
                    oppDeck.RemoveAt(0);
                    burnDamage = 0;
                    damageCanceled = true;
                    break;
                }
            }

            if (oppDeck.Count == 0)
            {
                refreshPenalty += Refresh();
            }

            return burnDamage + refreshPenalty;
        }

        public (List<Card> removedCards, bool causedRefresh) MillBottom(int count)
        {
            List<Card> removedCards = new List<Card>();
            bool causedRefresh = false;

            // Check if the requested count exceeds the number of cards in the opponent's deck
            if (count > oppDeck.Count)
            {
                // If so, trigger a deck refresh
                causedRefresh = true;
                int cardsToMill = oppDeck.Count;

                // Remove all the cards from the deck
                for (int i = 0; i < cardsToMill; i++)
                {
                    removedCards.Add(oppDeck[oppDeck.Count - 1]);
                    oppDeck.RemoveAt(oppDeck.Count - 1);
                }

                // Refresh the opponent's deck after it gets emptied
                Refresh();

                // Add the remaining cards requested from the top of the new refreshed deck
                count -= cardsToMill;
            }

            // Continue removing the remaining cards if necessary
            for (int i = 0; i < count; i++)
            {
                if (oppDeck.Count > 0)
                {
                    removedCards.Add(oppDeck[oppDeck.Count - 1]);
                    oppDeck.RemoveAt(oppDeck.Count - 1);
                }
            }

            return (removedCards, causedRefresh);
        }

        public int IcyTail(int value)
        {
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(value);
            if (refresh) refreshdamage += 1;

            int burnCount = 0;
            foreach (Card removedCard in removedCards) {
                if (removedCard.Type == Card.CardType.CX) burnCount++;
            }

            burnDamge = Burn(burnCount);

            return burnDamge + refreshdamage;
        }

        public int IcyTail_lv0(int value)
        {
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(value);
            if (refresh) refreshdamage += 1;

            int burnCount = 0;
            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Level == 0) burnCount++;
            }

            burnDamge = Burn(burnCount);

            return burnDamge + refreshdamage;
        }

        public int IcyTail_Ping(int cards, int dmg)
        {
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(cards);
            if (refresh) refreshdamage += 1;

            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX) burnDamge += Burn(dmg);
            }

            return burnDamge + refreshdamage;
        }

        public int IcyTail_Ping_lv0 (int cards, int dmg)
        {
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(cards);
            if (refresh) refreshdamage += 1;

            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Level == 0) burnDamge += Burn(dmg);
            }

            return burnDamge + refreshdamage;
        }

        public int CancelBurn(int value)
        {
            if (damageCanceled)
            {
                damageCanceled = false;
                return Burn(value);
            }
            else return 0;
        }

        public int MusashiBurn(int plusVal)
        {
            var topCard = ownDeck[0];
            return Burn(topCard.Level+plusVal);

        }

        public int MusashiBurnMill(int plusVal)
        {
            var topCard = ownDeck[0];
            ownDeck.RemoveAt(0);
            return Burn(topCard.Level + plusVal);

        }

        public int Kana(int soul)
        {
            int swingDamage = Swing(soul);
            int burndamage = 0;
            if (damageCanceled){
                damageCanceled = false;
                burndamage = IcyTail_Ping_lv0(3, 2);
            }
            return burndamage + swingDamage;
        }

        public int Tsuzuri(int soul)
        {
            int swingDamage = Swing(soul);
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(3);
            if (refresh) refreshdamage += 1;

            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX) return swingDamage+refreshdamage;
            }

            int burnDamage = 0;
            for (int i = 0; i < 4; i++)
            {
                burnDamage += 1;
            }

            return swingDamage + burnDamage + refreshdamage;

        }



        // Optional: Getter for damageCanceled
        public bool DamageCanceled => damageCanceled;
    }
}
