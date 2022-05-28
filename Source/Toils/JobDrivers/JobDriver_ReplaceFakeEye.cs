using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Immortals
{
    public class JobDriver_ReplaceFakeEye : JobDriver
    {

        // public static HediffDef fakeEyeHediffDef = DefDatabase<HediffDef>.GetNamed("IH_FakeEye");
        //public static HediffDef placeHolderDef = DefDatabase<HediffDef>.GetNamed("IH_FakeEyePlaceHolder");
        public static BodyPartTagDef sightSource = DefDatabase<BodyPartTagDef>.GetNamed("SightSource");

        // Token: 0x170009DE RID: 2526
        // (get) Token: 0x06003523 RID: 13603 RVA: 0x0012E8E8 File Offset: 0x0012CAE8
        private ThingWithComps TargetEquipment
        {
            get
            {
                return (ThingWithComps)base.TargetA.Thing;
            }
        }

        // Token: 0x06003524 RID: 13604 RVA: 0x00012A6D File Offset: 0x00010C6D
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x06003525 RID: 13605 RVA: 0x0012E908 File Offset: 0x0012CB08
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);

            Toil reserveEye = Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return reserveEye;
            Toil gotoThing = new();
            gotoThing.initAction = delegate ()
            {
                this.pawn.pather.StartPath(base.TargetThingA, PathEndMode.ClosestTouch);
            };
            yield return gotoThing;
            gotoThing.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            gotoThing.FailOnDespawnedNullOrForbidden(TargetIndex.A);

            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);


            yield return new Toil
            {
                initAction = delegate ()
                {
                    BodyPartRecord part = null;
                    Hediff placeHolder = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_FakeEyePlaceHolder);
                    if (placeHolder != null)
                    {
                        part = (placeHolder as FakeEyeHolder_Hediff).place;
                    }

                    Hediff oldEyeHediff = null;
                    foreach (Hediff hediff in this.pawn.health.hediffSet.hediffs)
                    {
                        if (hediff.def == HediffDefOf_Immortals.IH_FakeEye && hediff.Part == part)
                        {
                            oldEyeHediff = hediff;
                            part = hediff.Part;
                            break;
                        }
                    }
                    this.pawn.health.RemoveHediff(placeHolder);
                    FakeEye_Hediff fakeEyeHediff = oldEyeHediff as FakeEye_Hediff;
                    ThingDef oldStuff = fakeEyeHediff.Stuff;
                    fakeEyeHediff.Stuff = this.TargetA.Thing.Stuff;
                    if (this.TargetA.Thing.stackCount == 1)
                    {
                        this.TargetA.Thing.Destroy();
                    }
                    else
                    {
                        this.TargetA.Thing.stackCount--;
                    }

                    Thing oldEye = ThingMaker.MakeThing(this.TargetA.Thing.def, oldStuff);
                    GenPlace.TryPlaceThing(oldEye, this.pawn.Position, base.Map, ThingPlaceMode.Near, null, null, default);
                },

                defaultCompleteMode = ToilCompleteMode.Instant
            };



            yield break;
        }

        // Token: 0x04001EBC RID: 7868
        private const int DurationTicks = 30;
    }
}
