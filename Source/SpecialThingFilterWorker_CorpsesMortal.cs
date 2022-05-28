using Verse;

namespace Immortals.Source
{
    [HotSwappable]
    public class SpecialThingFilterWorker_CorpsesMortal : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            if (t is not Corpse corpse)
            {
                return false;
            }

            if (!corpse.InnerPawn.def.race.Humanlike)
            {
                return false;
            }
            
            return !corpse.InnerPawn.IsImmmortal();

            // if (corpse.InnerPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal) == null)
            // {
            //     return true;
            // }
            // return false;
        }

    }
}
