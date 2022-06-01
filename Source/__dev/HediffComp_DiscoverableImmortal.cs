// Verse.HediffComp_Discoverable
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Immortals
{
    [HotSwappable]
    public class HediffComp_DiscoverableImmortal : HediffComp
    {
        public float suspicionLevel;
        public List<string> suspiciousPawns;
        public bool wasHealingFast;
        public bool wasDeadAndHealing;
        public bool wasQuickened;
        public bool wasRegrowingLimbs;
        private bool discovered;
        public HediffCompProperties_DiscoverableImmortal Props => (HediffCompProperties_DiscoverableImmortal)props;

        // RimWorld.PawnUtility
        public static bool ShouldSendNotificationAbout(Pawn p)
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return false;
            }
            if (PawnGenerator.IsBeingGenerated(p))
            {
                return false;
            }
            if (p.IsWorldPawn() && (!p.IsCaravanMember() || !p.GetCaravan().IsPlayerControlled) && !PawnUtility.IsTravelingInTransportPodWorldObject(p) && !p.IsBorrowedByAnyFaction() && p.Corpse.DestroyedOrNull())
            {
                return false;
            }

            return true;

            if (p.Faction != Faction.OfPlayer)
            {
                if (p.HostFaction != Faction.OfPlayer)
                {
                    return false;
                }
                if (p.RaceProps.Humanlike && p.guest.Released && !p.Downed && !p.InBed())
                {
                    return false;
                }
                if (p.CurJob != null && p.CurJob.exitMapOnArrival && !PrisonBreakUtility.IsPrisonBreaking(p))
                {
                    return false;
                }
            }
            return true;
        }

        public override string CompDebugString()
        {
            return "discovered: " + discovered +
                "\nsuspicionLevel: " + suspicionLevel +
                "\nwasQuickened: " + wasQuickened +
                "\nwasRegrowingLimbs: " + wasRegrowingLimbs +
                "\nwasHealingFast: " + wasHealingFast +
                "\nwasDeadAndHealing: " + wasDeadAndHealing +
                "\nsuspiciousPawns: " + suspiciousPawns.ToCommaList(true);
        }

        public override bool CompDisallowVisible()
        {
            return !discovered;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref discovered, "discovered", defaultValue: false);

            Scribe_Values.Look(ref suspicionLevel, "suspicionLevel", defaultValue: 0f);
            Scribe_Values.Look(ref wasRegrowingLimbs, "wasRegrowingLimbs", defaultValue: false);
            Scribe_Values.Look(ref wasHealingFast, "wasHealingFast", defaultValue: false);
            Scribe_Values.Look(ref wasQuickened, "wasQuickened", defaultValue: false);
            Scribe_Values.Look(ref wasDeadAndHealing, "wasDeadAndHealing", defaultValue: false);

            Scribe_Collections.Look(ref suspiciousPawns, "suspiciousPawns", LookMode.Value);
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            CheckDiscovered();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Find.TickManager.TicksGame % 103 == 0)
            {
                CheckDiscovered();
                //Tick(ref severityAdjustment);
            }
        }

        public override void Notify_PawnDied()
        {
            CheckDiscovered();
        }

        public void Tick()
        {
        }

        private void CheckDiscovered()
        {
            if (discovered) { return; }
            var pawn = base.Pawn;
            if (pawn == null) { return; }
            var pos = base.Pawn.Position;
            var map = base.Pawn.Map; ;

            if (!pawn.Spawned)
            {
                if (pawn.Corpse == null) { return; }
                if (!pawn.Corpse.Spawned) { return; }
                pos = base.Pawn.Corpse.Position;
                map = base.Pawn.Corpse.MapHeld; 
            }

            // if (!base.Pawn.RaceProps.Humanlike) return;
            //Log.Message(pawn.ToString());

            // the severity is needed to calculate the range and the discoverability
            if (!base.Pawn.IsImmmortal(out Hediff immi)) { return; }

            float severity = immi.Severity;

            // the immortal aura
            float immortalAuraRange = 8f;

            Pawn discoveree = null;
            bool spottedByImmortal = false;
            bool pawnWasDiscovered = false;


            if (map == null)
            {
                return;
            }
            List<Pawn> candidates = map.mapPawns.FreeColonistsSpawned.FindAll(x => x != null && !x.Dead && x.Position.InHorDistOf(pos, immortalAuraRange));
            if (candidates.NullOrEmpty()) { return; }

            foreach (Pawn candidate in candidates)
            {
                //Log.Message(base.Pawn.ToString());
                // Log.Message(item.def.defName.ToString());
                // if (item is not Pawn candidate) { continue; }
                if (candidate == pawn) continue;
                if (!candidate.Awake()) continue;
                if (!candidate.RaceProps.Humanlike) { continue; }
                if (candidate.IsQuestLodger()) { continue; }

                if (severity >= 1f && suspicionLevel > 1f)
                {
                    if (wasRegrowingLimbs || wasQuickened || wasDeadAndHealing)
                    {
                        discoveree = candidate;
                        pawnWasDiscovered = true;
                        break;
                    }
                }

                if (!candidate.IsImmmortal(out Hediff candidateImmortality) || !candidateImmortality.Visible || candidateImmortality.Severity < 0.8f)
                {
                    continue;
                }
                // if (severity < 1f) // first timers are harder to detect
                {
                    var chanceToSpot = candidateImmortality.Severity / 10f;
                    chanceToSpot *= severity;
                    if (Rand.Range(0f, 1f) < chanceToSpot)
                    {
                        discoveree = candidate;
                        pawnWasDiscovered = true;
                        spottedByImmortal = true;
                        break;
                    }
                }
            }

            if (!pawnWasDiscovered) { return; }

            discovered = true;
            immi.CurStage.becomeVisible = true;

            if (!Props.sendLetterWhenDiscovered || !ShouldSendNotificationAbout(base.Pawn))
            {
                return;
            }

            if (base.Pawn.RaceProps.Humanlike)
            {
                string text = "Immortal Discovered: " + base.Pawn.LabelShortCap;
                //string text = (Props.discoverLetterLabel.NullOrEmpty() ? ((string)("LetterLabelNewDisease".Translate() + ": " + base.Def.LabelCap)) : string.Format(Props.discoverLetterLabel, base.Pawn.LabelShortCap).CapitalizeFirst());

                string text2;

                if (spottedByImmortal)
                {
                    text2 = discoveree.LabelShortCap + " discovered another immortal.\n";
                    if (severity < 1f)
                    {
                        text2 += "\n" + base.Pawn.LabelShortCap + " is not yet aware of his immortality and still has to endure his first death to fully embrace his gift. There might have been signs for this in the past.";
                    }
                }
                else
                {
                    text2 = base.Pawn.LabelShortCap + " is suspected by " + discoveree.LabelShortCap + " to be an immortal. There had been signs in the past.";
                    if (base.Pawn.health.Dead)
                    {
                        text2 += "\nThis person is dead, is cold and has no pulse. And yet it seems as if the body is refusing to die.";
                    }
                }
                text2 += wasDeadAndHealing ? "\n- The guy is dead and still the wounds are closing." : "";
                text2 += wasHealingFast ? "\n- The wounds were healing considerably fast." : "";
                text2 += wasRegrowingLimbs ? "\n- " + base.Pawn.LabelShortCap + " was regrowing limbs like a lizard." : "";
                text2 += wasQuickened ? "\n- Strange lightning came from the sky. " + base.Pawn.LabelShortCap + " should have died and stay dead, this is unnatural." : "";

                if (suspiciousPawns.Count > 0)
                {
                    text2 += "\n\nThis was noticed by ";
                    text2 += suspiciousPawns.ToCommaList(true);
                }

                // string text2 = ((!Props.discoverLetterText.NullOrEmpty()) ? ((string)Props.discoverLetterText.Formatted(base.Pawn.LabelIndefinite(), base.Pawn.Named("PAWN")).AdjustedFor(base.Pawn).CapitalizeFirst()) : ((parent.Part != null) ? ((string)"NewPartDisease".Translate(base.Pawn.Named("PAWN"), parent.Part.Label, base.Pawn.LabelDefinite(), base.Def.label).AdjustedFor(base.Pawn).CapitalizeFirst()) : ((string)"NewDisease".Translate(base.Pawn.Named("PAWN"), base.Def.label, base.Pawn.LabelDefinite()).AdjustedFor(base.Pawn).CapitalizeFirst())));

                // Find.LetterStack.ReceiveLetter(text, text2, (Props.letterType != null) ? Props.letterType : LetterDefOf.NegativeEvent, base.Pawn);

                Find.LetterStack.ReceiveLetter(text, text2, (Props.letterType != null) ? Props.letterType : ((base.Pawn.Faction == Faction.OfPlayer && !base.Pawn.IsQuestLodger() ? LetterDefOf.PositiveEvent : base.Pawn.Faction.HostileTo(Faction.OfPlayer) ? LetterDefOf.NegativeEvent : LetterDefOf.NeutralEvent)), base.Pawn);

                return;
            }
            string text3;
            if (Props.discoverLetterText.NullOrEmpty())
            {
                text3 = ((parent.Part != null) ? ((string)"NewPartDiseaseAnimal".Translate(base.Pawn.LabelShort, parent.Part.Label, base.Pawn.LabelDefinite(), base.Def.LabelCap, base.Pawn.Named("PAWN")).AdjustedFor(base.Pawn).CapitalizeFirst()) : ((string)"NewDiseaseAnimal".Translate(base.Pawn.LabelShort, base.Def.LabelCap, base.Pawn.LabelDefinite(), base.Pawn.Named("PAWN")).AdjustedFor(base.Pawn).CapitalizeFirst()));
            }
            else
            {
                string text4 = base.Pawn.KindLabelIndefinite();
                if (base.Pawn.Name.IsValid && !base.Pawn.Name.Numerical)
                {
                    text4 = string.Concat(base.Pawn.Name, " (", base.Pawn.KindLabel, ")");
                }
                text3 = Props.discoverLetterText.Formatted(text4, base.Pawn.Named("PAWN")).AdjustedFor(base.Pawn).CapitalizeFirst();
            }
            Messages.Message(text3, base.Pawn, (Props.messageType != null) ? Props.messageType : MessageTypeDefOf.NegativeHealthEvent);
        }
    }
}