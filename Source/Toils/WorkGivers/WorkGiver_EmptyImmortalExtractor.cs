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
    public class WorkGiver_EmptyImmortalExtractor : WorkGiver_Scanner
    {
        // Token: 0x17000A51 RID: 2641
        // (get) Token: 0x060039BB RID: 14779 RVA: 0x00142845 File Offset: 0x00140A45
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(DefDatabase<ThingDef>.GetNamed("IH_ImmortalExtractor"));
            }
        }

        // Token: 0x060039BC RID: 14780 RVA: 0x00144FD8 File Offset: 0x001431D8
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("IH_ImmortalExtractor"));
            for (int i = 0; i < list.Count; i++)
            {
                if (((Building_ImmortalExtractor)list[i]).Finished)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x17000A52 RID: 2642
        // (get) Token: 0x060039BD RID: 14781 RVA: 0x000930A6 File Offset: 0x000912A6
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        // Token: 0x060039BE RID: 14782 RVA: 0x00145024 File Offset: 0x00143224
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return t is Building_ImmortalExtractor building_ImmortalExtractor && building_ImmortalExtractor.Finished && !t.IsBurning() && !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, forced);
        }

        // Token: 0x060039BF RID: 14783 RVA: 0x0014506D File Offset: 0x0014326D
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("IH_EmptyImmortalExtractor"), t);
        }
    }
}
