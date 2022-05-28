using RimWorld;
using Verse;

namespace Immortals.Source
{
    // RimWorld.SpecialThingFilterWorker_CorpsesSlave
    [HotSwappable]
    public class SpecialThingFilterWorker_CorpsesImmortal : SpecialThingFilterWorker
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

            if (corpse.InnerPawn.health.hediffSet.GetFirstHediffOfDef(Immortal_Component.immortalHediff) != null)
            {
                return true;
            }

            return false;
        }

    }
}
