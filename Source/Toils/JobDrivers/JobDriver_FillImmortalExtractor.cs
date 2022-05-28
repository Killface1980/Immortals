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
    // Token: 0x02000723 RID: 1827
    public class JobDriver_FillImmortalExtractor : JobDriver
    {
        // Token: 0x17000984 RID: 2436
        // (get) Token: 0x06003319 RID: 13081 RVA: 0x0012871C File Offset: 0x0012691C
        protected Building_ImmortalExtractor Extractor
        {
            get
            {
                return (Building_ImmortalExtractor)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x17000985 RID: 2437
        // (get) Token: 0x0600331A RID: 13082 RVA: 0x00128744 File Offset: 0x00126944
        protected Thing Parts
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        // Token: 0x0600331B RID: 13083 RVA: 0x00128768 File Offset: 0x00126968
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.Extractor, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.Parts, this.job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600331C RID: 13084 RVA: 0x001287B9 File Offset: 0x001269B9
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            base.AddEndCondition(delegate
            {
                if (this.Extractor.SpaceLeft > 0)
                {
                    return JobCondition.Ongoing;
                }
                return JobCondition.Succeeded;
            });
            yield return Toils_General.DoAtomic(delegate
            {
                this.job.count = this.Extractor.SpaceLeft;
            });
            Toil reserveImmortalParts = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return reserveImmortalParts;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveImmortalParts, TargetIndex.B, TargetIndex.None, true, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    this.Extractor.AddPart(this.Parts);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }

        // Token: 0x04001E36 RID: 7734
        private const TargetIndex BarrelInd = TargetIndex.A;

        // Token: 0x04001E37 RID: 7735
        private const TargetIndex WortInd = TargetIndex.B;

        // Token: 0x04001E38 RID: 7736
        private const int Duration = 200;
    }
}
