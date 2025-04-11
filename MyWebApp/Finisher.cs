using System.Collections.Generic;
using System.Linq;
using MyWebApp.Models;

namespace MyWebApp
{
    public class Finishers
    {
        private readonly Damages damages;

        public Finishers(Damages damages)
        {
            this.damages = damages;
        }

        public int Kana(int soul)
        {
            int swing = damages.Swing(soul);
            int burn = 0;
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.IcyTail_Ping_lv0(3, 2);
            }
            return burn + swing;
        }

        public int Kana_Burn(int soul)
        {
            bool canceledOnce = false;
            int burn = damages.Burn(3);
            if (damages.DamageCanceled)
            {
                canceledOnce = true;
                damages.ResetCancelFlag();
                burn += damages.IcyTail_Ping_lv0(3, 2);
            }

            int swing = damages.Swing(soul);

            if (damages.DamageCanceled && !canceledOnce)
            {
                damages.ResetCancelFlag();
                burn += damages.IcyTail_Ping_lv0(3, 2);
            }

            return burn + swing;
        }

        public int Kana_Moca(int soul)
        {
            var oppDeck = damages.OppDeck;
            if (oppDeck[0].Type == Card.CardType.CX)
            {
                var card = oppDeck[0];
                oppDeck.RemoveAt(0);
                oppDeck.Add(card);
            }
            soul += 1;
            int swing = damages.Swing(soul);
            int burn = 0;
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.IcyTail_Ping_lv0(3, 2);
            }
            return burn + swing;
        }

        public int Ruby(int soul)
        {
            int burn = damages.Burn(2);
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.Burn(2);
            }
            return burn + damages.Swing(soul);
        }

        public int Tsuzuri(int soul)
        {
            int refresh = 0;
            var (removed, didRefresh) = damages._millBottom(3);
            if (didRefresh) refresh += 1;

            bool hitCx = removed.Exists(card => card.Type == Card.CardType.CX);

            int burn = 0;
            if (!hitCx)
            {
                for (int i = 0; i < 4; i++)
                    burn += damages.Burn(1);
            }

            return damages.Swing(soul) + burn + refresh;
        }

        public int Nadeshiko(int soul)
        {
            int burn = 0, refresh = 0;
            bool canceled = false;
            int newSoul;
            int swing = damages.Swing_ReturnSoul(soul, out newSoul);

            if (damages.DamageCanceled)
            {
                canceled = true;
                damages.ResetCancelFlag();
                burn += damages.MusashiBurn_Mill(1);
            }

            swing += damages.Swing(newSoul);
            if (damages.DamageCanceled && !canceled)
            {
                damages.ResetCancelFlag();
                burn += damages.MusashiBurn_Mill(1);
            }

            return swing + burn + refresh;
        }

        public int Nadeshiko_NoStand(int soul)
        {
            int burn = 0, refresh = 0;
            int newSoul;
            int swing = damages.Swing_ReturnSoul(soul, out newSoul);

            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.MusashiBurn_Mill(1);
            }

            return swing + burn + refresh;
        }

        public int Meguru(int soul) =>
            damages.Burn(1) + damages.Burn(1) + damages.Swing(soul);

        public int Rem(int soul) =>
            damages.Burn(3) + damages.Burn(3) + damages.Swing(soul);

        public int Hime(int soul) =>
            damages.IcyTail_Ping(6, 1) + damages.Swing(soul);

        public int Mai(int soul) =>
            damages.Swing(soul) + Mai_icy(2, 2) * 3;

        public int Mai_icy(int count, int damage)
        {
            var (removed, refreshed) = damages._millBottom(count);
            bool hasCx = removed.Exists(c => c.Type == Card.CardType.CX);
            int burn = hasCx ? damages.Burn(damage) : 0;
            return burn + (refreshed ? 1 : 0);
        }

        public int Akane(int soul)
        {
            int burn = 0, refresh = 0;
            for (int i = 0; i < 3; i++)
            {
                var (removed, r) = damages._millBottom(1);
                if (r) refresh++;
                if (removed[0].Level == 0)
                    burn += damages.Burn(1);
            }
            return damages.Swing(soul) + burn + refresh;
        }

        private int Akane_T(int soul) =>
            damages.IcyTail_Ping_lv0(3, 1) + damages.Swing(soul);

        private int IcyTail_N(int soul) =>
            damages.IcyTail(4) + damages.Swing(soul);

        public int Mill_Top_IcyTail(int soul)
        {
            int swing = damages.Swing(soul);
            var (removed, refresh) = damages._millTop(4);
            int burnCount = removed.Count(c => c.Type == Card.CardType.CX);
            return swing + damages.Burn(burnCount) + (refresh ? 1 : 0);
        }

        public int Nagi(int soul)
        {
            int swing = 0;
            int newSoul;
            swing += damages.Swing_ReturnSoul(soul, out newSoul);

            for (int i = 0; i < 3; i++)
                damages.OppDeck.Add(new Card(Card.CardType.DMG, 0, 0));
            damages.ShuffleOppDeck();

            swing += damages.Swing(newSoul);
            return swing;
        }

        public int Top_Road(int soul)
        {
            int swing = damages.Swing(soul);
            int burn = 0;

            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.Burn(3);
            }
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.Burn(1);
            }

            return swing + burn;
        }

        public int Karen_Chan(int soul)
        {
            int burn = damages.Burn(1);
            damages.Moca(2);
            return burn + damages.Swing(soul);
        }

        public int Kajyu(int soul)
        {
            int burn = damages.Burn(2);
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.Burn(2);
                if (damages.DamageCanceled)
                {
                    damages.ResetCancelFlag();
                    burn += damages.Burn(2);
                }
            }
            return burn + damages.Swing(soul);
        }

        public int Doloris_Triple_backrowburn(int soul)
        {
            int swing = 0, burn = 0, pass1 = 0, pass2 = 0, pass3 = 0, curSoul;

            swing += damages.Swing_ReturnSoul(soul, out curSoul);
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                burn += damages.Burn(curSoul);
                if (damages.DamageCanceled) { damages.ResetCancelFlag(); pass1++; }
            }

            swing += damages.Swing_ReturnSoul(soul, out curSoul);
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                pass1++;
                for (int i = 0; i < pass1; i++)
                {
                    burn += damages.Burn(curSoul);
                    if (damages.DamageCanceled) { damages.ResetCancelFlag(); pass2++; }
                }
            }

            swing += damages.Swing_ReturnSoul(soul, out curSoul);
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                pass2++;
                for (int i = 0; i < pass2; i++)
                {
                    burn += damages.Burn(curSoul);
                    if (damages.DamageCanceled) { damages.ResetCancelFlag(); pass3++; }
                }
            }

            burn += damages.Burn(2);
            if (damages.DamageCanceled)
            {
                damages.ResetCancelFlag();
                for (int i = 0; i < pass3; i++)
                    swing += damages.Burn(2);
            }

            return swing + burn;
        }
    }
}
