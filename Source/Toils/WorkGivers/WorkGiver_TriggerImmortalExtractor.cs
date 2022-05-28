using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;


namespace Immortals
{
    // Token: 0x0200085A RID: 2138
    public class WorkGiver_TriggerImmortalExtractor : WorkGiver_Scanner
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
        /*
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(triggerDesignation))
			{
				{
					yield return des.target.Thing;
				}
			}
			yield break;
		}*/

        // Token: 0x17000A2E RID: 2606
        // (get) Token: 0x06003905 RID: 14597 RVA: 0x000930A6 File Offset: 0x000912A6
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.OnCell;
            }
        }

        // Token: 0x06003906 RID: 14598 RVA: 0x00142854 File Offset: 0x00140A54
        public static void ResetStaticData()
        {
            WorkGiver_TriggerImmortalExtractor.NoWortTrans = "NoWort".Translate();
        }

        // Token: 0x06003907 RID: 14599 RVA: 0x00142894 File Offset: 0x00140A94
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_ImmortalExtractor building_Extractor || building_Extractor.Finished || building_Extractor.SpaceLeft != 0 || building_Extractor.Quickened)
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
            if (FindTriggerImmortal(pawn, building_Extractor) == null)
            {
                JobFailReason.Is(WorkGiver_TriggerImmortalExtractor.NoWortTrans, null);
                return false;
            }
            return !t.IsBurning();
        }

        // Token: 0x06003908 RID: 14600 RVA: 0x00142958 File Offset: 0x00140B58
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_ImmortalExtractor extractor = (Building_ImmortalExtractor)t;
            Thing t2 = FindTriggerImmortal(pawn, extractor);

            return JobMaker.MakeJob(triggerExtractorDef, t, t2);
        }

        // Token: 0x06003909 RID: 14601 RVA: 0x0014298C File Offset: 0x00140B8C
        private static Thing FindTriggerImmortal(Pawn pawn, Building_ImmortalExtractor extractor)
        {
            Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1, null, false);
            Thing immortal = null;
            foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(triggerDesignation))
            {
                {
                    immortal = des.target.Thing;
                    if (Immortals_DesignatorUtility.CanBeUsedToTrigger(immortal))
                    {
                        return immortal;
                    }
                }
            }

            return null;
        }


        // Token: 0x04001FB3 RID: 8115
        private static string NoWortTrans;

        static JobDef triggerExtractorDef = DefDatabase<JobDef>.GetNamed("IH_TriggerImmortalExtractor");
        static DesignationDef triggerDesignation = DefDatabase<DesignationDef>.GetNamed("IH_ExtractorTrigger");
    }
}
