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
    public class JobDriver_TriggerImmortalExtractor : JobDriver
    {
        public static DesignationDef extractorTriggerDesignation = DefDatabase<DesignationDef>.GetNamed("IH_ExtractorTrigger");

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
        protected Pawn TriggerPawn
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Pawn;
            }
        }

        // Token: 0x0600331B RID: 13083 RVA: 0x00128768 File Offset: 0x00126968
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            bool reservePawn = false;
            bool reserveExtractor = this.pawn.Reserve(this.Extractor, this.job, 1, -1, null, errorOnFailed);
            reservePawn = this.pawn.Reserve(this.TargetB, this.job, 1, -1, null, errorOnFailed);

            return reserveExtractor && reservePawn;
        }

        // Token: 0x0600331C RID: 13084 RVA: 0x001287B9 File Offset: 0x001269B9
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.pawn.CurJob.count = 1;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOnAggroMentalState(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            Toil haul = Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return haul;

            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.A);
            yield return carryToCell;
            //yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = this.job.targetB.Thing;

                    Designation designation = base.Map.designationManager.DesignationOn(thing, extractorTriggerDesignation);
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
                        DamageDef damageDef = DefDatabase<DamageDef>.GetNamed("Cut");
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
                        Hediff immortalHediff = beheadee.health.hediffSet.GetFirstHediffOfDef(Verse.DefDatabase<HediffDef>.GetNamed("IH_Immortal"));

                        DamageInfo beheadDmg = new(damageDef, 1000, 999, -1f, null, bodyPart, null, DamageInfo.SourceCategory.ThingOrUnknown, null);

                        Hediff_Injury hediff = (Hediff_Injury)HediffMaker.MakeHediff(HealthUtility.GetHediffDefFromDamage(damageDef, beheadee, bodyPart), beheadee, bodyPart);
                        hediff.Severity = 1000f;
                        //beheadee.TakeDamage(beheadDmg);
                        //beheadee.PostApplyDamage(beheadDmg, 1000);



                        if (immortalHediff != null)
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
                                Hediff immortalDif = beheadee.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("IH_Immortal"));
                                if (immortalDif != null)
                                {
                                    headComp.SetImmortalLevel(immortalDif.Severity);
                                }
                            }
                            IntVec3 position = this.job.GetTarget(TargetIndex.A).Thing.Position;
                            if (position != null)
                            {
                                this.pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_Quickening(this.pawn.Map, position, 0));
                            }
                            beheadee.health.RemoveHediff(immortalHediff);
                            Thing extractorThing = this.job.GetTarget(TargetIndex.A).Thing;
                            if (extractorThing is Building_ImmortalExtractor)
                            {
                                (extractorThing as Building_ImmortalExtractor).Quicken(immortalHediff.Severity);
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

        // Token: 0x04001E36 RID: 7734
        private const TargetIndex BarrelInd = TargetIndex.A;

        // Token: 0x04001E37 RID: 7735
        private const TargetIndex WortInd = TargetIndex.B;

        // Token: 0x04001E38 RID: 7736
        private const int Duration = 200;

    }
}
