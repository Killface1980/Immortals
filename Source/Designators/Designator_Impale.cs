using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace Immortals
{
    [HotSwappable]
    public class Designator_Impale : Designator
    {

        public Designator_Impale()
        {
            this.defaultLabel     = "IH_Designator_Impale".Translate();
            this.defaultDesc      = "IH_Designator_ImpaleDesc".Translate();
            this.icon             = ContentFinder<Texture2D>.Get("UI/ImpaledHeart_Icon", true);
            this.soundDragSustain = SoundDefOf.Designate_DragStandard;
            this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            this.useMouseIcon     = true;
            this.soundSucceeded   = SoundDefOf.Designate_Claim;
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
                return "IH_MustDesignateImpaleAble".Translate();
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
            if (Immortals_DesignatorUtility.CanBeImpaled(t))
            {
                if (t is Corpse corpse && corpse.InnerPawn.IsImmmortal())
                {
                    return true;
                    //return corpse.InnerPawn.IsVisibleImmortal();
                    
                }
                if (t is Pawn pawn && pawn.Downed && pawn.IsImmmortal())
                {
                    return true;

                    //return pawn.IsVisibleImmortal();
                   
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

        static DesignationDef designationDef = DefDatabase<DesignationDef>.GetNamed("IH_Impale");
    }
}
