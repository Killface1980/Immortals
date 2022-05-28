using System.Collections.Generic;
using Verse;
using Verse.AI;


namespace Immortals
{
    public class JobDriver_Impale : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            LocalTargetInfo targetB = this.job.targetB;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed) && pawn.Reserve(targetB, job, 1, 1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.pawn.CurJob.count = 1;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);

            Toil reserveSteel = Toils_Reserve.Reserve(TargetIndex.B, 1, 1, null);
            yield return reserveSteel;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = this.job.targetA.Thing;

                    Designation designation = base.Map.designationManager.DesignationOn(thing, DefDatabase<DesignationDef>.GetNamed("IH_Impale"));
                    if (designation != null)
                    {
                        designation.Delete();
                    }
                    Pawn impaleee = null;

                    if (thing is Corpse)
                    {
                        impaleee = (thing as Corpse).InnerPawn;
                    }

                    if (thing is Pawn)
                    {
                        impaleee = (thing as Pawn);
                    }

                    if (impaleee != null)
                    {
                        BodyPartRecord bodyPart = null;
                        foreach (BodyPartRecord part in impaleee.health.hediffSet.GetNotMissingParts())
                        {
                            foreach (BodyPartTagDef tag in part.def.tags)
                            {
                                if (tag.vital)
                                {
                                    if (tag.defName == "BloodPumpingSource")
                                    {
                                        bodyPart = part;
                                    }
                                }
                            }
                        }
                        Hediff impaleDif = impaleee.health.AddHediff(HediffDefOf_Immortals.IH_ImpaledHeart, bodyPart);
                        if (this.TargetB.Thing.stackCount == 1)
                        {
                            this.TargetB.Thing.Destroy();
                        }
                        else
                        {
                            this.TargetB.Thing.stackCount--;
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }


    }
}
