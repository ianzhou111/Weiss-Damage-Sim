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
        public bool damageCanceled;


        public Damages(List<Card> oppDeck, List<Card> ownDeck, DeckInfo oppDeckInfo, DeckInfo selfDeckInfo, DeckInfo oppSecondDeckInfo)
        {
            this.oppDeck = oppDeck;
            this.ownDeck = ownDeck;
            this.oppDeckInfo = oppDeckInfo;
            this.selfDeckInfo = selfDeckInfo;
            this.oppSecondDeckInfo = oppSecondDeckInfo;
            this.damageCanceled = false;
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
            soul += ownDeck[0].SoulTrigger; 
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

        public int Swing_ReturnSoul(int soul, out int curSoul)
        {
            int swingDamage = 0;
            int refreshPenalty = 0;
            damageCanceled = false;

            // Vanilla Swing + Triggercheck
            soul += ownDeck[0].SoulTrigger; // Assuming the ownDeck contains the soul value in the Level field
            curSoul = soul;
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

        public (List<Card> removedCards, bool causedRefresh) MillTop(int count)
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
                    removedCards.Add(oppDeck[0]);
                    oppDeck.RemoveAt(0);
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
                    removedCards.Add(oppDeck[0]);
                    oppDeck.RemoveAt(0);
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
                burndamage += IcyTail_Ping_lv0(3, 2);
            }
            return burndamage + swingDamage;
        }

        public int Kana_Burn(int soul)
        {
            bool canceledOnce = false;
            int burndamage = Burn(3);
            if (damageCanceled)
            {
                canceledOnce = true;
                damageCanceled = false;
                burndamage += IcyTail_Ping_lv0(3, 2);
            }

            int swingDamage = Swing(soul);
            
            if (damageCanceled&&!canceledOnce)
            {
                damageCanceled = false;
                burndamage += IcyTail_Ping_lv0(3, 2);
            }
            return burndamage + swingDamage;
        }

        public int Kana_Moca(int soul)
        {
            if (oppDeck[0].Type == Card.CardType.CX)
            {
                var card = oppDeck[0];
                oppDeck.RemoveAt(0);
                oppDeck.Add(card);
            }
            soul += 1;
            int swingDamage = Swing(soul);
            int burndamage = 0;
            if (damageCanceled)
            {
                damageCanceled = false;
                burndamage += IcyTail_Ping_lv0(3, 2);
            }
            return burndamage + swingDamage;
        }

        public int Ruby(int soul)
        {
            int burndamage = 0;
            burndamage += Burn(2);
            if (damageCanceled)
            {
                damageCanceled = false;
                burndamage += Burn(2);
            }
            int swingDamage = Swing(soul);
            return burndamage + swingDamage;
        }

        public int Tsuzuri(int soul)
        {
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(3);
            if (refresh) refreshdamage += 1;
            bool hitCx = false;
            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX)
                {
                    hitCx = true;
                }
            }

            int burnDamage = 0;
            if (!hitCx)
            {
                for (int i = 0; i < 4; i++)
                {
                    burnDamage += Burn(1);
                }
            }


            int swingDamage = Swing(soul);

            return swingDamage + burnDamage + refreshdamage;

        }

        public int Tsuzuri_1(int soul)
        {
            oppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(3);
            if (refresh) refreshdamage += 1;
            bool hitCx = false;
            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX)
                {
                    hitCx = true;
                }
            }

            int burnDamage = 0;
            if (!hitCx)
            {
                for (int i = 0; i < 4; i++)
                {
                    burnDamage += Burn(1);
                }
            }
            

            int swingDamage = Swing(soul);

            return swingDamage + burnDamage + refreshdamage;

        }

        public int Tsuzuri_2(int soul)
        {
            oppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            oppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(3);
            if (refresh) refreshdamage += 1;
            bool hitCx = false;
            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX)
                {
                    hitCx = true;
                }
            }

            int burnDamage = 0;
            if (!hitCx)
            {
                for (int i = 0; i < 4; i++)
                {
                    burnDamage += Burn(1);
                }
            }


            int swingDamage = Swing(soul);

            return swingDamage + burnDamage + refreshdamage;

        }

        public int Nadeshiko(int soul)
        {
            bool canceldOnce = false;
            int newSoul = soul;
            int swingDamage = Swing_ReturnSoul(soul, out newSoul);
            int burnDamage = 0;
            int refreshdamage = 0;

            if (damageCanceled) {
                canceldOnce = true;
                damageCanceled = false;
                burnDamage += MusashiBurnMill(1);
            }

            swingDamage += Swing(newSoul);
            if (damageCanceled&&!canceldOnce)
            {
                canceldOnce = true;
                damageCanceled = false;
                burnDamage += MusashiBurnMill(1);
            }

            return swingDamage + refreshdamage + burnDamage;



        }

        public int Nadeshiko_NoStand(int soul)
        {
            bool canceldOnce = false;
            int newSoul = soul;
            int swingDamage = Swing_ReturnSoul(soul, out newSoul);
            int burnDamage = 0;
            int refreshdamage = 0;

            if (damageCanceled)
            {
                canceldOnce = true;
                damageCanceled = false;
                burnDamage += MusashiBurnMill(1);
            }

            return swingDamage + refreshdamage + burnDamage;



        }

        public int Meguru(int soul)
        {
            int burnDamage = Burn(1);
            burnDamage += Burn(1);
            int swingDamage = Swing(soul);
            return swingDamage + burnDamage;
        }

        public int Rem(int soul)
        {
            int burnDamage = Burn(3);
            burnDamage += Burn(3);
            int swingDamage = Swing(soul);
            return swingDamage + burnDamage;
        }

        public int Hime(int soul)
        {
            int burnDamage = IcyTail_Ping(6, 1);
            int swingDamage = Swing(soul);
            return swingDamage + burnDamage;
        }

        public int ClockKick(int amount)
        {
            int refreshPenalty = 0;
            if (oppDeck.Count == 0)
            {
                refreshPenalty += Refresh();
            }
            for(int i = 0; i < amount; i++)
            {
                oppDeck.RemoveAt(0);
            }
            return refreshPenalty + amount;
        }

        public int Mai(int soul)
        {
            int burnDamge = 0;
            burnDamge += Mai_icy(2, 2);
            burnDamge += Mai_icy(2, 2);
            burnDamge += Mai_icy(2, 2);
            int swingDamage = Swing(soul);
            return swingDamage + burnDamge;
        }

        public int Mai_icy(int amount, int damage)
        {
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(amount);
            if (refresh) refreshdamage += 1;

            bool hasCx = false;
            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX) hasCx = true;
            }
            if (hasCx)
            {
                burnDamge = Burn(damage);
            }
            
            return burnDamge + refreshdamage;
        }

        public int Akane(int soul)
        {
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillBottom(1);
            if (refresh) refreshdamage += 1;
            if (removedCards[0].Level == 0)
            {
                burnDamge += Burn(1);
            }

            (List<Card> removedCards1, bool refresh1) = MillBottom(1);
            if (refresh1) refreshdamage += 1;
            if (removedCards1[0].Level == 0)
            {
                burnDamge += Burn(1);
            }

            (List<Card> removedCards2, bool refresh2) = MillBottom(1);
            if (refresh2) refreshdamage += 1;
            if (removedCards2[0].Level == 0)
            {
                burnDamge += Burn(1);
            }

            int swingDamage = Swing(3);
            return swingDamage + refreshdamage + burnDamge;
        }

        public int Akane_T(int soul)
        {
            
            int burndamage = IcyTail_Ping_lv0(3, 1);
            int swingDamage = Swing(soul);
            return burndamage + swingDamage;
        }

        public int IcyTail_N(int soul)
        {

            int burndamage = IcyTail(4);
            int swingDamage = Swing(soul);
            return burndamage + swingDamage;
        }

        public int MillTopIcyTail(int soul)
        {

            int swingDamage = Swing(3);
            int burnDamge = 0;
            int refreshdamage = 0;

            (List<Card> removedCards, bool refresh) = MillTop(4);
            if (refresh) refreshdamage += 1;

            int burnCount = 0;
            foreach (Card removedCard in removedCards)
            {
                if (removedCard.Type == Card.CardType.CX) burnCount++;
            }

            burnDamge = Burn(burnCount);

            return burnDamge + refreshdamage + swingDamage;

        }


        public int Nagi(int soul)
        {
            int swingDamage = 0;
            int newSoul = soul;
            swingDamage += Swing_ReturnSoul(soul, out newSoul);

            oppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            oppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            oppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            shuffleDeck(oppDeck);

            swingDamage += Swing(newSoul);
            return swingDamage;
        }

        public int TopRoad(int soul)
        {
            int swingDamage = Swing(soul);
            int burnDamage = 0;

            if (damageCanceled)
            {
                damageCanceled = false;
                burnDamage += Burn(3);
            }

            if (damageCanceled)
            {
                damageCanceled = false;
                burnDamage += Burn(1);
            }

            return swingDamage + burnDamage;

        }

        public int KarenChan(int soul)
        {
            int burnDamage = Burn(1);
            Moca(2);
            int swingDamage = Swing(soul);
            return burnDamage + swingDamage;
        }

        public void Moca(int amount)
        {
            var listCards = new List<Card>();
            for (int i = 0; i < amount; i++)
            {
                if (oppDeck[i].Type == Card.CardType.CX)
                {
                    oppDeck.RemoveAt(i);
                }
            }
        }

        private void shuffleDeck(List<Card> deck)
        {
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Shared.Next(n + 1);
                (deck[k], deck[n]) = (deck[n], deck[k]); // Swap using tuple
            }
        }

        public int Doloris(int soul)
        {
            //setup
            int swingDamage = 0;
            int burnDamage = 0;
            int passCancelburn = 0;
            int curSoul = 0;

            //first attack
            swingDamage += Swing_ReturnSoul(soul, out curSoul);
            if (damageCanceled)
            {
                //Burn soul 
                damageCanceled = false;
                burnDamage += Burn(curSoul);
                if (damageCanceled)
                {
                    damageCanceled = false;
                    passCancelburn++;
                }

            }
            int passCancelburn2 = 0;
            //second attack
            swingDamage += Swing_ReturnSoul(soul, out curSoul);
            if (damageCanceled)
            {
                damageCanceled = false;
                passCancelburn++;
                for (int i = 0; i < passCancelburn; i++) {
                    burnDamage += Burn(curSoul);
                    if (damageCanceled)
                    {
                        damageCanceled = false;
                        passCancelburn2++;
                    }

                }
            }

            //3rd attack
            int passCancelburn3 = 0;
            swingDamage += Swing_ReturnSoul(soul, out curSoul);
            if (damageCanceled)
            {
                damageCanceled = false;
                passCancelburn2++;
                for (int i = 0; i < passCancelburn2; i++)
                {
                    burnDamage += Burn(curSoul);
                    if (damageCanceled)
                    {
                        damageCanceled = false;
                        passCancelburn3++;
                    }

                }
            }

            //backrow burn
            burnDamage += Burn(2);
            if (damageCanceled)
            {
                damageCanceled = false;
                for(int i = 0;i < passCancelburn3; i++)
                {
                    swingDamage += Burn(2);
                }
            }

            return swingDamage + burnDamage;

        }

        public int Kajyu(int soul)
        {
            int burnDamage = 0;
            burnDamage += Burn(2);
            if (damageCanceled)
            {
                damageCanceled = false;
                burnDamage += Burn(2);
                if (damageCanceled)
                {
                    damageCanceled = false;
                    burnDamage += Burn(2);
                }
            }
            int swingDamage = Swing(soul);
            return swingDamage + burnDamage;
        }




        // Optional: Getter for damageCanceled
        public bool DamageCanceled => damageCanceled;
    }
}
