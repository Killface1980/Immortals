using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using JetBrains.Annotations;

namespace Immortals
{
    [HotSwappable]
    public class Designator_Behead : Designator
    {

        public Designator_Behead()
        {
            this.defaultLabel = "IH_Designator_Behead".Translate();
            this.defaultDesc = "IH_Designator_BeheadDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Behead_Icon", true);
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

            if (Immortals_DesignatorUtility.CanBeBeheaded(t))
            {
                if (t is Corpse corpse && corpse.InnerPawn.IsImmmortal())
                {
                    return true;

                    //return corpse.InnerPawn.IsVisibleImmortal();                        ;
                }
                if (t is Pawn pawn && pawn.Downed && pawn.IsImmmortal())
                {
                    return true;
                    //return pawnIsVisibleImmortal();                        ;
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

        public static DesignationDef designationDef = DefDatabase<DesignationDef>.GetNamed("IH_Behead");
    }
}
