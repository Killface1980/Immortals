using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Immortals
{
    // Token: 0x02000DC6 RID: 3526
    public class Recipe_InstallFakeEye : Recipe_Surgery
    {
        public static ThingDef fakeEyeDef = DefDatabase<ThingDef>.GetNamed("IH_FakeEye");
     
        
        // Token: 0x0600524E RID: 21070 RVA: 0x001BAD8C File Offset: 0x001B8F8C
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
            {
                IEnumerable<Hediff> source = from x in pawn.health.hediffSet.hediffs
                                             where x.Part == record
                                             select x;
                return (source.Count<Hediff>() != 1 || source.First<Hediff>().def != recipe.addsHediff) && (record.parent == null || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(record.parent)) && (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record));
            });
        }

        // Token: 0x0600524F RID: 21071 RVA: 0x001BADCC File Offset: 0x001B8FCC
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {

            bool flag = MedicalRecipesUtility.IsClean(pawn, part);
            bool flag2 = !PawnGenerator.IsBeingGenerated(pawn) && this.IsViolationOnPawn(pawn, part, Faction.OfPlayer);
            if (billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
                if (flag && flag2 && part.def.spawnThingOnRemoved != null)
                {
                    ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn, billDoer);
                }
                if (flag2)
                {
                    base.ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
                }
                if (ModsConfig.IdeologyActive)
                {
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)), true);
                }
            }
            else if (pawn.Map != null)
            {
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, pawn.Position, pawn.Map);
            }
            else
            {
                pawn.health.RestorePart(part, null, true);
            }
            pawn.health.AddHediff(this.recipe.addsHediff, part, null, null);

            Hediff eyeHediff;
            eyeHediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.recipe.addsHediff);
            ThingDef eyeStuff = null;
            foreach (Thing t in ingredients)
            {
                if (t.def == fakeEyeDef)
                {
                    eyeStuff = t.Stuff;
                }
            }
            if (eyeHediff != null && eyeStuff != null && eyeHediff is FakeEye_Hediff)
            {
                FakeEye_Hediff fakeEyeHediff = eyeHediff as FakeEye_Hediff;
                fakeEyeHediff.Stuff = eyeStuff;
            }
        }

        // Token: 0x06005250 RID: 21072 RVA: 0x001BAEE0 File Offset: 0x001B90E0
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return ((pawn.Faction != billDoerFaction && pawn.Faction != null) || pawn.IsQuestLodger()) && (this.recipe.addsHediff.addedPartProps == null || !this.recipe.addsHediff.addedPartProps.betterThanNatural) && HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
        }
    }
}
