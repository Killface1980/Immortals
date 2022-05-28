using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Immortals
{
    public class WorkGiver_Impale : WorkGiver_Scanner
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
            LocalTargetInfo target = t;
            bool reservePawn = pawn.CanReserve(target, 1, -1, null, forced) && (t is Corpse || t is Pawn);

            if (FindSteel(pawn) == null)
            {
                JobFailReason.Is("IH_NoSteel".Translate(), null);
                return false;
            }
            return reservePawn;
        }

        // Token: 0x06000757 RID: 1879 RVA: 0x00041D3D File Offset: 0x0004013D

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Thing t2 = FindSteel(pawn);
            return JobMaker.MakeJob(impaleJobDef, t, t2);
        }

        private static Thing FindSteel(Pawn pawn)
        {
            Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1, null, false);
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(DefDatabase<ThingDef>.GetNamed("Steel")), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }


        static JobDef impaleJobDef = DefDatabase<JobDef>.GetNamed("IH_ImpaleHeart");
        static DesignationDef designation = DefDatabase<DesignationDef>.GetNamed("IH_Impale");
    }

}
