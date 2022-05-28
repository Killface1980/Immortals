using Verse;

namespace Immortals
{
    public static class Extensions
    {
        public static bool IsVisibleImmortal(this Pawn pawn)
        {
            //Hediff imDiff;
            if (pawn.IsImmmortal(out Hediff imDiff) && imDiff.Severity > 0.5f)// .health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_Immortal))
            {
                return true;

                // imDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
                // if (imDiff.Severity > 0.5f)
                // {
                //     return true;
                // }
            }
            return false;
        }

        public static bool IsImmmortal(this Pawn pawn, out Hediff hedDiff)
        {
            if (pawn == null)
            {
                hedDiff = null;
                return false;
            }
            hedDiff = pawn.GetImmortalHediff();
            return hedDiff != null;
        }

        public static bool IsImmmortal(this Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }

            return pawn.GetImmortalHediff() != null;
        }

        public static Hediff GetImmortalHediff(this Pawn pawn)
        {
            if (pawn == null) return null;
            return pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
        }
    }
}