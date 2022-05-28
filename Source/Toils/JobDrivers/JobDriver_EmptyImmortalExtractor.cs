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
    class JobDriver_EmptyImmortalExtractor : JobDriver
    {

        // Token: 0x170009B4 RID: 2484
        // (get) Token: 0x06003408 RID: 13320 RVA: 0x0012A9E4 File Offset: 0x00128BE4
        protected Building_ImmortalExtractor Extractor
        {
            get
            {
                return (Building_ImmortalExtractor)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x170009B5 RID: 2485
        // (get) Token: 0x06003409 RID: 13321 RVA: 0x0012AA0C File Offset: 0x00128C0C
        protected Thing Beer
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        // Token: 0x0600340A RID: 13322 RVA: 0x0012AA2D File Offset: 0x00128C2D
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.Extractor, this.job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600340B RID: 13323 RVA: 0x0012AA4F File Offset: 0x00128C4F
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).FailOn(() => !this.Extractor.Finished).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = this.Extractor.TakeOutImmortalis();
                    GenPlace.TryPlaceThing(thing, this.pawn.Position, base.Map, ThingPlaceMode.Near, null, null, default);
                    StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
                    if (StoreUtility.TryFindBestBetterStoreCellFor(thing, this.pawn, base.Map, currentPriority, this.pawn.Faction, out IntVec3 c, true))
                    {
                        this.job.SetTarget(TargetIndex.C, c);
                        this.job.SetTarget(TargetIndex.B, thing);
                        this.job.count = thing.stackCount;
                        return;
                    }
                    base.EndJobWith(JobCondition.Incompletable);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return Toils_Reserve.Reserve(TargetIndex.C, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.C, PathEndMode.ClosestTouch);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carryToCell, true, false);
            yield break;
        }

        // Token: 0x04001E73 RID: 7795
        private const TargetIndex BarrelInd = TargetIndex.A;

        // Token: 0x04001E74 RID: 7796
        private const TargetIndex BeerToHaulInd = TargetIndex.B;

        // Token: 0x04001E75 RID: 7797
        private const TargetIndex StorageCellInd = TargetIndex.C;

        // Token: 0x04001E76 RID: 7798
        private const int Duration = 200;
    }
}
