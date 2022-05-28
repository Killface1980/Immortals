using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Immortals
{
    [HotSwappable]
    class Designator_ExtractorTrigger : Designator
    {

        public Designator_ExtractorTrigger()
        {
            this.defaultLabel = "IH_Designator_ExtractorTrigger".Translate();
            this.defaultDesc = "IH_Designator_ExtractorTriggerDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/TriggerExtractor_Icon", true);
            this.soundDragSustain = SoundDefOf.Designate_DragStandard;
            this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            this.useMouseIcon = true;
            this.soundSucceeded = SoundDefOf.Designate_Claim;
        }

        public override int DraggableDimensions
        {
            get
            {
                return 2;
            }
        }


        public override DesignationDef Designation
        {
            get
            {
                return designationDef;
            }
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(base.Map))
            {
                return false;
            }
            if (!this.CorpsesInCell(loc).Any())
            {
                return "IH_MustDesignateBeheadAble".Translate();
            }
            return true;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            foreach (Thing t in this.CorpsesInCell(c))
            {
                this.DesignateThing(t);
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (base.Map.designationManager.DesignationOn(t, this.Designation) != null)
            {
                return false;
            }

            if (Immortals_DesignatorUtility.CanBeUsedToTrigger(t))
            {
                if (t is Corpse corpse && corpse.InnerPawn.health.hediffSet.GetFirstHediffOfDef(Immortal_Component.immortalHediff) != null)
                {
                    return  Immortals_DesignatorUtility.IsVisibleImmortal(corpse.InnerPawn);                        ;
                }
                if (t is Pawn pawn && pawn.Downed && pawn.health.hediffSet.GetFirstHediffOfDef(Immortal_Component.immortalHediff) != null)
                {
                    return Immortals_DesignatorUtility.IsVisibleImmortal(pawn); ;
                }
            }

            return false;
        }

        public override void DesignateThing(Thing t)
        {
            base.Map.designationManager.AddDesignation(new Designation(t, this.Designation));
        }


        private IEnumerable<Thing> CorpsesInCell(IntVec3 loc)
        {
            if (loc.Fogged(base.Map))
            {
                yield break;
            }
            //IEnumerable<Thing> thingEnumerable = loc.GetThingList(base.Map).Where(thing => (thing is Corpse));
            List<Thing> thingList = loc.GetThingList(base.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (this.CanDesignateThing(thingList[i]).Accepted)
                {
                    yield return thingList[i];
                }
            }
        }

        static DesignationDef designationDef = DefDatabase<DesignationDef>.GetNamed("IH_ExtractorTrigger");
    }
}
