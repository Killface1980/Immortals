using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Immortals
{
    // Token: 0x0200085A RID: 2138
    public class WorkGiver_FillImmortalExtractor : WorkGiver_Scanner
    {
        // Token: 0x17000A2D RID: 2605
        // (get) Token: 0x06003904 RID: 14596 RVA: 0x00142845 File Offset: 0x00140A45
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(DefDatabase<ThingDef>.GetNamed("IH_ImmortalExtractor"));
            }
        }

        // Token: 0x17000A2E RID: 2606
        // (get) Token: 0x06003905 RID: 14597 RVA: 0x000930A6 File Offset: 0x000912A6
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        // Token: 0x06003906 RID: 14598 RVA: 0x00142854 File Offset: 0x00140A54
        public static void ResetStaticData()
        {
            WorkGiver_FillImmortalExtractor.NoWortTrans = "NoWort".Translate();
        }

        // Token: 0x06003907 RID: 14599 RVA: 0x00142894 File Offset: 0x00140A94
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_ImmortalExtractor building_Extractor || building_Extractor.Finished || building_Extractor.SpaceLeft <= 0)
            {
                return false;
            }
            if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }
            if (FindImmortalHeads(pawn, building_Extractor) == null)
            {
                JobFailReason.Is(WorkGiver_FillImmortalExtractor.NoWortTrans, null);
                return false;
            }
            return !t.IsBurning();
        }

        // Token: 0x06003908 RID: 14600 RVA: 0x00142958 File Offset: 0x00140B58
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_ImmortalExtractor extractor = (Building_ImmortalExtractor)t;
            Thing t2 = FindImmortalHeads(pawn, extractor);
            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("IH_FillImmortalExtractor"), t, t2);
        }

        // Token: 0x06003909 RID: 14601 RVA: 0x0014298C File Offset: 0x00140B8C
        private static Thing FindImmortalHeads(Pawn pawn, Building_ImmortalExtractor extractor)
        {
            Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false);
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(DefDatabase<ThingDef>.GetNamed("IH_Head")), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }

        // Token: 0x04001FB2 RID: 8114
        private static string TemperatureTrans;

        // Token: 0x04001FB3 RID: 8115
        private static string NoWortTrans;
    }
}
