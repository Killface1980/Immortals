using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Immortals
{
    public class Immortal_Component : GameComponent
    {
        public bool loaded = false;

        private static float baseHealSpeed = 0.002f;

        private static float baseHungerSpeed = 50f;

        private Dictionary<Pawn, Map> deadPawnMaps;

        private List<Pawn> deadPawns;

        // HediffDef halfCyclerHediff;
        private DamageInfo dmg;

        private bool firstTick = true;

        private HediffComp getsPermanent;

        private List<Pawn> immortalPawns;

        private int lastTick;

        private List<Pawn> pawnsToRez;

        private int rareTick;

        private Dictionary<Pawn, Dictionary<float, float>> recoverValues;

        private Immortals_Settings settings;

        //List<string> deadAilments;
        //List<string> livingAilments;
        //List<string> cureAilments;
        private List<string> sourceNeeds;

        // HediffDef nemnirHediff;
        // HediffDef komaHediff;
        // HediffDef mortalisCrystalHediff;
        private DamageDef stuntedDmgDef;

        private List<Pawn> stuntedPawns;

        // HediffDef immortalHediff;
        // HediffDef deathStasisHediff;
        // HediffDef stuntedHediff;
        // HediffDef stuntedProcHediff;
        // HediffDef stuntedBurnHediff;
        // HediffDef timerHediff;
        // HediffDef growingHediff;
        // HediffDef missingHediff;
        // HediffDef adjustingHediff;
        // HediffDef returnedHediff;
        // HediffDef firstReturnedHediff;
        // HediffDef hungerHediff;
        // HediffDef hungerHolderHediff;
        // HediffDef impaledHeart;
        private int ticks = 0;

        public Immortal_Component(Game game)
        {
            this.loaded = false;
            this.settings = LoadedModManager.GetMod<Immortals_Mod>().GetSettings<Immortals_Settings>();
            if (!this.settings.hediffsLoaded)
            {
                this.settings.LoadBaseHediffs();
            }

            if (!this.settings.pawnTypesLoaded)
            {
                this.settings.LoadPawnCategories();
            }

            if (!this.settings.hediffSettingsLoaded)
            {
                this.settings.LoadHediffSettings();
            }

            this.settings.LoadAgeHediffs();
            if (this.settings.immortalClasses == null)
            {
                this.settings.DefaultImmortalClasses();
            }

            this.immortalPawns = new List<Pawn>();
            this.deadPawns = new List<Pawn>();
            this.deadPawnMaps = new Dictionary<Pawn, Map>();
            this.pawnsToRez = new List<Pawn>();
            this.stuntedPawns = new List<Pawn>();
        }

        public override void GameComponentUpdate()
        {
            bool doTick = false;
            int tickMult = Find.TickManager.TicksGame - this.lastTick;
            this.lastTick = Find.TickManager.TicksGame;
            this.ticks += tickMult;

            //Are we running at 1x or not
            if (Current.Game.tickManager.slower.ForcedNormalSpeed || Current.Game.tickManager.CurTimeSpeed == TimeSpeed.Normal)
            {
                if (this.ticks > this.settings.immortalTickRateSingle)
                {
                    doTick = true;
                }
            }
            else
            {
                if (this.ticks > this.settings.immortalTickRate)
                {
                    doTick = true;
                }
            }
            //if enough ticks have passed then do a pass on all immortals
            if (doTick)
            {
                //First ticks have a huge since last value so ignore them
                if (!this.firstTick)
                {
                    this.rareTick += this.ticks;
                }
                else
                {
                    this.firstTick = false;
                }

                //
                this.pawnsToRez = new List<Pawn>();
                List<Pawn> pawnsToDelete = new();
                //Work on dead immortals
                if (this.deadPawns.Count > 0)
                {
                    List<Pawn> pawnsToRez = new();
                    foreach (Pawn pawn in this.deadPawns)
                    {
                        //Heal dead immortals and check if we should revive them
                        if (!this.TickDeadImmortal(pawn, this.ticks))
                        {
                            pawnsToDelete.Add(pawn);
                        }
                    }
                }

                //Work on living immortals
                if (this.immortalPawns.Count > 0)
                {
                    foreach (Pawn pawn in this.immortalPawns)
                    {
                        //Run through living immortals and check if we should move any to dead immortals or remove them fully.
                        if (!this.TickLivingImmortal(pawn, this.ticks))
                        {
                            pawnsToDelete.Add(pawn);
                        }
                    }
                }

                //Revive pawns
                if (this.pawnsToRez.Count > 0)
                {
                    foreach (Pawn pawn in this.pawnsToRez)
                    {
                        this.RezPawn(pawn);
                    }
                }

                if (pawnsToDelete.Count > 0)
                {
                    foreach (Pawn pawn in pawnsToDelete)
                    {
                        if (pawn != null)
                        {
                            //Make sure pawn has a brain otherwise put them in the proper list
                            if (pawn.Dead && pawn.Corpse != null && Immortals_DesignatorUtility.HasConciousnessPart(pawn))
                            {
                                this.AddDeadImmortal(pawn);
                            }
                            else if (!pawn.Dead)
                            {
                                this.AddImmortal(pawn, false);
                            }
                            else
                            {
                                this.CheckQuickening(pawn);
                            }
                        }
                    }
                }
                this.ticks = 0;
            }

            //is it time for a rare tick
            if (this.rareTick > this.settings.immortalTickRate * this.settings.immortalTickRateRare)
            {
                foreach (Pawn pawn in this.immortalPawns)
                {
                    this.RareTickLivingImmortal(pawn, this.rareTick);
                }

                List<Pawn> unStuntedImmortals = new();

                foreach (Pawn pawn in this.stuntedPawns)
                {
                    if (!this.TickStuntedImmoral(pawn, this.rareTick))
                    {
                        unStuntedImmortals.Add(pawn);
                    }
                }
                foreach (Pawn pawn in unStuntedImmortals)
                {
                    this.stuntedPawns.Remove(pawn);
                }

                this.rareTick -= (int)(this.settings.immortalTickRate * this.settings.immortalTickRateRare);
                //Rare stuff yo
            }


        }


        private bool TickDeadImmortal(Pawn pawn, int tickMult)
        {
            bool keep = false;
            if (pawn.health.Dead && Immortals_DesignatorUtility.HasConciousnessPart(pawn))
            {
                if (pawn.Corpse != null)
                {
                    pawn.Position = pawn.Corpse.Position;
                    //set healSpeed based on time past
                    float healSpeed = (baseHealSpeed * this.settings.baseHealSpeed) * tickMult;

                    CompRottable compRottable = pawn.Corpse.GetComp<CompRottable>();
                    Hediff hediff;
                    Hediff imDiff = pawn.GetImmortalHediff();
                    Hediff stundDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Stunted);

                    //Stop pawns from fully rotting
                    if (compRottable != null)
                    {
                        if (compRottable.RotProgress < 80000)
                        {
                            compRottable.RotProgress -= 0.5f * tickMult;
                        }
                        else if (compRottable.RotProgress < 140000)
                        {
                            compRottable.RotProgress -= 0.75f * tickMult;
                        }
                        else
                        {
                            compRottable.RotProgress = 140000;
                        }
                    }

                    if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_Immortal))
                    {
                        keep = true;

                        if (imDiff != null)
                        {
                            healSpeed *= imDiff.Severity;
                            if (imDiff.Severity <= 0.5f)
                            {
                                healSpeed *= this.settings.firstDeathHealFactor;
                                healSpeed *= 2;
                            }
                        }
                    }

                    //Get a list of all injuries that need to be cured and organs that need to be regrown
                    IEnumerable<Hediff> heDiffsEnumerable = pawn.health.hediffSet.hediffs.Where(hd => this.settings.HediffCureToRevive(hd) || (hd.def == HediffDefOf_Immortals.IH_regrowing && this.NeedPart((hd as Regrowing_Hediff).forPart)) && hd.Severity < 1f);
                    hediff = null;
                    bool canReturn = false;
                    if (heDiffsEnumerable.Any())
                    {
                        hediff = heDiffsEnumerable.RandomElement();
                        if (hediff != null)
                        {
                            healSpeed *= this.settings.HediffHealSpeedDef(hediff.def);
                            HediffComp_Disappears dissapears = hediff.TryGetComp<HediffComp_Disappears>();
                            //Check if random hediff is the growing hediff do stuff
                            if (hediff.def == HediffDefOf_Immortals.IH_regrowing)
                            {
                                Regrowing_Hediff growingDif = hediff as Regrowing_Hediff;
                                hediff.Severity -= ((healSpeed * this.HealingFactor(pawn)) / growingDif.partHp / (2 / this.settings.immortalRegrowSpeed));
                                if (this.settings.immortalAccumulateFoodNeed != 0)
                                {
                                    float foodCost = healSpeed / (imDiff.Severity / 4) * (tickMult * baseHungerSpeed) * this.settings.immortalRegrowFoodCost / 10;
                                    Hediff foodDiff = null;

                                    if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_hungerHolder))
                                    {
                                        foodDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_hungerHolder);
                                    }
                                    else
                                    {
                                        foodDiff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_hungerHolder);
                                    }

                                    foodDiff.Severity -= foodCost;
                                }
                            }
                            //If the hediff is a time based one reduce its time
                            else if (dissapears != null)
                            {
                                float val = 10;
                                dissapears.CompPostTick(ref val);
                            }
                            else
                            {
                                if (hediff.def == HediffDefOf_Immortals.IH_DeathTimer)
                                {
                                    hediff.Severity -= baseHealSpeed * tickMult;
                                }
                                else
                                {
                                    hediff.Severity -= healSpeed;
                                }
                            }
                            if (hediff.Severity <= 0.001f)
                            {
                                pawn.health.RemoveHediff(hediff);
                            }
                        }
                    }
                    //Check if we have any parts that need to be regrown more before we can come back
                    IEnumerable<Hediff> regrowDiffsEnumerable = heDiffsEnumerable.Where(hd => (hd.def == HediffDefOf_Immortals.IH_regrowing && this.NeedPart((hd as Regrowing_Hediff).forPart)));
                    if (regrowDiffsEnumerable.Count() - heDiffsEnumerable.Count() == 0)
                    {
                        canReturn = true;
                        foreach (Hediff growDiff in regrowDiffsEnumerable)
                        {
                            if (growDiff.Severity < 0.5f)
                            {
                                canReturn = false;
                                break;
                            }
                        }
                    }
                    if (canReturn)
                    {
                        this.pawnsToRez.Add(pawn);
                    }
                }
            }
            return keep;
        }

        private bool TickLivingImmortal(Pawn pawn, int tickMult)
        {
            bool keep = false;
            if (pawn != null && !pawn.health.Dead)
            {
                keep = true;
                IEnumerable<Hediff> slowDownEnumerable = pawn.health.hediffSet.hediffs;
                Hediff hediff;
                float healSpeed = this.GetHealSpeed(pawn) * tickMult;


                if (healSpeed != 0)
                {
                    float foodSpeed = 1 / healSpeed;

                    //Cure and limit Diseases and Conditions
                    foreach (Hediff slowDiff in slowDownEnumerable)
                    {
                        if (slowDiff.def == HediffDefOf_Immortals.IH_Stunted)
                        {
                            break;
                        }
                        if (this.settings.hediffSettings.ContainsKey(slowDiff.def))
                        {
                            if (!this.settings.HediffBaseCanGet(slowDiff.def))
                            {
                                slowDiff.Heal(100f);
                            }
                            float maxProg = this.settings.HediffMaxProgress(slowDiff.def);
                            if (maxProg != -1)
                            {
                                if (slowDiff.Severity > maxProg)
                                {
                                    slowDiff.Severity = maxProg;
                                }
                            }
                        }
                        else if (this.settings.oldAgeDefs.Contains(slowDiff.def))
                        {
                            slowDiff.Heal(100f);
                        }
                        else if (slowDiff.def.injuryProps == null && slowDiff.def.isBad)
                        {
                            if (!slowDiff.def.tendable)
                            {
                                if (this.settings.canGetConditions)
                                {
                                    if (slowDiff.Severity > this.settings.conditionMaxProg)
                                    {
                                        slowDiff.Severity = this.settings.conditionMaxProg;
                                    }
                                }
                                else
                                {
                                    slowDiff.Heal(100f);
                                }
                            }
                            else
                            {
                                if (this.settings.canGetDisease)
                                {
                                    if (slowDiff.Severity > this.settings.diseaseMaxProg)
                                    {
                                        slowDiff.Severity = this.settings.diseaseMaxProg;
                                    }
                                }
                                else
                                {
                                    slowDiff.Heal(100f);
                                }
                            }
                        }
                    }

                    //Try and Heal pawn of injuries

                    IEnumerable<Hediff> heDiffsEnumerable = pawn.health.hediffSet.hediffs.Where(hd => this.settings.HediffBaseHeal(hd, false) && hd.def != HediffDefOf_Immortals.IH_DeathTimer);
                  
                    
                    hediff = null;
                    if (heDiffsEnumerable.Any())
                    {
                        hediff = heDiffsEnumerable.RandomElement();
                    }

                    if (hediff != null)
                    {
                        healSpeed *= this.settings.HediffHealSpeedDef(hediff.def);
                        //Add hunger if the hediff is a regrowing part
                        if (hediff.def == HediffDefOf_Immortals.IH_regrowing)
                        {
                            float growMult = 1;
                            if (pawn.needs.food != null)
                            {
                                if (pawn.needs.food.CurLevel >= 0.75f)
                                {
                                    growMult = 0.2f;
                                }
                                else if (pawn.needs.food.CurLevel >= 0.5f)
                                {
                                    growMult = 0.15f;
                                }
                                else if (pawn.needs.food.CurLevel <= 0.1f)
                                {
                                    growMult = 0.075f;
                                }
                            }
                            hediff.Severity -= (healSpeed / hediff.Part.def.GetMaxHealth(pawn) / (2 / this.settings.immortalRegrowSpeed)) * growMult;
                            float foodCost = ((healSpeed / foodSpeed * (pawn.needs.food.FoodFallPerTick * tickMult * baseHungerSpeed) * this.settings.immortalRegrowFoodCost)) / 10;
                            if (pawn.needs.food != null && this.settings.immortalRegrowCostsFood)
                            {
                                if (pawn.needs.food.CurLevel > 0)
                                {
                                    pawn.needs.food.CurLevel += foodCost;
                                }
                                else if (this.settings.immortalAccumulateFoodNeed != 0)
                                {
                                    Hediff foodHediff = null;
                                    if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_theHunger))
                                    {
                                        foodHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_theHunger);
                                    }
                                    else
                                    {
                                        foodHediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_theHunger);
                                    }

                                    foodHediff.Severity += foodCost * this.settings.immortalAccumulateFoodNeed;
                               
                                }
                            }
                            //Swap the hegrowing hediff for the adjusting hediff
                            if (hediff.Severity >= 1 && hediff.Part != null)
                            {
                                pawn.health.AddHediff(HediffDefOf_Immortals.IH_adjusting, hediff.Part);
                                pawn.health.RemoveHediff(hediff);
                            }
                        }
                        //Otherwise just heal the hediff
                        else
                        {
                            hediff.Severity -= healSpeed * this.HealingFactor(pawn);
                        }
                    }
                }
                else
                {
                    keep = false;
                }
            }
            return keep;
        }

        private bool TickStuntedImmoral(Pawn pawn, int tickMult)
        {
            Hediff stHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Stunted);
            Hediff imHediff = pawn.GetImmortalHediff();
            if (stHediff != null && imHediff != null)
            {
                int chance = 0;
                chance = Rand.RangeInclusive(0, 100);
                if (chance <= this.settings.burnChance * 100)
                {
                    float damageNum = Rand.Range(1f, ((float)tickMult) / 1000f * this.settings.burnMult);
                    DamageInfo damage = new(this.stuntedDmgDef, damageNum);
                    pawn.TakeDamage(damage);
                    stHediff.Severity -= damageNum / 25;
                    imHediff.Severity -= damageNum / 25;
                    if (imHediff.Severity < 0.6f)
                    {
                        imHediff.Severity = 0.6f;
                    }
                }
                return true;
            }
            return false;
        }

        private bool RareTickLivingImmortal(Pawn pawn, int tickMult)
        {
            bool keep = false;
            if (pawn != null && pawn.IsImmmortal(out Hediff imDiff))//.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_Immortal))
            {
                keep = true;
                IEnumerable<Hediff> regrowHediffs;
                // Hediff imDiff = pawn.GetImmortalHediff();
                if (imDiff.Severity > 0.5f)
                {
                    //Start Regrowing Limbs
                    regrowHediffs = pawn.health.hediffSet.hediffs.Where(partDif => partDif.def == HediffDefOf_Immortals.IH_regrowing);
                    if ((regrowHediffs.Count() < this.settings.immortalRegrowMaxParts || this.settings.immortalRegrowMaxParts == -1))
                    {
                        this.RegrowLimbs(pawn, imDiff);
                    }

                    //Heal Scars

                    if (this.settings.scarHealSpeed > 0)
                    {
                        this.HealScars(pawn, imDiff, tickMult);
                    }

                    //
                    if (regrowHediffs != null)
                    {
                        foreach (Hediff hediff in regrowHediffs)
                        {
                            if (pawn.health.hediffSet.hediffs.Where(partDif => hediff.Part.parent == partDif.Part && (partDif.def == HediffDefOf_Immortals.IH_regrowing || partDif.def.addedPartProps != null)).Any())
                            {
                                hediff.Severity = 0;
                            }
                        }

                    }
                    /*
                    IEnumerable<Hediff> slowHediffs = pawn.health.hediffSet.hediffs.Where(slowDif => settings.hediffSettings.ContainsKey(slowDif.def));
                    foreach (Hediff slowDif in slowHediffs)
                    {
                        if (!settings.hediffSettings[slowDif.def].canGet.Value)
                        {
                            pawn.health.RemoveHediff(slowDif);
                            break;
                        }
                    }
                    */
                    Hediff stDiff;
                    stDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Stunted);
                    if (stDiff != null)
                    {
                        if (!this.stuntedPawns.Contains(pawn))
                        {
                            this.stuntedPawns.Add(pawn);
                        }
                    }
                }
            }
            return keep;
        }

        private void RegrowLimbs(Pawn pawn, Hediff imDiff)
        {
            IEnumerable<Hediff> hediffs;
            float maxHp = this.settings.immortalStage1MaxPartSize;
            if (imDiff.Severity >= 10)
            {
                maxHp = this.settings.immortalStage5MaxPartSize;
            }
            else if (imDiff.Severity >= 7)
            {
                maxHp = this.settings.immortalStage4MaxPartSize;
            }
            else if (imDiff.Severity >= 4)
            {
                maxHp = this.settings.immortalStage3MaxPartSize;
            }
            else if (imDiff.Severity >= 2)
            {
                maxHp = this.settings.immortalStage2MaxPartSize;
            }
            else
            {
                maxHp = this.settings.immortalStage1MaxPartSize;
            }

            hediffs = pawn.health.hediffSet.hediffs.Where(hediff => hediff.def == HediffDefOf.MissingBodyPart && hediff.Part.def.hitPoints <= maxHp && !pawn.health.hediffSet.PartIsMissing(hediff.Part.parent) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff.Part));
            if (hediffs != null && hediffs.Any())
            {
                Hediff hediff = hediffs.RandomElement();
                if (hediff != null && hediff.Part != null)
                {
                    BodyPartRecord part = hediff.Part;
                    while (pawn.health.hediffSet.PartIsMissing(part.parent))
                    {
                        part = part.parent;
                    }
                    if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_regrowing))
                    {
                        if (!pawn.health.hediffSet.hediffs.Where(partDif => part.parent == partDif.Part && (partDif.def == HediffDefOf_Immortals.IH_regrowing || partDif.def.addedPartProps != null)).Any())
                        {
                            hediff = pawn.health.hediffSet.hediffs.Where(partDiff => partDiff.Part == part).First();
                            pawn.health.RemoveHediff(hediff);
                            hediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_regrowing, part);
                            hediff.Severity = 0.01f;
                        }
                    }
                    else
                    {
                        hediff = pawn.health.hediffSet.hediffs.Where(partDiff => partDiff.Part == part).First();
                        pawn.health.RemoveHediff(hediff);
                        hediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_regrowing, part);
                        hediff.Severity = 0.01f;
                    }
                }
            }
        }
        private void HealScars(Pawn pawn, Hediff imDiff, int tickMult)
        {
            IEnumerable<Hediff> scars;
            Hediff scar;

            scars = pawn.health.hediffSet.hediffs.InRandomOrder().Where(hediff => hediff.def.injuryProps != null && hediff.IsPermanent() && hediff.def != HediffDefOf.MissingBodyPart);

            float healValue = 0.000001125f;
            healValue *= imDiff.Severity * this.settings.baseHealSpeed * this.HealingFactor(pawn) * this.settings.scarHealSpeed * tickMult;

            if (scars != null && scars.Any())
            {
                scar = scars.First();
                if (scar != null)
                {
                    scar.Severity -= healValue;
                }
            }
        }

        private void RezPawn(Pawn pawn)
        {
            if (pawn != null && pawn.Corpse != null)
            {
                if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_ImpaledHeart))
                {
                    return;
                }

                Pawn_NeedsTracker pawnNeeds = pawn.needs;
                List<BodyPartRecord> partsToRestore = new();
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.GetType() == typeof(Hediff_MissingPart))
                    {
                        Hediff_MissingPart partDiff = hediff as Hediff_MissingPart;
                        if (partDiff.Part != null)
                        {
                            if (this.NeedPart(partDiff.Part))
                            {
                                partsToRestore.Add(partDiff.Part);
                            }
                        }
                    }
                }
                foreach (BodyPartRecord part in partsToRestore)
                {
                    pawn.health.RestorePart(part);
                }
                Pawn_HealthTracker health = pawn.health;
                HediffSet hediffSet = health.hediffSet;

                pawn.health.hediffSet = new HediffSet(pawn);
                ImmunityHandler immunity = health.immunity;
                pawn.health.immunity = new ImmunityHandler(pawn);

                List<BodyPartRecord> restoreParts = new();
                Caravan pawnCaravan = null;
                int tile = -1;
                if (!pawn.Corpse.Spawned)
                {
                    if (pawn.Corpse != null)
                    {
                        tile = pawn.Corpse.Tile;
                    }
                    if (this.deadPawnMaps[pawn] != null)
                    {
                        GenSpawn.Spawn(pawn.Corpse, pawn.PositionHeld, pawn.MapHeld);
                    }
                }
                if (tile != -1)
                {
                    foreach (Caravan caravan in Find.World.worldObjects.AllWorldObjects.Where(checkObject => checkObject is Caravan))
                    {
                        if (caravan.AllThings.Contains(pawn.Corpse))
                        {
                            pawnCaravan = caravan;
                        }
                    }
                }

                Thing storage = pawn.Corpse.StoringThing();
                if (storage != null)
                {
                    //pawn.Corpse.
                    GenSpawn.Spawn(pawn.Corpse, storage.Position, storage.Map);
                }
                ResurrectionUtility.Resurrect(pawn);

                if (pawnCaravan != null)
                {
                    pawnCaravan.AddPawn(pawn, false);
                }
                //pawn.SetFaction(DefDatabase<FactionDef>.GetNamed(""))
                foreach (Hediff hediff in hediffSet.hediffs)
                {
                    if (hediff.TendableNow())
                    {
                        if (hediff is HediffWithComps hediffWithComps)
                        {
                            HediffComp_TendDuration hediffComp_TendDuration = hediffWithComps.TryGetComp<HediffComp_TendDuration>();

                            hediffComp_TendDuration.tendQuality = 0f;
                            hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                        }
                    }
                    if (hediff.GetType() == typeof(Hediff_MissingPart))
                    {
                        Hediff_MissingPart partDiff = hediff as Hediff_MissingPart;
                        partDiff.IsFresh = false;
                    }
                }


                pawn.health.hediffSet = hediffSet;


                pawn.needs.AddOrRemoveNeedsAsAppropriate();
                //pawn.TakeDamage(dmg);

                if (pawn.health.immunity != null)
                {
                    pawn.health.immunity = immunity;
                }

                if (pawn.needs != null)
                {
                    if (pawn.needs.food != null)
                    {
                        pawn.needs.food.CurLevelPercentage = 0;
                    }

                    if (pawn.needs.rest != null && pawn.needs.rest.RestFallPerTick > 0)
                    {
                        pawn.needs.rest.CurLevelPercentage = 1;
                    }
                }
                Hediff dsHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_DeathStasis);
                if (dsHediff == null)
                {
                    dsHediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_DeathStasis);
                    dsHediff.Severity = 0.25f;
                }
                Hediff stHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_StuntedProc);
                if (stHediff != null)
                {
                    Hediff newStHediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_Stunted);
                    newStHediff.Severity = Rand.Range(1, 4);
                    pawn.health.RemoveHediff(stHediff);
                }

                Hediff imHediff = pawn.health.hediffSet.GetHediffs<Immortal_Hediff>().First();

                if (imHediff.Severity <= 0.5f)
                {
                    Hediff retHediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_revivedFirst);
                    retHediff.Severity = ((float)Rand.Range(50, 125)) / 100;
                    imHediff.Severity = 1f;
                }
                else
                {
                    Hediff retHediff;
                    if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_revived))
                    {
                        retHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_revived);
                    }
                    else
                    {
                        retHediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_revived);
                    }

                    if (retHediff != null)
                    {
                        retHediff.Severity = ((float)Rand.Range(50, 125)) / 100;
                    }

                    if (this.settings.deathEffectVal != 0)
                    {
                        imHediff.Severity += this.settings.deathEffectVal;
                    }
                    switch (this.settings.deathEffectVal)
                    {
                        case -1: imHediff.Severity -= 0.1f; break;
                        case 1: imHediff.Severity += 0.1f; break;
                    }
                    if (imHediff.Severity < 0.6)
                    {
                        imHediff.Severity = 0.6f;
                    }
                }

                if (this.settings.immortalAccumulateFoodNeed != 0)
                {
                    if (pawn.needs != null && pawn.needs.food != null)
                    {
                        if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_hungerHolder))
                        {
                            Hediff foodHoldHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_hungerHolder);
                            Hediff foodHediff;
                            if (pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_theHunger))
                            {
                                foodHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_theHunger);
                            }
                            else
                            {
                                foodHediff = pawn.health.AddHediff(HediffDefOf_Immortals.IH_theHunger);
                            }

                            foodHediff.Severity += foodHoldHediff.Severity * pawn.needs.food.FoodFallPerTick * 10;
                        }
                    }
                }
                this.AddImmortal(pawn, true);
            }

        }

        private static bool CheckDistance(IntVec3 vec1, IntVec3 vec2, int distance)
        {
            if (GetDistance(vec1, vec2) < distance)
            {
                return true;
            }

            return false;
        }

        private static int GetDistance(IntVec3 vec1, IntVec3 vec2)
        {
            int val = Mathf.RoundToInt((float)Math.Sqrt((Math.Pow(Math.Abs(vec1.x - vec2.x), 2) + Math.Pow(Math.Abs(vec1.z - vec2.z), 2))));
            return val;

        }


        public override void StartedNewGame()
        {
            this.StartUp();
        }

        public override void LoadedGame()
        {
            this.StartUp();
        }

        public void RemoveImmortals()
        {
            foreach (Pawn pawn in this.immortalPawns)
            {
                this.ClearPawn(pawn);
            }
            this.immortalPawns = new List<Pawn>();
            foreach (Pawn pawn in this.deadPawns)
            {
                this.ClearPawn(pawn);
            }

        }

        public static void RollImmortals()
        {
            foreach (Pawn pawn in Find.CurrentMap.mapPawns.AllPawns.Concat(Find.WorldPawns.AllPawnsAliveOrDead))
            {
                ChanceAddPawn(pawn);
            }
        }

        public void ClearPawn(Pawn pawn)
        {
            Hediff removeDif = pawn.GetImmortalHediff();
            if (removeDif != null)
            {
                pawn.health.RemoveHediff(removeDif);
            }

            removeDif = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_DeathStasis);
            if (removeDif != null)
            {
                pawn.health.RemoveHediff(removeDif);
            }

            removeDif = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_DeathTimer);
            if (removeDif != null)
            {
                pawn.health.RemoveHediff(removeDif);
            }

            removeDif = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Stunted);
            if (removeDif != null)
            {
                pawn.health.RemoveHediff(removeDif);
            }

            while (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_regrowing) != null)
            {
                pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_regrowing));
            }
        }

        public static void ChanceAddPawn(Pawn pawn)
        {
            if (Immortals_DesignatorUtility.HasConciousnessPart(pawn))
            {
                ImmortalsHarmony.ChanceAddImmortal(pawn);
            }
        }

        private void SetRecovery(Pawn pawn)
        {
            //if (pawn.health.hediffSet.has)
            this.deadPawns.Add(pawn);
            float minHeal = 0.25f;
            float maxHeal = 1f;
            Hediff imHediff = pawn.GetImmortalHediff();
            Hediff dsHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_DeathStasis);
            if (imHediff != null)
            {
                if (imHediff.Severity <= 0.5f)
                {
                    minHeal = 0.01f;
                    maxHeal = 0.5f;
                }

                Dictionary<float, float> healValues = new();
                IEnumerable<Hediff> hediffs;
                hediffs = pawn.health.hediffSet.hediffs.Where(hediff => this.settings.HediffCureToRevive(hediff));
                float hediffNum = hediffs.Count();
                foreach (Hediff hediff in hediffs)
                {
                    float val = 0f;
                    if (dsHediff == null || (dsHediff != null && dsHediff.Severity < 0.25f))
                    {
                        val = Rand.Range(minHeal, maxHeal);
                        if (hediff.def.lethalSeverity != -1)
                        {
                            val /= 10;
                        }

                        val /= (hediffNum / 10);
                        if (hediff.def == HediffDefOf_Immortals.IH_DeathTimer)
                        {
                            val = 0;
                        }
                    }

                    healValues.Add(hediff.loadID, val);
                }
                if (this.recoverValues.ContainsKey(pawn))
                {
                    this.recoverValues[pawn] = healValues;
                }
                else
                {
                    this.recoverValues.Add(pawn, healValues);
                }
            }

        }

        public void AddImmortal(Pawn pawn, bool flash)
        {
            if (pawn != null)
            {
                if (this.immortalPawns == null)
                {
                    Log.Message("Pawns is null");
                }

                if (this.immortalPawns.Contains(pawn))
                {
                    return;
                }

                if (this.CanHeal(pawn))
                {
                    if (pawn.Map != null && flash)
                    {
                        pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_Quickening(pawn.Map, pawn.Position, 0));
                    }

                    this.immortalPawns.Add(pawn);
                    if (this.deadPawns.Contains(pawn))
                    {
                        this.deadPawns.Remove(pawn);
                    }
                    if (this.deadPawnMaps.ContainsKey(pawn))
                    {
                        this.deadPawnMaps.Remove(pawn);
                    }
                }
                IEnumerable<Hediff> hediffs = pawn.health.hediffSet.hediffs.ToList();
                IEnumerable<Hediff> missingDifs = pawn.health.hediffSet.hediffs.Where(checkDif => checkDif.def == HediffDefOf.MissingBodyPart);
                foreach (Hediff hediff in hediffs)
                {
                    if (hediff != null && hediff.def == HediffDefOf_Immortals.IH_regrowing)
                    {
                        Regrowing_Hediff regrowDif = hediff as Regrowing_Hediff;
                        if (regrowDif.forPart != null)
                        {
                            foreach (Hediff partDif in missingDifs)
                            {
                                if (partDif.Part == regrowDif.forPart)
                                {
                                    pawn.health.RemoveHediff(partDif);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            pawn.health.RemoveHediff(regrowDif);
                        }
                    }
                }
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff != null && hediff.def == HediffDefOf_Immortals.IH_regrowing)
                    {
                        if (hediff is Regrowing_Hediff growDiff && growDiff.forPart != null)
                        {
                            hediff.Part = growDiff.forPart;
                            growDiff.partHp = (int)Math.Round(growDiff.Part.def.hitPoints * pawn.HealthScale);
                        }
                    }
                }
            }
        }

        public void AddDeadImmortal(Pawn pawn, bool strike = true)
        {
            if (pawn == null)
            {
                return;
            }

            if (this.immortalPawns.Contains(pawn))
            {
                this.immortalPawns.Remove(pawn);
            }

            if (pawn.IsImmmortal())
            {
                Hediff dsHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_DeathStasis);
                if (dsHediff != null)
                {
                    dsHediff.Severity = 1;
                }

                if (Immortals_DesignatorUtility.HasConciousnessPart(pawn) || this.settings.noBrainCheat)
                {
                    if (pawn.Corpse != null && pawn.Corpse.Map == null)
                    {
                        Map eventMap;
                        //eventMap = CaravanIncidentUtility.GetOrGenerateMapForIncident()
                    }
                    if (!pawn.health.hediffSet.HasHediff(HediffDefOf_Immortals.IH_DeathTimer))
                    {
                        pawn.health.AddHediff(HediffDefOf_Immortals.IH_DeathTimer);
                        pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_DeathTimer).Severity = 3f;
                    }
                    if (pawn.Corpse != null)
                    {
                        if (!this.deadPawnMaps.ContainsKey(pawn))
                        {
                            this.deadPawnMaps.Add(pawn, pawn.Corpse.MapHeld);
                        }
                        else
                        {
                            this.deadPawnMaps[pawn] = pawn.Corpse.MapHeld;
                        }
                    }
                    IEnumerable<Hediff> pawnHediffs = pawn.health.hediffSet.hediffs.ToList();
                    foreach (Hediff hediff in pawnHediffs)
                    {
                        if (hediff.GetType() == typeof(Hediff_MissingPart))
                        {
                            Hediff_MissingPart partDiff = hediff as Hediff_MissingPart;
                            if (partDiff.Part != null)
                            {
                                if ((this.NeedPart(partDiff.Part) && !pawn.health.hediffSet.hediffs.Where(checkDif => checkDif is Regrowing_Hediff growCheck && growCheck.forPart == partDiff.Part).Any()))
                                {
                                    BodyPartRecord part = partDiff.Part;
                                    Regrowing_Hediff newReGrowing = pawn.health.AddHediff(HediffDefOf_Immortals.IH_regrowing) as Regrowing_Hediff;
                                    newReGrowing.forPart = part;
                                    newReGrowing.Severity = 0.01f;
                                    newReGrowing.partHp = part.def.hitPoints;
                                }
                            }
                        }
                    }
                    this.SetRecovery(pawn);
                }
                else
                {
                    this.CheckQuickening(pawn);
                }
            }
        }
        #region HealChecks

        private void CheckQuickening(Pawn pawn)
        {
            if (pawn != null && pawn.Dead && pawn.Corpse != null || (this.deadPawnMaps.ContainsKey(pawn) && this.deadPawnMaps[pawn] != null))
            {
                // Hediff imHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
                // if (imHediff != null && (imHediff.Severity > 0.5 || this.settings.firstDeathQuickening))
                if (pawn.IsImmmortal(out Hediff imHediff) && (imHediff.Severity > 0.5 || this.settings.firstDeathQuickening))
                {
                    Map map = null;
                    if (pawn.Corpse != null)
                    {
                        map = pawn.Corpse.MapHeld;
                    }
                    else if (this.deadPawnMaps.ContainsKey(pawn))
                    {
                        map = this.deadPawnMaps[pawn];
                    }

                    if (map != null)
                    {
                        map.weatherManager.eventHandler.AddEvent(new WeatherEvent_Quickening(map, pawn.Position, 0));
                    }

                    int closest = 20;
                    Pawn closestPawn = null;
                    foreach (Pawn other in this.immortalPawns)
                    {
                        if (other != null && other.MapHeld != null && (pawn.Corpse != null && pawn.Corpse.MapHeld == other.MapHeld) || (this.deadPawnMaps.ContainsKey(pawn) && other.MapHeld == this.deadPawnMaps[pawn]))
                        {
                            // Hediff otherImHediff = other.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
                            // if (otherImHediff != null)
                            if (other.IsImmmortal())
                            {
                                int checkDist = GetDistance(pawn.Position, other.Position);
                                if (checkDist < closest)
                                {
                                    closest = checkDist;
                                    closestPawn = other;
                                }
                            }
                        }
                    }
                    if (closestPawn != null)
                    {
                        this.DoQuickening(pawn, closestPawn);
                    }
                    else
                    {
                        GameConditionDef def = DefDatabase<GameConditionDef>.GetNamed("IH_QuickeningSky");
                        map.weatherManager.eventHandler.AddEvent(new WeatherEvent_Quickening(map, pawn.Position, 15, false));
                        GameCondition flashStorm = GameConditionMaker.MakeCondition(def, 4000);
                        pawn.health.RemoveHediff(imHediff);
                        map.GameConditionManager.RegisterCondition(flashStorm);
                    }
                    this.clearPawn(pawn);
                }
                else if (imHediff != null)
                {
                    pawn.health.RemoveHediff(imHediff);
                    this.clearPawn(pawn);
                }
                else
                {
                    this.clearPawn(pawn);
                }
            }
        }









        private void DoQuickening(Pawn giver, Pawn taker)
        {
            Hediff imHediff = giver.GetImmortalHediff();
            Hediff stHediff = taker.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Stunted);
            GameConditionDef def = DefDatabase<GameConditionDef>.GetNamed("IH_Quickening");
            taker.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_Quickening(taker.Map, giver.Position, 0));
            float powerTransfer = 0;
            Immortal_Class immortalClassGiver = Immortals_DesignatorUtility.GetPawnType(giver);
            Immortal_Class immortalClassTaker = Immortals_DesignatorUtility.GetPawnType(taker);

            float immortality = imHediff.Severity;
            if (immortalClassGiver != null && immortalClassGiver.immortalTransferOut != null)
            {
                immortality *= immortalClassGiver.immortalTransferOut.Value;
            }
            else
            {
                immortality *= this.settings.immortalTransferOut;
            }

            if (immortalClassTaker != null && immortalClassTaker.immortalTransferIn != null)
            {
                immortality *= immortalClassTaker.immortalTransferIn.Value;
            }
            else
            {
                immortality *= this.settings.immortalTransferIn;
            }

            immortality *= this.TransferFactor(giver, taker);
            if (stHediff == null)
            {
                taker.GetImmortalHediff().Severity += immortality;
            }
            else
            {
                if (immortality > stHediff.Severity)
                {
                    immortality = powerTransfer - stHediff.Severity;
                    imHediff.Severity += immortality;
                    taker.health.RemoveHediff(stHediff);
                }
                else
                {
                    stHediff.Severity -= immortality;
                }
            }

            //Transfer Skills
            if (giver.skills != null && taker.skills != null)
            {
                int passions = 0;
                int passionChances = (int)this.settings.passionChances;
                if (this.settings.passionChance != 0)
                {
                    if (this.settings.passionLimit != -1)
                    {
                        foreach (SkillRecord skill in taker.skills.skills)
                        {
                            if (skill.passion == Passion.Minor)
                            {
                                passions++;
                            }
                            else if (skill.passion == Passion.Major)
                            {
                                passions += 2;
                            }
                        }
                    }
                    else
                    {
                        passions = -2000;
                    }

                    foreach (SkillRecord skill in giver.skills.skills.InRandomOrder())
                    {
                        if (passionChances == 0 && passions >= this.settings.passionLimit)
                        {
                            break;
                        }

                        SkillRecord takerSkill = taker.skills.skills.Where(checkSkill => checkSkill.def == skill.def).First();

                        if (takerSkill != null && !takerSkill.TotallyDisabled && takerSkill.passion != Passion.Major)
                        {
                            if (skill.passion != Passion.None)
                            {
                                if (Rand.Range(0, 1) <= this.settings.passionChance)
                                {
                                    passionChances--;
                                    passions++;
                                    if (skill.passion == Passion.Major)
                                    {
                                        if (takerSkill.passion == Passion.None)
                                        {
                                            takerSkill.passion = Passion.Minor;
                                        }
                                        else
                                        {
                                            takerSkill.passion = Passion.Major;
                                        }
                                    }
                                    else
                                    {
                                        takerSkill.passion = Passion.Minor;
                                    }
                                }
                            }
                        }

                        float xpToLearn;
                        xpToLearn = Rand.Range(0, (skill.XpTotalEarned / 10) * Rand.Range(0.1f, 2f));
                        taker.skills.Learn(skill.def, xpToLearn * this.settings.learningMult, true);
                    }
                }
            }
            giver.health.RemoveHediff(imHediff);
            GameCondition quickening = GameConditionMaker.MakeCondition(def, 10000);
            taker.Map.GameConditionManager.RegisterCondition(quickening);
            (quickening as GameCondition_Quickening).pawn = taker;
        }

        private void clearPawn(Pawn pawn)
        {
            if (pawn != null)
            {
                if (this.deadPawns.Contains(pawn))
                {
                    this.deadPawns.Remove(pawn);
                }

                if (this.immortalPawns.Contains(pawn))
                {
                    this.immortalPawns.Remove(pawn);
                }
            }

        }

        private float GetHealSpeed(Pawn pawn)
        {
            float speed = 0;

            // Hediff imDiff = null;
            Hediff nemDiff = null;
            Hediff koDiff = null;
            nemDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_NemnirHigh);
            // imDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
            koDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_KomaHigh);

            // if (imDiff != null && imDiff.Severity > 0.5f)
            if (pawn.IsImmmortal(out Hediff imDiff) && imDiff.Severity > 0.5f)
            {
                speed = imDiff.Severity;
            }

            if (nemDiff != null)
            {
                if (speed == 0)
                {
                    speed = 1 + nemDiff.Severity;
                }
                else
                {
                    speed *= 1 + nemDiff.Severity;
                }
            }
            if (koDiff != null)
            {
                if (speed == 0)
                {
                    speed = 5;
                }
                else
                {
                    speed *= 4;
                    if (speed < 5)
                    {
                        speed = 5;
                    }
                }
            }
            if (speed != 0)
            {
                speed = baseHealSpeed * this.settings.baseHealSpeed * speed;
            }


            return speed;
        }

        private bool CanHeal(Pawn pawn)
        {
            // Hediff imDiff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
            // if (imDiff != null && imDiff.Severity > 0.1f)
            if (pawn.IsImmmortal(out Hediff imDiff) && imDiff.Severity > 0.1f)
            {
                return true;
            }

            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_NemnirHigh) != null)
            {
                return true;
            }

            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_KomaHigh) != null)
            {
                return true;
            }

            return false;
        }

        private float HealingFactor(Pawn pawn)
        {
            return Mathf.Pow(pawn.BodySize, this.settings.immortalHealingSizeFactor);
        }
        private float TransferFactor(Pawn giver, Pawn taker)
        {
            return Mathf.Pow(giver.BodySize / taker.BodySize, this.settings.immortalTransferSizeFactor);
        }

        private bool NeedPart(BodyPartRecord part)
        {
            if (part != null)
            {
                foreach (string need in this.sourceNeeds)
                {
                    foreach (BodyPartTagDef tag in part.def.tags)
                    {
                        if (tag.vital)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        /*
        public bool NeedToHeal(Hediff hediff, bool dead)
        {
            if (hediff.def == deathStasisHediff || hediff.def == stuntedHediff || hediff.def == stuntedBurnHediff)
                return false;
            if (hediff.def.injuryProps != null && hediff.def != missingHediff && hediff.IsPermanent())
                return false;
            if (settings.hediffSettings.ContainsKey(hediff.def))
                return !settings.hediffSettings[hediff.def].canGet.Value;
            if (hediff.def.lethalSeverity != -1 && ((hediff.def.tendable && !dead) || dead))
                return true;
            if (hediff.def.injuryProps != null && hediff.def != missingHediff && !hediff.IsPermanent())
                return true;
            if (hediff.def == growingHediff)
                return true;
            if (hediff.def == timerHediff)
                return true;
            return false;
        }

        public bool NeedToRecover(Hediff hediff, Pawn pawn)
        {
            if (NeedToHeal(hediff, pawn.Dead))
            {
                if (!recoverValues[pawn].ContainsKey(hediff.loadID) || (hediff.def == growingHediff && hediff.Severity >= 0.5f))
                {
                    return false;
                }
                else if (recoverValues[pawn][hediff.loadID] <= hediff.Severity)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SlowToRecover(Hediff hediff)
        {
            if (hediff.def.lethalSeverity != -1)
                return true;
            if (hediff.def.injuryProps == null)
                return true;
            if (hediff.def == growingHediff)
                return true;

            return false;
        }

        private bool ToCure(Hediff hediff)
        {
            if (settings.hediffSettings.ContainsKey(hediff.def))
            {
                return !settings.hediffSettings[hediff.def].canGet.Value;
            }
            if (hediff.def.tendable)
            {
                if (settings.canGetConditions)
                    return false;
                else
                    return true;
            }
            else
            {
                if (settings.canGetDisease)
                    return false;
                else
                    return true;
            }

            return false;
        }
        */

        #endregion HealChecks
        private void StartUp()
        {
            if (this.immortalPawns == null)
            {
                this.immortalPawns = new List<Pawn>();
            }

            if (this.deadPawns == null)
            {
                this.deadPawns = new List<Pawn>();
            }

            if (this.deadPawnMaps == null)
            {
                this.deadPawnMaps = new Dictionary<Pawn, Map>();
            }

            if (this.pawnsToRez == null)
            {
                this.pawnsToRez = new List<Pawn>();
            }

            if (this.stuntedPawns == null)
            {
                this.stuntedPawns = new List<Pawn>();
            }

            this.rareTick = 0;

            this.settings = LoadedModManager.GetMod<Immortals_Mod>().GetSettings<Immortals_Settings>();
            //ReLoadAilments();
            DamageDef dmgDef = DamageDefOf.Blunt;
            this.stuntedDmgDef = DefDatabase<DamageDef>.GetNamed("IH_internalBurn");
            this.dmg = new DamageInfo(dmgDef, 1, 0);
            this.sourceNeeds = new List<string>
            {
                "ConsciousnessSource",
                "BloodPumpingSource",
                "BloodFiltrationLiver",
                "BreathingSource",
                "MetabolismSource"
            };

            // immortalHediff           = HeDiffDefOf_Immortals.IH_Immortal;
            // this.deathStasisHediff   = HeDiffDefOf_Immortals.IH_DeathStasis;
            // this.stuntedHediff       = HeDiffDefOf_Immortals.IH_Stunted;
            // this.stuntedProcHediff   = HeDiffDefOf_Immortals.IH_StuntedProc;
            // this.stuntedBurnHediff   = HeDiffDefOf_Immortals.IH_Burn;
            // this.timerHediff         = HeDiffDefOf_Immortals.IH_DeathTimer;
            // this.growingHediff       = HeDiffDefOf_Immortals.IH_regrowing;
            // this.missingHediff       = HediffDefOf.MissingBodyPart;
            // this.adjustingHediff     = HeDiffDefOf_Immortals.IH_adjusting;
            // this.returnedHediff      = HeDiffDefOf_Immortals.IH_revived;
            // this.firstReturnedHediff = HeDiffDefOf_Immortals.IH_revivedFirst;
            // this.hungerHediff        = HeDiffDefOf_Immortals.IH_theHunger;
            // this.hungerHolderHediff  = HeDiffDefOf_Immortals.IH_hungerHolder;
            // this.impaledHeart        = HeDiffDefOf_Immortals.IH_ImpaledHeart;

            // this.nemnirHediff          = HeDiffDefOf_Immortals.IH_NemnirHigh;
            // this.komaHediff            = HeDiffDefOf_Immortals.IH_KomaHigh;
            // this.mortalisCrystalHediff = HeDiffDefOf_Immortals.IH_MortalisCrystalImplant;

            // this.halfCyclerHediff = HeDiffDefOf_Immortals.CircadianHalfCycler;
            this.recoverValues = new Dictionary<Pawn, Dictionary<float, float>>();

            //getsPermanent = DefDatabase<HediffComp>.GetNamed("HediffComp_GetsPermanent");

            IEnumerable<Pawn> allImmortalPawns = Find.CurrentMap.mapPawns.AllPawns.Concat(Find.WorldPawns.AllPawnsAliveOrDead);

            foreach (Pawn pawn in allImmortalPawns)
            {
                if (pawn != null && (!pawn.Dead || pawn.Corpse != null) && this.CanHeal(pawn))
                {
                    if (Immortals_DesignatorUtility.HasConciousnessPart(pawn))
                    {
                        if (pawn.Dead)
                        {
                            this.AddDeadImmortal(pawn, false);
                        }
                        else
                        {
                            this.AddImmortal(pawn, false);
                        }
                    }
                }
            }

            this.loaded = true;
            this.firstTick = true;
        }


        /*
        public void ReLoadAilments()
        {
            cureAilments = new List<string>();

            cureAilments = new List<string>()
            {
                    "WoundInfection",
                    "Flu",
                    "Plague",
                    "Malaria"
            };

            deadAilments = new List<string>
            {
                "BloodLoss",
                "Hypothermia",
                "Heatstroke",
                "WoundInfection",
                "Flu",
                "Plague",
                "Malaria",
                "IH_DeathTimer"
            };

            livingAilments = new List<string>()
            {
                "BloodLoss",
                "Hypothermia",
                "Heatstroke"
            };
        }

        /* Work Priority Saving
        private void SaveWorkPriorities(Pawn pawn)
        {
            Dictionary<string, int> workValues = new Dictionary<string, int>();
            foreach (WorkTypeDef workDef in Verse.DefDatabase<WorkTypeDef>.AllDefs)
            {
                workValues.Add(workDef.defName, pawn.workSettings.GetPriority(workDef));
            }
            if (workPriorities.ContainsKey(pawn.GetUniqueLoadID()))
                workPriorities[pawn.GetUniqueLoadID()] = workValues;
            else
                workPriorities.Add(pawn.GetUniqueLoadID(), workValues);
        }

        private void LoadWorkPriorities(Pawn pawn)
        {
            string loadId = pawn.GetUniqueLoadID();
            if (workPriorities.ContainsKey(loadId))
            {
                Pawn_WorkSettings workSettings = new Pawn_WorkSettings(pawn);
                workSettings.EnableAndInitializeIfNotAlreadyInitialized();
                foreach (WorkTypeDef workDef in Verse.DefDatabase<WorkTypeDef>.AllDefs)
                {
                    if (workPriorities[loadId].ContainsKey(workDef.defName))
                    {
                        workSettings.SetPriority(workDef, workPriorities[loadId][workDef.defName]);
                    }
                }
                pawn.workSettings = workSettings;
            }
        }
        */
    }
}