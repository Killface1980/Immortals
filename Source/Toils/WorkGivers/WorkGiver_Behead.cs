using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Immortals
{
    [HotSwappable]
    public class WorkGiver_Behead : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(designation))
            {
                {
                    yield return des.target.Thing;
                }
            }
            yield break;
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        // Token: 0x06000756 RID: 1878 RVA: 0x00041CE8 File Offset: 0x000400E8
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Map.designationManager.DesignationOn(t, designation) == null)
            {
                return false;
            }

            // Hediff immortality = pawn.health.hediffSet.GetFirstHediffOfDef(Immortal_Component.immortalHediff);
            // bool corpseIsImmortal = (t as Corpse).InnerPawn.health.hediffSet.GetFirstHediffOfDef(Immortal_Component.immortalHediff) != null;
            // 
            // if (immortality == null)
            // {
            //     if (corpseIsImmortal)
            //     {
            //         if (Find.CurrentMap.PlayerPawnsForStoryteller.Any(x=> x.health.hediffSet.GetFirstHediffOfDef(Immortal_Component.immortalHediff) != null))
            //         return false;
            //     }
            // }
            // else
            // {
            //     if (corpseIsImmortal && immortality.Severity < 1f)
            //     {
            //         return false;
            //     }
            // 
            // }

            LocalTargetInfo target = t;
            return pawn.CanReserve(target, 1, -1, null, forced) && (t is Corpse || t is Pawn);
        }

        // Token: 0x06000757 RID: 1879 RVA: 0x00041D3D File Offset: 0x0004013D
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(beheadJobDef, t);
        }



        static JobDef beheadJobDef = DefDatabase<JobDef>.GetNamed("IH_Behead");
        static DesignationDef designation = DefDatabase<DesignationDef>.GetNamed("IH_Behead");
    }

}
