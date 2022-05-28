using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace Immortals
{

    public class Immortals_Settings : ModSettings
    {


        //General Settings

        /// <summary>
        /// THe tick rate for immortals while running at 1x
        /// </summary>
        public float immortalTickRateSingle = 50;
        /// <summary>
        /// The tick rate for immortals while running at anytghing above 1x
        /// </summary>
        public float immortalTickRate = 250;
        /// <summary>
        /// The multiplier for rare ticks, after this many normal ticks it will do a rare tick. Thats where limbs regrow and hediffs are removed.
        /// </summary>
        public float immortalTickRateRare = 25;
        /// <summary>
        /// Turns off or makes the flashstorms safe from non-quickening immortal beheadings
        /// </summary>
        public bool disableFlashstorm = false;
        /// <summary>
        /// Enables/Disables whether immortal who have not had their first death grant a quickening
        /// </summary>
        public bool firstDeathQuickening = false;
        /// <summary>
        /// Adds the immortality number to the end of the hediff.
        /// </summary>
        public bool revealImmortalityNumber = false;

        //General Immortality Settings

        /// <summary>
        /// Changes the level of immortality of a pawn after death by this amount
        /// </summary>
        public float deathEffectVal = 0;
        /// <summary>
        /// THe general heal speed of an immortal pawn
        /// </summary>
        public float baseHealSpeed = 1;
        /// <summary>
        /// A multiplier of base heal speed for a pawn who has died their first death.
        /// </summary>
        public float firstDeathHealFactor = 0.1f;
        /// <summary>
        /// The multiplier for how much food is accumulated from regrowing limbs and organs
        /// </summary>
        public float immortalAccumulateFoodNeed = 1f;
        /// <summary>
        /// How much the size difference between 2 immortals matters when calculating gained immortality
        /// </summary>
        public float immortalTransferSizeFactor = 0.5f;
        /// <summary>
        /// How much the size of a creature affects how fast it heals.
        /// </summary>
        public float immortalHealingSizeFactor = 0.5f;
        /// <summary>
        /// The healspeed of scars
        /// </summary>
        public float scarHealSpeed = 1f;

        //Default Immortality

        /// <summary>
        /// The chance an immortal will spawn pre-first death
        /// </summary>
        public float immortalChance = 0.05f;
        /// <summary>
        /// The chance an immortal will spawn with visible immorality
        /// </summary>
        public float immortalChanceVisible = 0.1f;
        /// <summary>
        /// The minimum value of immortality a visible immortal will spawn with
        /// </summary>
        public float immortalSpawnMin = 1f;
        /// <summary>
        /// The maximum value of immortality a visible immortal will spawn with
        /// </summary>
        public float immortalSpawnMax = 5f;
        /// <summary>
        /// A multiplier for how much immortality a pawn gains from a quickening
        /// </summary>
        public float immortalTransferIn = 0.2f;
        /// <summary>
        /// A multiplier for how much immortality a pawn gives from a quickening
        /// </summary>
        public float immortalTransferOut = 1f;


        //Immortality Classes
        /// <summary>
        /// Have we loaded the pawnTypes
        /// </summary>
        public bool pawnTypesLoaded = false;

        /// <summary>
        /// The list of default pawns
        /// </summary>
        public List<ThingDef> defaultPawns = new();
        /// <summary>
        /// The list of immortal classes
        /// </summary>
        public List<Immortal_Class> immortalClasses = new();


        //Others
        /// <summary>
        /// A not yet implement heavily requested feature
        /// </summary>
        public bool noBrainCheat = false;



        //Quickening Transfer Stuffs
        /// <summary>
        /// A multiplier for experience an immortal gains from a quickening(from the beheaded pawn)
        /// </summary>
        public float learningMult = 1f;
        /// <summary>
        /// The chance a pawn swaps traits with the beheaded pawn
        /// </summary>
        public float swapTraitsChance = 0;
        /// <summary>
        /// The chance a pawn will gain some passions from the beheaded pawn
        /// </summary>
        public float passionChance = 0.05f;
        /// <summary>
        /// The maximum amount of passion gains
        /// </summary>
        public float passionChances = 3;
        /// <summary>
        /// Chance for a pawn to lose a passion
        /// </summary>
        public float passionLoss = 0;
        /// <summary>
        /// The maximum number of passions a pawn can have, past this value they will no longer gain any from beheading. -1 for no limit
        /// </summary>
        public float passionLimit = -1;


        //Healing Stuffs

        /// <summary>
        /// How fast pawns regrow limbs
        /// </summary>
        public float immortalRegrowSpeed = 1f;
        /// <summary>
        /// Do pawns accumulate food need from regrowing limbs
        /// </summary>
        public bool immortalRegrowCostsFood = true;
        /// <summary>
        /// The food cost multiplier for a pawn regrowing limbs, per limb
        /// </summary>
        public float immortalRegrowFoodCost = 0.5f;
        /// <summary>
        /// The maximum number of parts a pawn can be regrowing at a time(mostly for limbs)
        /// </summary>
        public float immortalRegrowMaxParts = 1f;

        /// <summary>
        /// The maximum part size a lesser immortal will regrow
        /// </summary>
        public float immortalStage0MaxPartSize = 0f;
        /// <summary>
        /// THe maximum part size a default immortal will regrow
        /// </summary>
        public float immortalStage1MaxPartSize = 9f;
        /// <summary>
        /// The maximum part size a greater immortal will regrow
        /// </summary>
        public float immortalStage2MaxPartSize = 20f;
        /// <summary>
        /// The maximum part size a high immortal will regrow
        /// </summary>
        public float immortalStage3MaxPartSize = 35f;
        /// <summary>
        /// The maximum part size a grand immortal will regrow
        /// </summary>
        public float immortalStage4MaxPartSize = 40f;
        /// <summary>
        /// The maximum part size a apex immortal will regrow
        /// </summary>
        public float immortalStage5MaxPartSize = 50f;


        //Diseases
        /// <summary>
        /// The maximum progress a condition(non-tendable lethal at 1) can progress to
        /// </summary>
        public float hediffConditionMax = 1f;
        /// <summary>
        /// The maximum progress a disease(tendable lethal at 1) can progress to
        /// </summary>
        public float hediffDiseaseMax = 1f;

        /// <summary>
        /// Can immortal pawns get diseases(tendable lethal at 1)
        /// </summary>
        public bool canGetDisease = true;
        /// <summary>
        /// Can immortal pawns get conditions(non-tendable lethal at 1)
        /// </summary>
        public bool canGetConditions = false;
        /// <summary>
        /// Can immortals die to diseases
        /// </summary>
        public bool canDieToDisease = true;
        public float diseaseMaxProg = 1f;
        public float conditionMaxProg = 1f;





        //Stunted Immortals Stuff


        //Immortality Stealing
        /// <summary>
        /// The chance a stunted immortal will burn themselves
        /// </summary>
        public float burnChance = 0.05f;
        /// <summary>
        /// The multiplier for the severity of stunted burns
        /// </summary>
        public float burnMult = 1;

        /// <summary>
        /// The chance a pawn will become a stunted immortal when they gain immortality via eating a head
        /// </summary>
        public float stuntedChanceHead = 0.25f;
        /// <summary>
        /// The chance a pawn will become a stunted immortal when they gain immortality via the drug that doesnt exist yet
        /// </summary>
        public float stuntedChanceDrug = 1;

        /// <summary>
        /// The chance of gaining immortality when eating a default immortals head
        /// </summary>
        public float stage1StealChance = 0.02f;
        /// <summary>
        /// The chance of gaining immortality when eating a greater immortals head
        /// </summary>
        public float stage2StealChance = 0.07f;
        /// <summary>
        /// The chance of gaining immortality when eating a high immortals head
        /// </summary>
        public float stage3StealChance = 0.15f;
        /// <summary>
        /// The chance of gaining immortality when eating a grand immortals head
        /// </summary>
        public float stage4StealChance = 0.25f;
        /// <summary>
        /// The chance of gaining immortality when eating a apex immortals head
        /// </summary>
        public float stage5StealChance = 1;

        /// <summary>
        /// No implemented
        /// </summary>
        public float stewChanceMult = 1;


        //Full customized stuffs
        /*
        public Dictionary<ThingDef, int> pawnTypeCustomizations;
        public Dictionary<string, int> pawnTypeSaving;
        public List<string> pawnTypes;
        */

        public bool hediffSettingsLoaded = false;
        public Dictionary<HediffDef, Hediff_Setting> hediffSettings;
        public Dictionary<string, Hediff_Setting> hediffSettingsSave;

        public List<HediffDef> oldAgeDefs = new();


        //Working Hediffs
        public bool hediffsLoaded = false;

        // public HediffDef immortalHediff;
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
        // 
        // HediffDef nemnirHediff;
        // HediffDef komaHediff;
        // HediffDef mortalisCrystalHediff;
        // HediffDef mortalisBuildupHediff;

        List<HediffDef> immortalHediffs;

        float slowHealSpeed = 0.1f;
        List<HediffDef> baseHealHediffs;
        List<HediffDef> baseIgnoreHediffs;


        public override void ExposeData()
        {

            //General Settings


            Scribe_Values.Look(ref this.immortalTickRateSingle, "immortalTickRateSingle", 50);
            Scribe_Values.Look(ref this.immortalTickRate, "immortalTickRate", 250);
            Scribe_Values.Look(ref this.immortalTickRateRare, "immortalTickRateRate", 25);

            Scribe_Values.Look(ref this.revealImmortalityNumber, "revealImmortalityNumber", false);
            Scribe_Values.Look(ref this.disableFlashstorm, "disableFlashstorm", false);
            Scribe_Values.Look(ref this.firstDeathQuickening, "firstDeathQuickening", false);

            //General Immortality Settings
            Scribe_Values.Look(ref this.deathEffectVal, "deathEffectOnVal", 1);


            Scribe_Values.Look(ref this.baseHealSpeed, "baseHealSpeed", 1f);
            Scribe_Values.Look(ref this.firstDeathHealFactor, "firstDeathHealFactor", 0.1f);
            Scribe_Values.Look(ref this.immortalTransferSizeFactor, "immortalTransferSizeFactor", 0.5f);
            Scribe_Values.Look(ref this.immortalHealingSizeFactor, "immortalHealingSizeFator", 0.5f);

            Scribe_Values.Look(ref this.scarHealSpeed, "scarHealSpeed", 1);

            //Default Immortality

            Scribe_Values.Look(ref this.immortalChance, "immortalChance", 0.05f);
            Scribe_Values.Look(ref this.immortalTransferIn, "immortalTransferIn", 0.2f);
            Scribe_Values.Look(ref this.immortalTransferOut, "immortalTransferOut", 1f);
            Scribe_Values.Look(ref this.immortalSpawnMin, "immortalChanceMin", 0.6f);
            Scribe_Values.Look(ref this.immortalSpawnMax, "immortalChanceMax", 5f);
            Scribe_Values.Look(ref this.immortalChanceVisible, "immortalChanceVisible", 0.1f);



            //Regrowth

            Scribe_Values.Look(ref this.immortalStage0MaxPartSize, "immortalStage0MaxPartSize", 0);
            Scribe_Values.Look(ref this.immortalStage1MaxPartSize, "immortalStage1MaxPartSize", 9);
            Scribe_Values.Look(ref this.immortalStage2MaxPartSize, "immortalStage2MaxPartSize", 20);
            Scribe_Values.Look(ref this.immortalStage3MaxPartSize, "immortalStage3MaxPartSize", 35);
            Scribe_Values.Look(ref this.immortalStage4MaxPartSize, "immortalStage4MaxPartSize", 40);
            Scribe_Values.Look(ref this.immortalStage5MaxPartSize, "immortalStage5MaxPartSize", 50);
            Scribe_Values.Look(ref this.immortalAccumulateFoodNeed, "immortalAccumulateFoodNeed", 1f);
            Scribe_Values.Look(ref this.immortalRegrowSpeed, "immortalRegrowSpeed", 1f);
            Scribe_Values.Look(ref this.immortalRegrowFoodCost, "imomrtalRegrowFoodCost", 0.5f);
            Scribe_Values.Look(ref this.immortalRegrowCostsFood, "immortalRegrowCostsFood", true);
            Scribe_Values.Look(ref this.immortalRegrowMaxParts, "immortalRegrowMaxParts", -1);


            //Quickening Change 

            Scribe_Values.Look(ref this.learningMult, "learningMult", 1);
            Scribe_Values.Look(ref this.passionChance, "passionChance", 0.05f);
            Scribe_Values.Look(ref this.passionChances, "passionChances", 3);
            Scribe_Values.Look(ref this.passionLoss, "passionLoss", 3);
            Scribe_Values.Look(ref this.passionLimit, "passionLimit", -1);


            Scribe_Values.Look(ref this.noBrainCheat, "noBrainCheat", false);


            Scribe_Values.Look(ref this.hediffConditionMax, "hediffConditionMax", 1);
            Scribe_Values.Look(ref this.hediffDiseaseMax, "hediffDiseaseMax", 1);

            Scribe_Values.Look(ref this.diseaseMaxProg, "diseaseMaxProg", 1f);
            Scribe_Values.Look(ref this.conditionMaxProg, "conditionMaxProg", 1f);

            Scribe_Values.Look(ref this.canGetDisease, "canGetDisease", true);
            Scribe_Values.Look(ref this.canGetConditions, "canGetConditions", true);
            Scribe_Values.Look(ref this.canDieToDisease, "canDieToDisease", true);


            //Stunted Stuffs
            Scribe_Values.Look(ref this.burnChance, "stuntedBurnChance", 5);
            Scribe_Values.Look(ref this.burnMult, "stuntedBurnMult", 1);

            Scribe_Values.Look(ref this.stuntedChanceHead, "stuntedChanceHead", 25);
            Scribe_Values.Look(ref this.stuntedChanceDrug, "stuntedChanceDrug", 100);

            //Immortality Stealing
            Scribe_Values.Look(ref this.stage1StealChance, "stage1StealChance", 0.02f);
            Scribe_Values.Look(ref this.stage2StealChance, "stage2StealChance", 0.07f);
            Scribe_Values.Look(ref this.stage3StealChance, "stage3StealChance", 0.15f);
            Scribe_Values.Look(ref this.stage4StealChance, "stage4StealChance", 0.25f);
            Scribe_Values.Look(ref this.stage5StealChance, "stage5StealChance", 1);

            if (this.stage1StealChance > 1)
            {
                this.stage1StealChance = 0.02f;
            }

            if (this.stage2StealChance > 1)
            {
                this.stage2StealChance = 0.07f;
            }

            if (this.stage3StealChance > 1)
            {
                this.stage3StealChance = 0.15f;
            }

            if (this.stage4StealChance > 1)
            {
                this.stage4StealChance = 0.25f;
            }

            if (this.stage5StealChance > 1)
            {
                this.stage5StealChance = 1;
            }

            if (this.burnChance > 1)
            {
                this.burnChance = 0.05f;
            }

            Scribe_Values.Look(ref this.stewChanceMult, "stewChanceMult", 1);


            Scribe_Collections.Look(ref this.immortalClasses, "immortalClasses", LookMode.Deep);


            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {

            }
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (this.hediffSettingsSave == null)
                {
                    this.hediffSettingsSave = new Dictionary<string, Hediff_Setting>();
                }

                if (this.hediffSettings != null)
                {
                    foreach (KeyValuePair<HediffDef, Hediff_Setting> setting in this.hediffSettings)
                    {
                        if (!this.hediffSettingsSave.ContainsKey(setting.Key.defName))
                        {
                            this.hediffSettingsSave.Add(setting.Key.defName, setting.Value);
                        }
                        else
                        {
                            this.hediffSettingsSave[setting.Key.defName] = setting.Value;
                        }
                    }
                }
                //Scribe_Collections.Look(ref hediffCustomizations, "hediffCustomizations", LookMode.Value, LookMode.Deep);

            }
            Scribe_Collections.Look(ref this.hediffSettingsSave, "hediffCustomizationsSave", LookMode.Value, LookMode.Deep);


        }

        void DefaultGeneral()
        {
            this.immortalTickRateSingle = 50;
            this.immortalTickRate = 250;
            this.immortalTickRateRare = 25;
            this.revealImmortalityNumber = false;
            this.disableFlashstorm = false;
            this.firstDeathQuickening = false;
        }

        void DefaultGeneralImmortality()
        {
            this.baseHealSpeed = 1f;
            this.firstDeathHealFactor = 0.1f;



            this.immortalTransferSizeFactor = 0.5f;
            this.immortalHealingSizeFactor = 0.5f;

            this.deathEffectVal = 0;

        }

        void DefaultDefaultImmortality()
        {
            this.immortalChance = 0.05f;
            this.immortalChanceVisible = 0.10f;
            this.immortalTransferIn = 1f;
            this.immortalTransferOut = 0.2f;
            this.immortalSpawnMin = 0.6f;
            this.immortalSpawnMax = 5f;
            this.scarHealSpeed = 1f;
        }

        void DefaultRegrowth()
        {
            //Healing Stuffs
            this.immortalRegrowSpeed = 1f;
            this.immortalRegrowFoodCost = 0.5f;
            this.immortalAccumulateFoodNeed = 1f;

            this.immortalStage0MaxPartSize = 0f;
            this.immortalStage1MaxPartSize = 9f;
            this.immortalStage2MaxPartSize = 20f;
            this.immortalStage3MaxPartSize = 35f;
            this.immortalStage4MaxPartSize = 40f;
            this.immortalStage5MaxPartSize = 50f;
        }

        void DefaultQuickeningChange()
        {
            this.learningMult = 1f;
            this.swapTraitsChance = 0;
            this.passionChance = 0.05f;
            this.passionChances = 3;
            this.passionLoss = 0;
            this.passionLimit = -1;
        }

        void DefaultTransfer()
        {
            this.stuntedChanceHead = 0.25f;
            this.stuntedChanceDrug = 1;
            this.burnChance = 0.05f;
            this.burnMult = 1f;

            this.stage1StealChance = 0.02f;
            this.stage2StealChance = 0.07f;
            this.stage3StealChance = 0.15f;
            this.stage4StealChance = 0.25f;
            this.stage5StealChance = 1;

            this.stewChanceMult = 1;
        }

        void DefaultDefaultHediffs()
        {
            this.hediffConditionMax = 1f;
            this.hediffDiseaseMax = 1f;
        }

        public void DefaultImmortalClasses()
        {
            Immortal_Class immortalClass;

            List<ThingDef> allPawns = DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn).ToList();


            this.immortalClasses = new List<Immortal_Class>();
            immortalClass = new Immortal_Class("IH_humanoid".Translate());
            foreach (ThingDef pawn in DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn).ToList())
            {
                if (pawn.race.Humanlike)
                {
                    immortalClass.pawnTypes.Add(pawn);
                    allPawns.Remove(pawn);
                }

            }
            this.immortalClasses.Add(immortalClass); //Humanoid
            immortalClass = new Immortal_Class("IH_animal".Translate());
            foreach (ThingDef pawn in DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn))
            {
                if (pawn.race.Animal)
                {
                    immortalClass.pawnTypes.Add(pawn);
                    allPawns.Remove(pawn);
                }
            }
            immortalClass.immortalChance = 0.05f;
            immortalClass.immortalChanceVisible = 0.0f;
            this.immortalClasses.Add(immortalClass); //Animal

            immortalClass = new Immortal_Class("IH_disable".Translate());
            foreach (ThingDef pawn in DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn))
            {
                if (pawn.race.IsMechanoid)
                {
                    immortalClass.pawnTypes.Add(pawn);
                    allPawns.Remove(pawn);
                }
            }
            immortalClass.canSpawnImmortal = false;
            immortalClass.canBeImmortal = false;
            immortalClass.pawnTypes.Concat(allPawns);
            this.immortalClasses.Add(immortalClass); //Remainder

        }

        void DefaultHediffSettings()
        {
            this.hediffSettings = new Dictionary<HediffDef, Hediff_Setting>();
            this.hediffSettingsSave = new Dictionary<string, Hediff_Setting>();
            this.LoadImmortalsHediffs();
        }

        public void LoadBaseHediffs()
        {
            this.immortalHediffs = new List<HediffDef>();

            // this.immortalHediff = DefDatabase<HediffDef>.GetNamed("IH_Immortal");
            // this.deathStasisHediff = DefDatabase<HediffDef>.GetNamed("IH_DeathStasis");
            // this.stuntedHediff = DefDatabase<HediffDef>.GetNamed("IH_Stunted");
            // this.stuntedProcHediff = DefDatabase<HediffDef>.GetNamed("IH_StuntedProc");
            // this.stuntedBurnHediff = DefDatabase<HediffDef>.GetNamed("IH_Burn");
            // this.timerHediff = DefDatabase<HediffDef>.GetNamed("IH_DeathTimer");
            // this.growingHediff = DefDatabase<HediffDef>.GetNamed("IH_regrowing");
            // this.missingHediff = DefDatabase<HediffDef>.GetNamed("MissingBodyPart");
            // this.adjustingHediff = DefDatabase<HediffDef>.GetNamed("IH_adjusting");
            // this.returnedHediff = DefDatabase<HediffDef>.GetNamed("IH_revived");
            // this.firstReturnedHediff = DefDatabase<HediffDef>.GetNamed("IH_revivedFirst");
            // this.hungerHediff = DefDatabase<HediffDef>.GetNamed("IH_theHunger");
            // this.hungerHolderHediff = DefDatabase<HediffDef>.GetNamed("IH_hungerHolder");

            // this.nemnirHediff = DefDatabase<HediffDef>.GetNamed("IH_NemnirHigh");
            // this.komaHediff = DefDatabase<HediffDef>.GetNamed("IH_KomaHigh");
            // this.mortalisCrystalHediff = DefDatabase<HediffDef>.GetNamed("IH_MortalisCrystalImplant");
            // this.mortalisBuildupHediff = DefDatabase<HediffDef>.GetNamed("IH_MortalisTollerance");

            this.immortalHediffs.Add(HediffDefOf_Immortals.IH_MortalisTollerance); // this.mortalisBuildupHediff
            this.immortalHediffs.Add(HediffDefOf_Immortals.IH_Burn); // this.stuntedBurnHediff
            //immortalHediffs.Add(deathStasisHediff);

            this.baseHealHediffs = new List<HediffDef>();
            this.baseHealHediffs.Add(HediffDefOf_Immortals.IH_regrowing);
            this.baseHealHediffs.Add(HediffDefOf_Immortals.IH_DeathTimer);

            this.baseIgnoreHediffs = new List<HediffDef>();
            this.baseIgnoreHediffs.Add(HediffDefOf_Immortals.IH_Stunted);
            this.baseIgnoreHediffs.Add(HediffDefOf_Immortals.IH_Burn);
            this.baseIgnoreHediffs.Add(HediffDefOf_Immortals.IH_DeathStasis);

            this.hediffsLoaded = true;
        }

        public void DeleteImmortalClass(Immortal_Class immortalClass)
        {
            if (immortalClass != null)
            {
                if (immortalClass.pawnTypes != null)
                {
                    foreach (ThingDef pawn in immortalClass.pawnTypes)
                    {
                        this.defaultPawns.Add(pawn);
                    }
                }
                this.immortalClasses.Remove(immortalClass);
            }
        }

        public void DefaultSettings(bool general, bool generalImmortal, bool regrowth, bool defaultImmortal, bool immortalClasses, bool quickeningChange, bool transfer, bool hediffs)
        {
            if (general)
            {
                this.DefaultGeneral();
            }

            if (generalImmortal)
            {
                this.DefaultGeneralImmortality();
            }

            if (regrowth)
            {
                this.DefaultRegrowth();
            }

            if (defaultImmortal)
            {
                this.DefaultDefaultImmortality();
            }

            if (immortalClasses)
            {
                this.DefaultImmortalClasses();
            }

            if (quickeningChange)
            {
                this.DefaultQuickeningChange();
            }

            if (transfer)
            {
                this.DefaultTransfer();
            }

            if (hediffs)
            {
                this.DefaultHediffSettings();
            }

            //Othe stuff to Have settings for
            this.noBrainCheat = false;

            //Diseases
            this.canGetDisease = true;
            this.canGetConditions = true;
            this.canDieToDisease = true;
            this.diseaseMaxProg = 1f;
            this.conditionMaxProg = 1f;

        }

        public void DefaultHediffs()
        {

            this.oldAgeDefs = new List<HediffDef>();
        }

        public void DefaultAll(bool general, bool generalImmortal, bool regrowth, bool defaultImmortal, bool immortalClasses, bool quickeningChange, bool transfer, bool hediffs)
        {
            this.DefaultHediffs();
            this.DefaultSettings(general, generalImmortal, regrowth, defaultImmortal, immortalClasses, quickeningChange, transfer, hediffs);
        }

        public void LoadPawnCategories()
        {

            int missingPawnCount = 0;
            string missingPawns = "";
            if (this.immortalClasses != null)
            {

                if (this.defaultPawns == null)
                {
                    this.defaultPawns = new List<ThingDef>();
                }

                this.defaultPawns = DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn).ToList();

                foreach (Immortal_Class immortalClass in this.immortalClasses)
                {
                    immortalClass.pawnTypes = new List<ThingDef>();
                    ThingDef addDef;

                    if (immortalClass.pawnTypesLoad != null)
                    {

                        foreach (string defName in immortalClass.pawnTypesLoad)
                        {
                            addDef = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
                            if (addDef != null)
                            {
                                immortalClass.pawnTypes.Add(addDef);
                                this.defaultPawns.Remove(addDef);
                            }
                            else
                            {
                                missingPawnCount++;
                                missingPawns = missingPawns + defName + ", ";
                            }
                        }

                    }

                }

                if (missingPawnCount != 0)
                {
                    Log.Warning("IH_ImmortalClassLoadMissing".Translate(missingPawnCount, missingPawns));
                }

                this.pawnTypesLoaded = true;
            }

        }

        public void LoadHediffSettings()
        {
            int missingHediffsCount = 0;
            string missingHediffs = "";
            if (this.hediffSettings == null)
            {
                this.hediffSettings = new Dictionary<HediffDef, Hediff_Setting>();
            }

            if (this.hediffSettingsSave == null)
            {
                return;
            }

            foreach (KeyValuePair<string, Hediff_Setting> setting in this.hediffSettingsSave)
            {
                HediffDef hediff = DefDatabase<HediffDef>.GetNamedSilentFail(setting.Key);
                if (hediff != null)
                {
                    this.hediffSettings.Add(hediff, setting.Value);
                }
                else
                {
                    missingHediffsCount++;
                    missingHediffs = missingHediffs + setting.Key + ", ";
                }
            }

            this.LoadImmortalsHediffs();

            if (missingHediffsCount > 0)
            {
                Log.Warning("IH_hediffLoadMissing".Translate(missingHediffsCount, missingHediffs));
            }

            this.hediffSettingsLoaded = true;


        }

        void LoadImmortalsHediffs()
        {
            if (!this.hediffSettings.ContainsKey(HediffDefOf_Immortals.IH_MortalisTollerance))
            {
                this.hediffSettings.Add(HediffDefOf_Immortals.IH_MortalisTollerance, new Hediff_Setting(null, 0, false, true, false, HediffType.Other));
            }

            if (!this.hediffSettings.ContainsKey(HediffDefOf_Immortals.IH_Burn))
            {
                this.hediffSettings.Add(HediffDefOf_Immortals.IH_Burn, new Hediff_Setting(null, 0.01f, false, true, false, HediffType.Other));
            }
        }

        public void LoadAgeHediffs()
        {
            foreach (HediffGiverSetDef hediffGiverSet in Verse.DefDatabase<HediffGiverSetDef>.AllDefs)
            {
                foreach (HediffGiver hediffGiver in hediffGiverSet.hediffGivers)
                {
                    if (hediffGiver.ToString().ToLower().Contains("birthday"))
                    {
                        if (hediffGiver.hediff.isBad)
                        {
                            if (!this.oldAgeDefs.Contains(hediffGiver.hediff))
                            {
                                this.oldAgeDefs.Add(hediffGiver.hediff);
                            }
                        }
                    }
                }
            }
        }

        public Hediff_Setting HediffMakeSetting(HediffDef hediff)
        {
            Hediff_Setting hediffSetting = new();

            hediffSetting.hediffType = HediffType.Other;
            hediffSetting.needToCure = this.HediffBaseCureToReviveDef(hediff);
            hediffSetting.canGet = this.HediffBaseCanGet(hediff);
            hediffSetting.hediffType = HediffBaseGetType(hediff);
            switch (hediffSetting.hediffType)
            {
                case HediffType.Disease:
                    hediffSetting.maxProgress = this.hediffDiseaseMax; break;
                case HediffType.Condition:
                    hediffSetting.maxProgress = this.hediffConditionMax; break;
                case HediffType.Other:
                    hediffSetting.maxProgress = -1f; break;
            }


            hediffSetting.healSpeed = this.HediffBaseHealSpeedDef(hediff);
            hediffSetting.regrowHediff = false;
            return hediffSetting;
        }

        public bool HediffBaseHeal(Hediff hediff, bool dead)
        {
            if (this.hediffSettings.ContainsKey(hediff.def) && this.hediffSettings[hediff.def].healSpeed != null)
            {
                float val = this.hediffSettings[hediff.def].healSpeed.Value;
                if (val != 0)
                {
                    return true;
                }

                return false;
            }
            if (hediff.def.injuryProps != null && hediff.def != HediffDefOf.MissingBodyPart)
            {
                if (hediff.IsPermanent())
                {
                    return false;
                }

                return true;
            }
            if (hediff.def.lethalSeverity != -1 && dead)
            {
                return true;
            }

            return this.HediffBaseHealDef(hediff.def);
        }

        private bool HediffBaseHealDef(HediffDef hediff)
        {
            if (this.baseIgnoreHediffs.Contains(hediff))
            {
                return false;
            }

            if (this.baseHealHediffs.Contains(hediff))
            {
                return true;
            }

            return false;
        }

        public bool HediffBaseCanGet(HediffDef hediff)
        {
            if (this.hediffSettings.ContainsKey(hediff) && this.hediffSettings[hediff].canGet != null)
            {
                return this.hediffSettings[hediff].canGet.Value;
            }

            if (hediff.lethalSeverity == 1)
            {
                if (hediff.tendable)
                {
                    if (this.hediffDiseaseMax == 0)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    if (this.hediffConditionMax == 0)
                    {
                        return false;
                    }

                    return true;
                }
            }
            return true;
        }
        public bool HediffCureToRevive(Hediff hediff)
        {
            if (this.hediffSettings.ContainsKey(hediff.def) && this.hediffSettings[hediff.def].needToCure != null)
            {
                return this.hediffSettings[hediff.def].needToCure.Value;
            }

            if (hediff.def.tendable)
            {
                if (hediff.IsPermanent() || hediff.def == HediffDefOf.MissingBodyPart)
                {
                    return false;
                }

                return true;
            }
            if (hediff.def == HediffDefOf_Immortals.IH_regrowing)
            {
                if (hediff.Severity < 1)
                {
                    return true;
                }

                return false;
            }

            return this.HediffBaseCureToReviveDef(hediff.def);
        }

        private bool HediffBaseCureToReviveDef(HediffDef hediff)
        {
            if (hediff == HediffDefOf_Immortals.IH_DeathStasis || hediff == HediffDefOf_Immortals.IH_Stunted)
            {
                return false;
            }

            if (hediff == HediffDefOf_Immortals.IH_DeathTimer)
            {
                return true;
            }

            if (hediff.lethalSeverity != -1)
            {
                return true;
            }

            return false;
        }

        private static HediffType HediffBaseGetType(HediffDef hediff)
        {
            if (hediff.lethalSeverity == 1)
            {
                if (hediff.tendable)
                {
                    return HediffType.Disease;
                }
                else
                {
                    return HediffType.Condition;
                }
            }
            return HediffType.Other;
        }

        public float HediffHealSpeedDef(HediffDef hediff)
        {
            if (this.hediffSettings.ContainsKey(hediff) && this.hediffSettings[hediff].healSpeed != null)
            {
                return this.hediffSettings[hediff].healSpeed.Value;
            }

            return this.HediffBaseHealSpeedDef(hediff);
        }

        private float HediffBaseHealSpeedDef(HediffDef hediff)
        {
            if (hediff == HediffDefOf_Immortals.IH_regrowing)
            {
                return -1;
            }

            if (hediff.lethalSeverity != -1)
            {
                return this.slowHealSpeed;
            }

            if (hediff.injuryProps == null)
            {
                return this.slowHealSpeed;
            }

            return 1;
        }

        public float HediffMaxProgress(HediffDef hediff)
        {
            if (this.hediffSettings.ContainsKey(hediff) && this.hediffSettings[hediff].maxProgress != null)
            {
                return this.hediffSettings[hediff].maxProgress.Value;
            }

            if (hediff.tendable)
            {
                return 9999f;
            }

            return -1;
        }

        public bool IsImmortalsHediff(HediffDef hediff)
        {
            if (this.immortalHediffs.Contains(hediff))
            {
                return true;
            }

            return false;
        }

        public bool SlowToRecover(Hediff hediff)
        {
            if (hediff.def.lethalSeverity != -1)
            {
                return true;
            }

            if (hediff.def.injuryProps == null)
            {
                return true;
            }

            if (hediff.def == HediffDefOf_Immortals.IH_regrowing)
            {
                return true;
            }

            return false;
        }

        public float GetMaxPartSize(float severity)
        {
            float retSize = 0;

            if (severity > 0.5)
            {
                if (severity >= 10)
                {
                    retSize += this.immortalStage5MaxPartSize;
                }
                else if (severity >= 7)
                {
                    retSize += this.immortalStage4MaxPartSize;
                }
                else if (severity >= 4)
                {
                    retSize += this.immortalStage3MaxPartSize;
                }
                else if (severity >= 2)
                {
                    retSize += this.immortalStage2MaxPartSize;
                }
                else if (severity >= 1)
                {
                    retSize += this.immortalStage1MaxPartSize;
                }
                else
                {
                    retSize += this.immortalStage0MaxPartSize;
                }
            }
            retSize = (float)Math.Floor(retSize);
            return retSize;
        }

    }

}
