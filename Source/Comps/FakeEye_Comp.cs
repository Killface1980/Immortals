using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Immortals
{
    public class FakeEye_Comp : ThingComp
    {
        public static HediffDef fakeEyeHediff = DefDatabase<HediffDef>.GetNamed("IH_FakeEye");
        //public static JobDef removeFakeEyeJobDef = DefDatabase<JobDef>.GetNamed("IH_RemoveFakeEye");
        public static BodyPartTagDef sightSource = DefDatabase<BodyPartTagDef>.GetNamed("SightSource");
        public static JobDef insertEyeJobDef = DefDatabase<JobDef>.GetNamed("IH_InsertFakeEye");
        public static JobDef replaceEyeJobDef = DefDatabase<JobDef>.GetNamed("IH_ReplaceFakeEye");
        public static HediffDef placeHolderDef = DefDatabase<HediffDef>.GetNamed("IH_FakeEyePlaceHolder");

        public override void PostExposeData()
        {

        }

        /*
        public override string TransformLabel(string label)
        {
            string newLabel = label + " - ";
            
            return newLabel;
        }
        */

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {

            foreach (Hediff hediff in selPawn.health.hediffSet.hediffs)
            {

                if (hediff.def == fakeEyeHediff)
                {
                    if ((hediff as FakeEye_Hediff).Stuff != this.parent.Stuff)
                    {

                        yield return new FloatMenuOption("replace " + hediff.LabelCap.ToLower() + " in " + hediff.Part.LabelCap.ToLower() + " with " + this.parent.LabelCap, delegate ()
                        {
                            FakeEyeHolder_Hediff placeHolder = selPawn.health.AddHediff(placeHolderDef) as FakeEyeHolder_Hediff;
                            placeHolder.place = hediff.Part;
                            Job job = JobMaker.MakeJob(replaceEyeJobDef, this.parent);
                            selPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
                        }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
                    }

                    //yield return new FloatMenuOption("replace " + hediff.LabelCap.ToLower() + " in " + hediff.Part.LabelCap.ToLower() + " with " + this.parent.LabelCap, null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
                }
                if (hediff.Part != null && hediff.def == HediffDefOf.MissingBodyPart)
                {
                    foreach (BodyPartTagDef tag in hediff.Part.def.tags)
                    {
                        if (tag == sightSource)
                        {
                            yield return new FloatMenuOption("place " + this.parent.LabelCap.ToLower() + " in " + hediff.Part.LabelCap.ToLower(), delegate ()
                            {
                                FakeEyeHolder_Hediff placeHolder = selPawn.health.AddHediff(placeHolderDef) as FakeEyeHolder_Hediff;
                                placeHolder.place = hediff.Part;
                                Job job = JobMaker.MakeJob(insertEyeJobDef, this.parent);
                                selPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
                            }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
                        }
                    }
                }
            }
            yield break;

        }

        public override bool AllowStackWith(Thing other)
        {
            FakeEye_Comp otherEye;
            otherEye = other.TryGetComp<FakeEye_Comp>();
            if (otherEye != null)
            {
                if (this.parent.Stuff == other.Stuff)
                {
                    return true;
                }
            }
            return false;
        }


    }





}
