using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;


namespace Immortals
{
    public class JobDriver_Behead : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnAggroMentalState(TargetIndex.A);
            this.FailOn(() => !Immortals_DesignatorUtility.CanBeBeheaded(base.TargetThingA));
            this.FailOn(() => (base.Map.designationManager.DesignationOn(this.job.targetA.Thing, DefDatabase<DesignationDef>.GetNamed("IH_Behead")) == null));
            Toil gotoThing = new();
            gotoThing.initAction = delegate ()
            {
                this.pawn.pather.StartPath(base.TargetThingA, PathEndMode.ClosestTouch);
            };
            gotoThing.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            gotoThing.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return gotoThing;
            yield return Toils_General.Wait(60, TargetIndex.None).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = this.job.targetA.Thing;

                    Designation designation = base.Map.designationManager.DesignationOn(thing, DefDatabase<DesignationDef>.GetNamed("IH_Behead"));
                    if (designation != null)
                    {
                        designation.Delete();
                    }
                    Pawn beheadee = null;

                    if (thing is Corpse)
                    {
                        beheadee = (thing as Corpse).InnerPawn;
                    }

                    if (thing is Pawn)
                    {
                        beheadee = (thing as Pawn);
                    }

                    if (beheadee != null)
                    {
                        DamageDef damageDef = DamageDefOf.Cut;
                        BodyPartRecord bodyPart = null;
                        //bodyPart = ffffff
                        foreach (BodyPartRecord part in beheadee.health.hediffSet.GetNotMissingParts())
                        {
                            foreach (BodyPartTagDef tag in part.def.tags)
                            {
                                if (tag.vital)
                                {
                                    if (tag.defName == "ConsciousnessSource")
                                    {
                                        bodyPart = part;
                                    }
                                }
                            }
                        }
                        while (!bodyPart.parent.IsCorePart)
                        {
                            bodyPart = bodyPart.parent;
                        }
                        DamageInfo beheadDmg = new(damageDef, 1000, 999, -1f, null, bodyPart, null, DamageInfo.SourceCategory.ThingOrUnknown, null);

                        Hediff_Injury hediff = (Hediff_Injury)HediffMaker.MakeHediff(HealthUtility.GetHediffDefFromDamage(damageDef, beheadee, bodyPart), beheadee, bodyPart);
                        hediff.Severity = 1000f;
                        //beheadee.TakeDamage(beheadDmg);
                        //beheadee.PostApplyDamage(beheadDmg, 1000);
                        // Hediff immortalHediff = beheadee.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);

                        //if (immortalHediff != null)
                        if (beheadee.IsImmmortal(out Hediff immortalHediff))
                        {
                            ThingDef headDef = Verse.DefDatabase<ThingDef>.GetNamed("IH_Head");
                            Thing head = ThingMaker.MakeThing(headDef);
                            Immortal_HeadComp headComp = head.TryGetComp<Immortal_HeadComp>();
                            IntVec3? pos = null;
                            if (beheadee.PositionHeld != null)
                            {
                                pos = beheadee.PositionHeld;
                            }

                            if (beheadee.Corpse != null && beheadee.Corpse.PositionHeld != null)
                            {
                                pos = beheadee.Corpse.PositionHeld;
                            }

                            if (pos != null)
                            {

                                GenPlace.TryPlaceThing(head, pos.Value, this.pawn.Map, ThingPlaceMode.Near);
                                //head.SetPositionDirect(pos.Value);
                            }
                            if (headComp != null)
                            {
                                // Hediff immortalDif = beheadee.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
                                // if (immortalDif != null)
                                if (beheadee.IsImmmortal(out Hediff immortalDif))
                                {

                                    headComp.SetImmortalLevel(immortalDif.Severity);
                                }
                            }

                        }
                        beheadee.health.AddHediff(hediff, bodyPart, beheadDmg);
                    }
                    //pawn.TakeDamage(damageInfo);
                    //this.pawn.records.Increment(RecordDefOf.BodiesStripped);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }

    }
}
