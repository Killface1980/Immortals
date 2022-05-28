using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using RimWorld.Planet;

namespace Immortals
{
    public class Immortals_Mod : Mod
    {
        Immortals_Settings settings;

        public Immortals_Mod(ModContentPack content) : base(content)
        {
            this.settings = this.GetSettings<Immortals_Settings>();

            //pawnsDefs = DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn);

        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (!this.settings.pawnTypesLoaded)
            {
                this.settings.LoadPawnCategories();
            }

            if (this.settings.immortalClasses == null)
            {
                this.settings.DefaultImmortalClasses();
            }

            if (!this.settings.hediffSettingsLoaded)
            {
                this.settings.LoadBaseHediffs();
            }

            if (this.settings.hediffSettings == null)
            {
                this.settings.LoadHediffSettings();
            }

            if (!this.settings.hediffsLoaded)
            {
                this.settings.LoadBaseHediffs();
            }

            int boxPadding = 5;
            int rectWidth = (int)inRect.width - 10;

            Rect scrollRect = new(0, 0, inRect.width - 100, this.totalHeight);
            Rect rect1 = new(boxPadding, boxPadding, inRect.width / 3 - 4 * boxPadding, this.ySpace);
            Rect rect2 = new(rectWidth / 3 + boxPadding, boxPadding, rectWidth / 3 - 4 * boxPadding, this.ySpace);
            Rect rect3 = new((rectWidth / 3 * 2) + boxPadding, boxPadding, rectWidth / 3 - 4 * boxPadding, this.ySpace);

            Widgets.BeginScrollView(inRect, ref this.scrollPos, scrollRect);

            Widgets.DrawBoxSolidWithOutline(new Rect(0, 0, rect1.width + (2 * boxPadding), this.section1Height + boxPadding * 2), this.editingGray, Color.grey);
            Widgets.DrawBoxSolidWithOutline(new Rect(rect3.x - boxPadding, 0, rect3.width + (2 * boxPadding), this.section1Height + boxPadding * 2), this.editingGray, Color.grey);




            //General Settings
            Widgets.LabelFit(rect1, "IH_GeneralSettings".Translate());
            rect1.y += this.ySpace;

            this.FieldSlider(ref rect1, ref this.settings.immortalTickRateSingle, 1, 1000, "IH_tickRateSingle", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalTickRate, 1, 1000, "IH_tickRate", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalTickRateRare, 1, 100, "IH_tickRateRare", 0);

            this.LabelCheckbox(ref rect1, ref this.settings.disableFlashstorm, "IH_disableFlashstorm");
            this.LabelCheckbox(ref rect1, ref this.settings.firstDeathQuickening, "IH_firstDeathGrantsQuickening");
            this.LabelCheckbox(ref rect1, ref this.settings.revealImmortalityNumber, "IH_ShowImmortalNumbers");

            Widgets.DrawBox(new Rect(0, 0, rect1.width + (2 * boxPadding), this.section1Height + (2 * boxPadding)));

            //General Immortality Settings
            Widgets.LabelFit(rect2, "IH_GeneralImmortalitySettings".Translate());
            rect2.y += this.ySpace;

            this.FieldSlider(ref rect2, ref this.settings.deathEffectVal, -1, 1, "IH_DeathEffectOnVal", 2);
            this.FieldSlider(ref rect2, ref this.settings.baseHealSpeed, 0, 10, "IH_baseHealSpeed", 2);
            this.FieldSlider(ref rect2, ref this.settings.scarHealSpeed, 0, 10, "IH_scarHealSpeed", 1);
            this.FieldSlider(ref rect2, ref this.settings.immortalTransferSizeFactor, 0.1f, 4, "IH_immortalTransferBodyFactor", 1);
            this.FieldSlider(ref rect2, ref this.settings.immortalHealingSizeFactor, 0.1f, 4, "IH_immortalHealingBodyFactor", 1);


            //Default and Utility
            if (!this.confirmDefaultAll)
            {
                TooltipHandler.TipRegion(new Rect(rect3.x, rect3.y, rect3.width, this.ySpace), "IH_defaultAllTip".Translate());
                if (Widgets.ButtonText(rect3, "IH_defaultAll".Translate()))
                {
                    this.confirmDefaultAll = true;
                }
                rect3.y += this.ySpace;
            }
            else
            {
                TooltipHandler.TipRegion(new Rect(rect3.x, rect3.y, rect3.width, this.ySpace), "IH_defaultAllTip".Translate());
                if (Widgets.ButtonText(rect3, "IH_defaultConfirm".Translate()))
                {
                    this.settings.DefaultAll(this.defaultGeneral, this.defaultGeneralImmortal, this.defaultRegrowth, this.defaultDefaultImmortal, this.defaultImmortalClasses, this.defaultQuickeningChange, this.defaultTransfer, this.defaultHediffs);
                    this.confirmDefaultAll = false;
                    this.defaultGeneral = false;
                    this.defaultGeneralImmortal = false;
                    this.defaultRegrowth = false;
                    this.defaultDefaultImmortal = false;
                    this.defaultImmortalClasses = false;
                    this.defaultQuickeningChange = false;
                    this.defaultTransfer = false;
                    this.defaultHediffs = false;
                    this.pawnClassSelection = null;
                }
                rect3.y += this.ySpace2;

                this.LabelCheckbox(ref rect3, ref this.defaultGeneral, "IH_defaultGeneral");
                this.LabelCheckbox(ref rect3, ref this.defaultGeneralImmortal, "IH_defaultGeneralImmortal");
                this.LabelCheckbox(ref rect3, ref this.defaultRegrowth, "IH_defaultRegrowth");
                this.LabelCheckbox(ref rect3, ref this.defaultDefaultImmortal, "IH_defaultDefaultImmortal");
                this.LabelCheckbox(ref rect3, ref this.defaultImmortalClasses, "IH_defaultImmortalClasses");
                this.LabelCheckbox(ref rect3, ref this.defaultQuickeningChange, "IH_quickeningChange");
                this.LabelCheckbox(ref rect3, ref this.defaultTransfer, "IH_defaultTransfer");
                this.LabelCheckbox(ref rect3, ref this.defaultHediffs, "IH_defaultHediffs");
            }

            if (Current.Game != null)
            {
                if (!this.confirmStripImmortals)
                {
                    if (Widgets.ButtonText(rect3, "IH_removeImmortality".Translate()))
                    {
                        this.confirmStripImmortals = true;
                    }
                }
                else
                {
                    if (Widgets.ButtonText(rect3, "IH_defaultConfirm".Translate()))
                    {
                        Immortal_Component immortalComponent = Current.Game.GetComponent<Immortal_Component>();
                        immortalComponent.RemoveImmortals();
                        this.confirmStripImmortals = false;
                    }
                }
                rect3.y += this.ySpace;

                if (!this.confirmRerollImmortals)
                {
                    if (Widgets.ButtonText(rect3, "IH_addImmortality".Translate()))
                    {
                        this.confirmRerollImmortals = true;
                    }
                }
                else
                {
                    if (Widgets.ButtonText(rect3, "IH_defaultConfirm".Translate()))
                    {
                        Immortal_Component immortalComponent = Current.Game.GetComponent<Immortal_Component>();
                        Immortal_Component.RollImmortals();
                        this.confirmRerollImmortals = false;
                    }
                }
                rect3.y += this.ySpace;
            }
            else
            {
                Widgets.ButtonText(rect3, "IH_removeImmortality".Translate(), true, false, false);
                rect3.y += this.ySpace2;
                Widgets.ButtonText(rect3, "IH_addImmortality".Translate(), true, false, false);
                rect3.y += this.ySpace2;
            }


            float newY = Math.Max(rect1.y, Math.Max(rect2.y, rect3.y));
            this.section1Height = (int)newY;
            newY += this.ySpace;

            rect1.y = newY;
            rect2.y = newY;
            rect3.y = newY;


            Widgets.DrawBoxSolidWithOutline(new Rect(rect2.x - boxPadding, newY - boxPadding, rect2.width + (2 * boxPadding), this.section2Height + boxPadding * 2), this.editingGray, Color.grey);



            rect1.height = this.ySpace;

            Widgets.LabelFit(new Rect(rect1.x, rect1.y, rect1.width, this.ySpace2), "IH_regrowth".Translate());
            rect1.y += this.ySpace2;

            this.FieldSlider(ref rect1, ref this.settings.immortalRegrowSpeed, 0, 10, "IH_regrowSpeed", 2);
            this.FieldSlider(ref rect1, ref this.settings.immortalRegrowMaxParts, -1, 10, "IH_regrowMaxParts", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalRegrowFoodCost, 0, 10, "IH_regrowCostsFood", 2);
            this.FieldSlider(ref rect1, ref this.settings.immortalAccumulateFoodNeed, 0, 2.5f, "IH_accumulateFoodNeed", 1);
            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, this.ySpace), "IH_perStageHpTip".Translate());
            Widgets.LabelFit(new Rect(rect1.x + rect1.width * 0.25f, rect1.y, rect1.width * 0.75f, this.ySpace), "IH_perStageHp".Translate());
            rect1.y += this.ySpace;

            this.FieldSlider(ref rect1, ref this.settings.immortalStage0MaxPartSize, 0, 50, "IH_regrowLesser", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalStage1MaxPartSize, 0, 50, "IH_regrowImmortal", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalStage2MaxPartSize, 0, 50, "IH_regrowGreater", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalStage3MaxPartSize, 0, 50, "IH_regrowHigh", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalStage4MaxPartSize, 0, 50, "IH_regrowGrand", 0);
            this.FieldSlider(ref rect1, ref this.settings.immortalStage5MaxPartSize, 0, 50, "IH_regrowApex", 0);



            this.settings.immortalStage1MaxPartSize = Math.Max(this.settings.immortalStage1MaxPartSize, this.settings.immortalStage0MaxPartSize);
            this.settings.immortalStage2MaxPartSize = Math.Max(this.settings.immortalStage2MaxPartSize, this.settings.immortalStage1MaxPartSize);
            this.settings.immortalStage3MaxPartSize = Math.Max(this.settings.immortalStage3MaxPartSize, this.settings.immortalStage2MaxPartSize);
            this.settings.immortalStage4MaxPartSize = Math.Max(this.settings.immortalStage4MaxPartSize, this.settings.immortalStage3MaxPartSize);
            this.settings.immortalStage5MaxPartSize = Math.Max(this.settings.immortalStage5MaxPartSize, this.settings.immortalStage4MaxPartSize);

            rect1.height = this.ySpace2;

            //Default Immortal Settings

            Widgets.LabelFit(rect2, "IH_defaultImmortality".Translate());
            rect2.y += this.ySpace;

            this.FieldSlider(ref rect2, ref this.settings.immortalChance, 0, 1, "IH_chance", 2);
            this.FieldSlider(ref rect2, ref this.settings.immortalChanceVisible, 0, 1, "IH_chanceVisible", 2);


            TooltipHandler.TipRegion(new Rect(rect2.x, rect2.y, rect2.width, this.ySpace), "IH_ImmortalClassChanceTip".Translate());
            Widgets.LabelFit(rect2, "IH_ImmortalClassRange".Translate() + this.settings.immortalSpawnMin + " - " + this.settings.immortalSpawnMax);
            rect2.y += this.ySpace;
            this.settings.immortalSpawnMin = Widgets.HorizontalSlider(new Rect(rect2.x, rect2.y, rect2.width, this.ySpace), this.settings.immortalSpawnMin, 0.6f, 10);
            this.settings.immortalSpawnMin = (float)Math.Round(this.settings.immortalSpawnMin, 1, MidpointRounding.AwayFromZero);
            rect2.y += this.ySpace;
            this.settings.immortalSpawnMax = Widgets.HorizontalSlider(new Rect(rect2.x, rect2.y, rect2.width, this.ySpace), this.settings.immortalSpawnMax, 0.6f, 10);
            this.settings.immortalSpawnMax = (float)Math.Round(this.settings.immortalSpawnMax, 1, MidpointRounding.AwayFromZero);
            rect2.y += this.ySpace;
            //FieldSlider(ref rect2, ref settings.immortalSpawnMin, 0.6f, 10, "IH_ImmortalClassRange", 1);



            if (this.settings.immortalSpawnMin > this.settings.immortalSpawnMax)
            {
                float temp = this.settings.immortalSpawnMin;
                this.settings.immortalSpawnMin = this.settings.immortalSpawnMax;
                this.settings.immortalSpawnMax = temp;
            }
            this.FieldSlider(ref rect2, ref this.settings.immortalTransferIn, 0, 2, "IH_ImmortalClassTransferIn", 2);
            this.FieldSlider(ref rect2, ref this.settings.immortalTransferOut, 0, 2, "IH_ImmortalClassTransferOut", 2);

            //Widgets.CheckboxLabeled(new Rect(rect2.x, rect2.y, rect2.width, ySpace), "IH_healScars".Translate(), ref settings.healScars);
            //rect2.y += ySpace;

            Widgets.LabelFit(rect3, "IH_defaultHediffs".Translate());
            rect3.y += this.ySpace;

            this.FieldSlider(ref rect3, ref this.settings.hediffConditionMax, 0, 1, "IH_conditionMax", 2);
            this.FieldSlider(ref rect3, ref this.settings.hediffDiseaseMax, 0, 1, "IH_diseaseMax", 2);



            newY = Math.Max(rect1.y, Math.Max(rect2.y, rect3.y));
            this.section2Height = (int)newY - this.section1Height;
            newY += this.ySpace2;

            rect1.y = newY;
            rect2.y = newY;
            rect3.y = newY;

            Widgets.DrawBoxSolidWithOutline(new Rect(0, newY - boxPadding, inRect.width - boxPadding * 2 - 10, this.section3Height + boxPadding * 2), this.editingGray, Color.grey);
            Widgets.DrawBoxSolidWithOutline(new Rect(0, newY - boxPadding, inRect.width - boxPadding * 2 - 10, this.ySpace2 + boxPadding * 2), this.editingGray, Color.grey);
            Widgets.DrawBoxSolidWithOutline(new Rect(0, newY - boxPadding, rect1.width + boxPadding * 2, this.ySpace2 + boxPadding * 2), this.editingGray, Color.grey);
            Widgets.DrawBoxSolidWithOutline(new Rect(rect3.x - boxPadding, newY - boxPadding, rect3.width + boxPadding * 2, this.ySpace2 + boxPadding * 2), this.editingGray, Color.grey);

            string label = "IH_categoriesOn".Translate();
            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, this.ySpace), "IH_categoriesTip".Translate());
            if (this.pawnCategorySelection)
            {
                label = "IH_categoriesOff".Translate();
            }

            if (Widgets.ButtonText(rect1, label))
            {
                this.pawnCategorySelection = !this.pawnCategorySelection;
                this.hediffSelection = false;
            }

            label = "IH_hediffsOn".Translate();
            if (this.hediffSelection)
            {
                label = "IH_hediffsOff".Translate();
            }

            if (Widgets.ButtonText(new Rect(rect3.x, rect3.y, rect3.width, this.ySpace2), label))
            {
                this.pawnCategorySelection = false;
                this.hediffSelection = !this.hediffSelection;
            }

            if (this.pawnCategorySelection)
            {
                Widgets.DrawBoxSolid(new Rect(1, (newY + 4) + this.ySpace2, rect1.width + boxPadding * 2 - 2, 3), this.editingGray);
                rect1.y += this.ySpace;
                rect2.y += this.ySpace;
                rect3.y += this.ySpace;
                this.EditPawns(inRect, ref rect1, ref rect2, ref rect3);
            }

            if (this.hediffSelection)
            {
                Widgets.DrawBoxSolid(new Rect(rect3.x - 3, (newY + 4) + this.ySpace2, rect3.width + boxPadding * 2 - 2, 3), this.editingGray);
                rect1.y += this.ySpace;
                rect2.y += this.ySpace;
                rect3.y += this.ySpace;
                this.EditHediffs(inRect, ref rect1, ref rect2, ref rect3);
            }



            newY = Math.Max(rect1.y, Math.Max(rect2.y, rect3.y));
            this.section3Height = (int)newY - this.section2Height - this.section1Height;
            newY += this.ySpace2 + boxPadding + 10;
            rect1.y = newY;
            rect2.y = newY;
            rect3.y = newY;


            Widgets.LabelFit(rect1, "IH_transferSettings".Translate());
            rect1.y += this.ySpace;

            this.FieldSlider(ref rect1, ref this.settings.stage1StealChance, 0, 1, "IH_stealStage1", 2);
            this.FieldSlider(ref rect1, ref this.settings.stage2StealChance, 0, 1, "IH_stealStage2", 2);
            this.FieldSlider(ref rect1, ref this.settings.stage3StealChance, 0, 1, "IH_stealStage3", 2);
            this.FieldSlider(ref rect1, ref this.settings.stage4StealChance, 0, 1, "IH_stealStage4", 2);
            this.FieldSlider(ref rect1, ref this.settings.stage5StealChance, 0, 1, "IH_stealStage5", 2);

            this.FieldSlider(ref rect1, ref this.settings.stuntedChanceHead, 0, 1, "IH_stuntedChance", 2);
            this.FieldSlider(ref rect1, ref this.settings.burnChance, 0, 1, "IH_stuntedBurnChance", 2);
            this.FieldSlider(ref rect1, ref this.settings.burnMult, 0, 10, "IH_stuntedBurnRate", 2);

            Widgets.LabelFit(rect3, "IH_quickeningChange".Translate());
            rect3.y += this.ySpace;

            this.FieldSlider(ref rect3, ref this.settings.learningMult, 0, 10, "IH_learningMult", 2);
            this.FieldSlider(ref rect3, ref this.settings.passionChance, 0, 1, "IH_passionChance", 2);
            this.FieldSlider(ref rect3, ref this.settings.passionChances, 1, 5, "IH_passionChances", 0);
            //FieldSlider(ref rect3, ref settings.passionLoss, 0, 1, "IH_passionLoss", 2);
            this.FieldSlider(ref rect3, ref this.settings.passionLimit, -1, 20, "IH_passionLimit", 0);


            this.totalHeight = (int)Math.Max(rect1.y, Math.Max(rect2.y, rect3.y)) + this.ySpace2;


            Widgets.EndScrollView();


            if (this.lastDisease)
            {
                this.update = true;
            }

            if (this.update)
            {
                Immortal_Component immortalComponent = Current.Game.GetComponent<Immortal_Component>();
                //immortalComponent.ReLoadAilments();
            }
        }

        private void EditPawns(Rect drawRect, ref Rect rect1, ref Rect rect2, ref Rect rect3)
        {
            int ySpace = 24;
            int ySpace2 = 36;
            int padding = 5;
            rect1.height = ySpace;
            rect2.y += ySpace;
            rect3.y += ySpace;

            rect1.y += ySpace + padding;

            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace), "IH_NewClassTip".Translate());
            if (Widgets.ButtonText(new Rect(rect1.x, rect1.y, rect1.width / 2, ySpace), "IH_NewClass".Translate()))
            {
                this.pawnClassSelection = new Immortal_Class("new Class");
                this.settings.immortalClasses.Add(this.pawnClassSelection);
            }
            foreach (Immortal_Class immortalClass in this.settings.immortalClasses)
            {
                rect1.y += ySpace;
                string name = immortalClass.name;
                if (this.pawnClassSelection == immortalClass)
                {
                    immortalClass.name = Widgets.TextField(rect1, immortalClass.name);
                }
                else
                {
                    if (Widgets.ButtonText(rect1, name))
                    {
                        this.pawnClassSelection = immortalClass;
                    }
                }
            }

            if (this.pawnClassSelection != null)
            {
                Rect settingsRect = rect1;
                settingsRect.x += padding;
                settingsRect.y += padding;
                settingsRect.width -= padding * 2;

                settingsRect.y += ySpace2;
                TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace), "IH_ImmortalClassCanBeTip".Translate());
                Widgets.CheckboxLabeled(settingsRect, "IH_ImmortalClassCanBe".Translate(), ref this.pawnClassSelection.canBeImmortal);
                settingsRect.y += ySpace;

                if (this.pawnClassSelection.canBeImmortal)
                {
                    //Immortal Chance

                    if (this.pawnClassSelection.immortalChance != null)
                    {
                        Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingYellow);
                        TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace), "IH_ImmortalClassChanceTip".Translate());
                        Widgets.LabelFit(settingsRect, "IH_ImmortalClassChance".Translate() + " " + this.pawnClassSelection.immortalChance.Value.ToString("0%"));
                        settingsRect.y += ySpace;
                        this.pawnClassSelection.immortalChance = Widgets.HorizontalSlider(settingsRect, this.pawnClassSelection.immortalChance.Value, 0, 1.25f);
                        if (this.pawnClassSelection.immortalChance < 1.125f)
                        {
                            if (this.pawnClassSelection.immortalChance > 1f)
                            {
                                this.pawnClassSelection.immortalChance = 1f;
                            }
                        }
                        else
                        {
                            this.pawnClassSelection.immortalChance = null;
                        }

                        settingsRect.y += ySpace;
                    }
                    else
                    {
                        Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingGreen);
                        TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace), "IH_ImmortalClassChanceTip".Translate());
                        Widgets.LabelFit(settingsRect, "IH_ImmortalClassChance".Translate() + "IH_editDefault".Translate(this.settings.immortalChance.ToString("0%")));
                        settingsRect.y += ySpace;
                        float temp = 1.25f;

                        temp = Widgets.HorizontalSlider(settingsRect, temp, 0, 1.25f);
                        if (temp < 1.125f)
                        {
                            this.pawnClassSelection.immortalChance = 1f;
                        }
                        else
                        {
                            this.pawnClassSelection.immortalChance = null;
                        }

                        settingsRect.y += ySpace;
                    }
                    if ((this.pawnClassSelection.immortalChance != null && this.pawnClassSelection.immortalChance > 0) || (this.pawnClassSelection.immortalChance == null && this.settings.immortalChance > 0))
                    {
                        if (this.pawnClassSelection.immortalChanceVisible != null)
                        {
                            Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingYellow);
                            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace), "IH_ImmortalClassChanceVisibleTip".Translate());
                            Widgets.LabelFit(settingsRect, "IH_ImmortalClassChanceVisible".Translate() + " " + this.pawnClassSelection.immortalChanceVisible.Value.ToString("0%"));
                            settingsRect.y += ySpace;
                            this.pawnClassSelection.immortalChanceVisible = Widgets.HorizontalSlider(settingsRect, this.pawnClassSelection.immortalChanceVisible.Value, 0, 1.25f);
                            if (this.pawnClassSelection.immortalChanceVisible < 1.125f)
                            {
                                if (this.pawnClassSelection.immortalChanceVisible > 1f)
                                {
                                    this.pawnClassSelection.immortalChanceVisible = 1f;
                                }
                            }
                            else
                            {
                                this.pawnClassSelection.immortalChanceVisible = null;
                            }

                            settingsRect.y += ySpace;
                        }
                        else
                        {
                            Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingGreen);
                            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace), "IH_ImmortalClassChanceVisibleTip".Translate());
                            Widgets.LabelFit(settingsRect, "IH_ImmortalClassChanceVisible".Translate() + "IH_editDefault".Translate(this.settings.immortalChanceVisible.ToString("0%")));
                            settingsRect.y += ySpace;
                            float temp = 1.25f;

                            temp = Widgets.HorizontalSlider(settingsRect, temp, 0, 1.25f);
                            if (temp < 1.125f)
                            {
                                this.pawnClassSelection.immortalChanceVisible = 1f;
                            }
                            else
                            {
                                this.pawnClassSelection.immortalChanceVisible = null;
                            }

                            settingsRect.y += ySpace;
                        }


                        //Spawn Levels
                        if (this.pawnClassSelection.immortalSpawnMin != null && this.pawnClassSelection.immortalSpawnMax != null)
                        {
                            if (this.pawnClassSelection.immortalSpawnMin > this.pawnClassSelection.immortalSpawnMax)
                            {
                                float temp = this.pawnClassSelection.immortalSpawnMax.Value;
                                this.pawnClassSelection.immortalSpawnMax = this.pawnClassSelection.immortalSpawnMin;
                                this.pawnClassSelection.immortalSpawnMin = temp;
                            }
                            Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2 + ySpace), this.editingYellow);
                            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace * 3), "IH_ImmortalClassRangeTip".Translate());
                            Widgets.LabelFit(settingsRect, "IH_ImmortalClassRange".Translate() + " " + this.pawnClassSelection.immortalSpawnMin.Value.ToString("0.0") + " - " + this.pawnClassSelection.immortalSpawnMax.Value.ToString("0.0"));
                            settingsRect.y += ySpace;
                            this.pawnClassSelection.immortalSpawnMin = Widgets.HorizontalSlider(settingsRect, this.pawnClassSelection.immortalSpawnMin.Value, 0.6f, 12f);

                            settingsRect.y += ySpace;
                            this.pawnClassSelection.immortalSpawnMax = Widgets.HorizontalSlider(settingsRect, this.pawnClassSelection.immortalSpawnMax.Value, 0.6f, 12f);
                            if (this.pawnClassSelection.immortalSpawnMin < 11f && this.pawnClassSelection.immortalSpawnMax < 11f)
                            {
                                this.pawnClassSelection.immortalSpawnMin = Mathf.Clamp(this.pawnClassSelection.immortalSpawnMin.Value, 0.6f, 10f);
                                this.pawnClassSelection.immortalSpawnMax = Mathf.Clamp(this.pawnClassSelection.immortalSpawnMax.Value, 0.6f, 10f);
                            }
                            else
                            {
                                this.pawnClassSelection.immortalSpawnMin = null;
                                this.pawnClassSelection.immortalSpawnMax = null;
                            }

                            settingsRect.y += ySpace;


                        }
                        else
                        {
                            Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2 + ySpace), this.editingGreen);
                            TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace * 3), "IH_ImmortalClassRangeTip".Translate());
                            Widgets.LabelFit(settingsRect, "IH_ImmortalClassRange".Translate() + "IH_editDefault".Translate(this.settings.immortalSpawnMin.ToString("0.0") + " - " + this.settings.immortalSpawnMax.ToString("0.0")));
                            settingsRect.y += ySpace;

                            float temp1 = 12f;
                            temp1 = Widgets.HorizontalSlider(settingsRect, temp1, 0.6f, 12f);
                            settingsRect.y += ySpace;

                            float temp2 = 12f;
                            temp2 = Widgets.HorizontalSlider(settingsRect, temp2, 0.6f, 12f);
                            settingsRect.y += ySpace;

                            if (temp2 < 11f || temp1 < 11f)
                            {
                                this.pawnClassSelection.immortalSpawnMin = 10f;
                                this.pawnClassSelection.immortalSpawnMax = 10f;
                            }
                            else
                            {
                                this.pawnClassSelection.immortalSpawnMin = null;
                                this.pawnClassSelection.immortalSpawnMax = null;
                            }


                        }
                    }
                    //Transefer In
                    if (this.pawnClassSelection.immortalTransferIn != null)
                    {
                        Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingYellow);
                        TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace + ySpace), "IH_ImmortalClassTransferInTip".Translate());
                        Widgets.LabelFit(settingsRect, "IH_ImmortalClassTransferIn".Translate() + " " + this.pawnClassSelection.immortalTransferIn.Value.ToString("0%"));
                        settingsRect.y += ySpace;
                        this.pawnClassSelection.immortalTransferIn = Widgets.HorizontalSlider(settingsRect, this.pawnClassSelection.immortalTransferIn.Value, 0, 2.5f);
                        if (this.pawnClassSelection.immortalTransferIn < 2.25f)
                        {
                            if (this.pawnClassSelection.immortalTransferIn > 2f)
                            {
                                this.pawnClassSelection.immortalTransferIn = 2f;
                            }
                        }
                        else
                        {
                            this.pawnClassSelection.immortalTransferIn = null;
                        }

                        settingsRect.y += ySpace;
                    }
                    else
                    {
                        Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingGreen);
                        TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace + ySpace), "IH_ImmortalClassTransferInTip".Translate());
                        Widgets.LabelFit(settingsRect, "IH_ImmortalClassTransferIn".Translate() + "IH_editDefault".Translate(this.settings.immortalTransferIn.ToString("0%")));
                        settingsRect.y += ySpace;
                        float temp = 2.5f;

                        temp = Widgets.HorizontalSlider(settingsRect, temp, 0, 2.5f);
                        if (temp < 2.25f)
                        {
                            this.pawnClassSelection.immortalTransferIn = 2f;
                        }
                        else
                        {
                            this.pawnClassSelection.immortalTransferIn = null;
                        }

                        settingsRect.y += ySpace;
                    }

                    //Transfer out
                    if (this.pawnClassSelection.immortalTransferOut != null)
                    {
                        Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingYellow);
                        TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace + ySpace), "IH_ImmortalClassTransferOutTip".Translate());
                        Widgets.LabelFit(settingsRect, "IH_ImmortalClassTransferOut".Translate() + " " + this.pawnClassSelection.immortalTransferOut.Value.ToString("0%"));
                        settingsRect.y += ySpace;
                        this.pawnClassSelection.immortalTransferOut = Widgets.HorizontalSlider(settingsRect, this.pawnClassSelection.immortalTransferOut.Value, 0, 2.5f);
                        if (this.pawnClassSelection.immortalTransferOut < 2.25f)
                        {
                            if (this.pawnClassSelection.immortalTransferOut > 2f)
                            {
                                this.pawnClassSelection.immortalTransferOut = 2f;
                            }
                        }
                        else
                        {
                            this.pawnClassSelection.immortalTransferOut = null;
                        }

                        settingsRect.y += ySpace;
                    }
                    else
                    {
                        Widgets.DrawBoxSolid(new Rect(settingsRect.x, settingsRect.y, settingsRect.width, settingsRect.height * 2), this.editingGreen);
                        TooltipHandler.TipRegion(new Rect(rect1.x, rect1.y, rect1.width, ySpace + ySpace), "IH_ImmortalClassTransferOutTip".Translate());
                        Widgets.LabelFit(settingsRect, "IH_ImmortalClassTransferOut".Translate() + "IH_editDefault".Translate(this.settings.immortalTransferOut.ToString("0%")));
                        settingsRect.y += ySpace;
                        Widgets.DrawBoxSolid(settingsRect, this.editingGreen);
                        float temp = 2.5f;

                        temp = Widgets.HorizontalSlider(settingsRect, temp, 0, 2.5f);
                        if (temp < 2.25f)
                        {
                            this.pawnClassSelection.immortalTransferOut = 2f;
                        }
                        else
                        {
                            this.pawnClassSelection.immortalTransferOut = null;
                        }

                        settingsRect.y += ySpace;
                    }

                    if (this.deleteConfirm != 100)
                    {
                        if (Widgets.ButtonText(settingsRect, "IH_DeleteClass".Translate()))
                        {
                            this.deleteClass = !this.deleteClass;
                        }
                    }
                    else if (Widgets.ButtonText(settingsRect, "IH_defaultConfirm".Translate()))
                    {
                        this.shouldDelete = true;
                    }

                    settingsRect.y += ySpace2;

                }


                if (this.deleteClass)
                {
                    Widgets.LabelFit(settingsRect, "IH_slideConfirm".Translate());
                    settingsRect.y += ySpace2;
                    this.deleteConfirm = (int)Widgets.HorizontalSlider(settingsRect, this.deleteConfirm, 0, 100);
                }


                //DO ALL THE PAWN SORTING NOW BITCH
                //

                this.classSearch = Widgets.TextField(rect2, this.classSearch);

                List<ThingDef> checkList;
                checkList = DefDatabase<ThingDef>.AllDefs.Where(thing => thing.category == ThingCategory.Pawn).ToList();
                if (this.classSearch != "search")
                {
                    checkList = checkList.Where(thing => thing.label.Contains(this.classSearch)).ToList();
                }

                rect2.y += ySpace2;
                rect3.y += ySpace2;

                TooltipHandler.TipRegion(new Rect(rect2.x, rect2.y, rect2.width, 10 * ySpace2), "IH_ImmortalClassPawnsTip".Translate());
                Widgets.LabelFit(rect2, "IH_classPawns".Translate());
                rect2.y += ySpace;

                rect2.height = ySpace2;
                rect3.height = ySpace2;
                Rect rect2Scroll = rect2;
                Rect rect2Icon = rect2;
                Rect rect2Name = rect2;
                rect2Icon.width = rect2.width * 0.2f;

                rect2Scroll.width -= 16;
                rect2Scroll.height = this.pawnClassSelection.pawnTypes.Count * ySpace2;

                rect2Name.x += rect2Icon.width;
                rect2Name.width -= rect2Icon.width;

                Rect editScrollRect1 = new(rect2.x, rect2.y, rect2.width, 10 * ySpace2);
                this.editScroll1Height = (int)rect2.y;
                Widgets.BeginScrollView(editScrollRect1, ref this.editScroll1, rect2Scroll);

                ThingDef thingToRemove = null;
                foreach (ThingDef pawnType in this.pawnClassSelection.pawnTypes)
                {
                    if (this.classSearch == "search" || pawnType.label.Contains(this.classSearch))
                    {
                        if (Widgets.ButtonText(rect2, pawnType.label))
                        {
                            thingToRemove = pawnType;
                            this.settings.defaultPawns.Add(pawnType);
                        }

                        Widgets.DrawBoxSolidWithOutline(rect2, this.editingGray, Color.white);
                        Widgets.DefIcon(rect2Icon, pawnType);
                        Widgets.LabelFit(rect2Name, pawnType.label);
                        rect2.y += ySpace2;
                        rect2Name.y += ySpace2;
                        rect2Icon.y += ySpace2;
                    }
                }
                if (thingToRemove != null)
                {
                    this.pawnClassSelection.pawnTypes.Remove(thingToRemove);
                }

                this.editScroll1Height = (int)rect2.y - this.editScroll1Height;
                Widgets.EndScrollView();

                TooltipHandler.TipRegion(new Rect(rect3.x, rect3.y, rect3.width, 10 * ySpace2), "IH_ImmortalClassNonPawnsTip".Translate());
                Widgets.LabelFit(rect3, "IH_nonClassPawns".Translate());
                rect3.y += ySpace;

                Rect rect3Scroll = rect3;
                Rect rect3Icon = rect3;
                Rect rect3Name = rect3;
                rect3Icon.width = rect3.width * 0.2f;

                rect3Scroll.width -= 16;
                rect3Scroll.height = checkList.Count * ySpace2 - (this.pawnClassSelection.pawnTypes.Count * ySpace2);

                rect3Name.x += rect3Icon.width;
                rect3Name.width -= rect3Icon.width;

                Rect editScrollRect2 = new(rect3.x, rect3.y, rect3.width, 10 * ySpace2);
                Widgets.BeginScrollView(editScrollRect2, ref this.editScroll2, rect3Scroll);

                foreach (ThingDef pawnType in checkList)
                {
                    if (this.pawnClassSelection.pawnTypes.Contains(pawnType))
                    {
                        continue;
                    }
                    //Is part of default pawns
                    if (this.settings.defaultPawns.Contains(pawnType))
                    {
                        if (Widgets.ButtonText(rect3, pawnType.label))
                        {
                            this.pawnClassSelection.pawnTypes.Add(pawnType);
                            this.settings.defaultPawns.Remove(pawnType);
                        }

                        Widgets.DrawBoxSolidWithOutline(rect3, this.editingRed, Color.white);
                        Widgets.DefIcon(rect3Icon, pawnType);
                        Widgets.LabelFit(rect3Name, pawnType.label);
                        rect3.y += ySpace2;
                        rect3Name.y += ySpace2;
                        rect3Icon.y += ySpace2;
                    }
                    else //Is part of another immortal class
                    {
                        if (Widgets.ButtonText(rect3, pawnType.label))
                        {
                            foreach (Immortal_Class otherClass in this.settings.immortalClasses)
                            {
                                if (otherClass.pawnTypes.Contains(pawnType))
                                {
                                    otherClass.pawnTypes.Remove(pawnType);
                                }
                            }
                            this.pawnClassSelection.pawnTypes.Add(pawnType);
                        }
                        Widgets.DrawBoxSolidWithOutline(rect3, this.editingYellow, Color.white);
                        Widgets.DefIcon(rect3Icon, pawnType);
                        Widgets.LabelFit(rect3Name, pawnType.label);
                        rect3.y += ySpace2;
                        rect3Name.y += ySpace2;
                        rect3Icon.y += ySpace2;
                    }
                }
                Widgets.EndScrollView();

                rect2.y = editScrollRect1.y + Math.Min(rect2Scroll.height, editScrollRect1.height);
                rect3.y = editScrollRect2.y + Math.Min(rect3Scroll.height, editScrollRect2.height);

                if (this.shouldDelete)
                {
                    this.settings.DeleteImmortalClass(this.pawnClassSelection);
                    this.pawnClassSelection = null;
                    this.deleteConfirm = 0;
                    this.deleteClass = false;
                    this.shouldDelete = false;
                }
            }
        }

        private void EditHediffs(Rect drawRect, ref Rect rect1, ref Rect rect2, ref Rect rect3)
        {
            rect1.y += this.ySpace2;
            rect2.y += this.ySpace2;
            rect3.y += this.ySpace2;


            //Hediff editing Stuff

            if (this.hediffEditDef != null)
            {
                Widgets.LabelFit(rect1, this.hediffEditDef.label);
                rect1.y += this.ySpace;
                Widgets.DrawBoxSolid(new Rect(rect1.x, rect1.y, rect1.width, 5), this.hediffEditDef.defaultLabelColor);

                rect1.y += 5;
                float textHeight = Text.CalcHeight(this.hediffEditDef.description, rect1.width);
                Widgets.DrawBoxSolidWithOutline(new Rect(rect1.x, rect1.y, rect1.width, textHeight + 4), this.editingGray, this.hediffEditDef.defaultLabelColor);
                rect1.y += 2;
                Widgets.TextArea(new Rect(rect1.x, rect1.y, rect1.width, textHeight), this.hediffEditDef.description, true);
                rect1.y += textHeight;

                if (this.hediffEditSetting != null)
                {
                    bool condition = false;
                    bool disease = false;
                    bool other = false;

                    switch (this.hediffEditSetting.hediffType)
                    {
                        case HediffType.Condition:
                            condition = true;
                            disease = false;
                            other = false;
                            break;
                        case HediffType.Disease:
                            condition = false;
                            disease = true;
                            other = false;
                            break;
                        case HediffType.Other:
                            condition = false;
                            disease = false;
                            other = true;
                            break;

                    }
                    condition = Widgets.RadioButtonLabeled(rect1, "IH_condition".Translate(), condition);
                    rect1.y += this.ySpace;
                    disease = Widgets.RadioButtonLabeled(rect1, "IH_disease".Translate(), disease);
                    rect1.y += this.ySpace;
                    other = Widgets.RadioButtonLabeled(rect1, "IH_other".Translate(), other);
                    rect1.y += this.ySpace;

                    if (condition)
                    {
                        this.hediffEditSetting.hediffType = HediffType.Condition;
                    }

                    if (disease)
                    {
                        this.hediffEditSetting.hediffType = HediffType.Disease;
                    }

                    if (other)
                    {
                        this.hediffEditSetting.hediffType = HediffType.Other;
                    }

                    rect1.y += 12;

                    if (this.hediffEditSetting.hediffType != HediffType.Other)
                    {
                        this.SliderThreshold(ref rect1, ref this.hediffEditSetting.maxProgress, 0, 1, this.hediffFakeSetting.maxProgress.Value, "IH_hediffMaxProgress", 2);
                    }

                    this.BoolSliderThreshold(ref rect1, ref this.hediffEditSetting.canGet, this.hediffFakeSetting.canGet.Value, "IH_hediffCanGet");
                    this.BoolSliderThreshold(ref rect1, ref this.hediffEditSetting.needToCure, this.hediffFakeSetting.needToCure.Value, "IH_hediffneedToCure");



                    this.SliderThreshold(ref rect1, ref this.hediffEditSetting.healSpeed, -2, 2, 1, "IH_hediffHealSpeed", 2);



                    //BoolSliderThreshold(ref rect1, ref hediffEditSetting.regrowHediff, false, "IH_hediffRegrow");

                    if (this.hediffEditSetting.nonDefault() || this.hediffEditSetting.hediffType != this.hediffFakeSetting.hediffType)
                    {
                        if (!this.settings.hediffSettings.ContainsKey(this.hediffEditDef))
                        {
                            this.settings.hediffSettings.Add(this.hediffEditDef, this.hediffEditSetting);
                        }
                    }
                    else
                    {
                        if (this.settings.hediffSettings.ContainsKey(this.hediffEditDef))
                        {
                            this.settings.hediffSettings.Remove(this.hediffEditDef);
                        }
                    }

                }



                /*
                 public float? maxProgress;
                 public bool? isSlow;
                 public bool? needToCure;
                 public bool? canGet;
                 public bool? healHediff;
                 public bool? regrowHediff;
                */
            }


            //Hediff Sorting Stuff
            bool tempIsbad = this.hediffOnlyBad;
            bool tempCondition = this.hediffOnlyCondition;
            bool tempCustom = this.hediffOnlyCustom;

            this.hediffSearch = Widgets.TextField(rect2, this.hediffSearch);
            rect2.y += this.ySpace2;

            this.LabelCheckbox(ref rect2, ref this.hediffOnlyBad, "IH_hediffShowOnlyBad");
            this.LabelCheckbox(ref rect2, ref this.hediffOnlyCondition, "IH_hediffShowOnlyCondition");
            this.LabelCheckbox(ref rect2, ref this.hediffOnlyCustom, "IH_hediffShowOnlyCustom");
            if (tempIsbad != this.hediffOnlyBad || tempCondition != this.hediffOnlyCondition || this.hediffSearchLast != this.hediffSearch)
            {
                this.hediffReSearch = true;
            }

            this.hediffSearchLast = this.hediffSearch;
            if (this.hediffReSearch)
            {
                if (this.hediffSearch != "search" && this.hediffSearch != "" && this.hediffSearch != " ")
                {
                    this.hediffsSearch = DefDatabase<HediffDef>.AllDefs.Where(hediff => hediff.label.ToLower().Contains(this.hediffSearch.ToLower()));
                }
                else
                {
                    this.hediffsSearch = DefDatabase<HediffDef>.AllDefs;
                }

                if (this.hediffOnlyBad)
                {
                    this.hediffsSearch = this.hediffsSearch.Where(hediff => hediff.isBad);
                }

                if (this.hediffOnlyCondition)
                {
                    this.hediffsSearch = this.hediffsSearch.Where(hediff => hediff.lethalSeverity != -1);
                }

                if (this.hediffOnlyCustom)
                {
                    this.hediffsSearch = this.hediffsSearch.Where(hediff => this.settings.hediffSettings.ContainsKey(hediff));
                }
            }


            if (this.hediffsSearch == null)
            {
                this.hediffsSearch = DefDatabase<HediffDef>.AllDefs;
            }



            //Hediff List
            rect3.height = this.ySpace2;

            Rect rect3Scroll = rect3;
            Rect rect3Icon = rect3;
            Rect rect3Name = rect3;
            rect3Icon.width = rect3.width * 0.2f;

            rect3Scroll.width -= 16;
            rect3Scroll.height = this.hediffsSearch.Count() * this.ySpace2;

            rect3Name.x += rect3Icon.width;
            rect3Name.width -= rect3Icon.width;

            Rect editScrollRect2 = new(rect3.x, rect3.y, rect3.width, 10 * this.ySpace2);
            Widgets.BeginScrollView(editScrollRect2, ref this.editScroll2, rect3Scroll);


            foreach (HediffDef hediff in this.hediffsSearch)
            {
                if (Widgets.ButtonText(rect3, " "))
                {
                    this.hediffEditDef = hediff;
                    if (this.settings.hediffSettings.ContainsKey(hediff))
                    {
                        this.hediffEditSetting = this.settings.hediffSettings[hediff];
                        this.hediffFakeSetting = this.settings.HediffMakeSetting(hediff);
                    }

                    else
                    {
                        this.hediffEditSetting = new Hediff_Setting();
                        this.hediffFakeSetting = this.settings.HediffMakeSetting(hediff);
                        this.hediffEditSetting.hediffType = this.hediffFakeSetting.hediffType;
                    }

                }
                TooltipHandler.TipRegion(rect3, hediff.description);

                if (this.settings.hediffSettings != null)
                {
                    if (this.settings.hediffSettings.ContainsKey(hediff))
                    {
                        if (this.settings.IsImmortalsHediff(hediff))
                        {
                            Widgets.DrawBoxSolidWithOutline(rect3, this.editingCyan, hediff.defaultLabelColor);
                        }
                        else
                        {
                            Widgets.DrawBoxSolidWithOutline(rect3, this.editingYellow, hediff.defaultLabelColor);
                        }
                    }
                    else
                    {
                        Widgets.DrawBoxSolidWithOutline(rect3, this.editingGreen, hediff.defaultLabelColor);
                    }
                }
                else
                {
                    Widgets.DrawBoxSolidWithOutline(rect3, this.editingGreen, hediff.defaultLabelColor);
                }

                Widgets.LabelFit(rect3Name, hediff.label);


                rect3.y += this.ySpace2;
                rect3Name.y += this.ySpace2;
                rect3Icon.y += this.ySpace2;
            }

            rect3.y = editScrollRect2.y + Math.Min(rect3Scroll.height, editScrollRect2.height);

            Widgets.EndScrollView();
        }



        private static bool PosNegNeu(int val)
        {
            return false;
        }

        public override string SettingsCategory()
        {
            return "IH_Immortals".Translate();
        }

        private static float SnapToMult(float val, float mult)
        {
            val /= mult;
            return Mathf.RoundToInt(val) * mult;
        }

        private void FieldSlider(ref Rect rect, ref float value, float min, float max, string label, int round = -1)
        {
            string buffer = value.ToString();
            TooltipHandler.TipRegion(new Rect(rect.x, rect.y, rect.width, this.ySpace + this.ySpace), (label + "Tip").Translate());
            Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width * 0.8f, this.ySpace), label.Translate());
            Widgets.TextFieldNumeric(new Rect(rect.x + rect.width * 0.8f, rect.y, rect.width * 0.2f, this.ySpace), ref value, ref buffer, -1);
            rect.y += this.ySpace;
            value = Widgets.HorizontalSlider(new Rect(rect.x, rect.y, rect.width, this.ySpace), value, min, max);
            rect.y += this.ySpace;
            if (round != -1)
            {
                value = (float)Math.Round(value, round, MidpointRounding.AwayFromZero);
            }
        }

        private void LabelCheckbox(ref Rect rect, ref bool value, string label)
        {
            TooltipHandler.TipRegion(new Rect(rect.x, rect.y, rect.width, this.ySpace), (label + "Tip").Translate());
            Widgets.CheckboxLabeled(new Rect(rect.x, rect.y, rect.width, this.ySpace), label.Translate(), ref value);
            rect.y += this.ySpace;
        }

        private void Label(ref Rect rect, string label)
        {
            TooltipHandler.TipRegion(new Rect(rect.x, rect.y, rect.width, this.ySpace), (label + "Tip").Translate());
            Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width, this.ySpace), label.Translate());
            rect.y += this.ySpace;
        }

        private void SliderThreshold(ref Rect rect, ref float? var, float min, float max, float defaultValue, string label, int round = -1)
        {
            float threshold = min + (max - min);
            float thresholdMax = threshold * 1.1f;
            float value = thresholdMax;
            if (var != null)
            {
                value = var.Value;
                Widgets.DrawBoxSolid(rect, this.editingYellow);
            }
            else
            {
                Widgets.DrawBoxSolid(rect, this.editingGreen);
            }

            TooltipHandler.TipRegion(new Rect(rect.x, rect.y, rect.width, this.ySpace + 12), (label + "Tip").Translate());
            if (var == null)
            {
                Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width, this.ySpace), label.Translate() + "IH_DefaultNum".Translate(defaultValue));
            }
            else
            {
                Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width, this.ySpace), label.Translate() + value);
            }

            rect.y += this.ySpace;
            value = Widgets.HorizontalSlider(new Rect(rect.x, rect.y, rect.width, this.ySpace), value, min, thresholdMax);
            rect.y += 12;
            if (round != -1)
            {
                value = (float)Math.Round(value, round, MidpointRounding.AwayFromZero);
            }

            if (value > threshold * 1.05f)
            {
                var = null;
            }
            else
            {
                var = Mathf.Clamp(value, min, max);
            }
        }
        private void BoolSliderThreshold(ref Rect rect, ref bool? var, bool defaultValue, string label)
        {
            float value = 1;
            string boolStr;
            if (var != null)
            {
                if (var.Value)
                {
                    value = 0;
                    boolStr = "IH_yes".Translate();
                }
                else
                {
                    value = -1;
                    boolStr = "IH_no".Translate();
                }
                Widgets.DrawBoxSolid(rect, this.editingYellow);
            }
            else
            {
                Widgets.DrawBoxSolid(rect, this.editingGreen);
                if (defaultValue)
                {
                    boolStr = "IH_DefaultNum".Translate("IH_yes".Translate());
                }
                else
                {
                    boolStr = "IH_DefaultNum".Translate("IH_no".Translate());
                }
            }



            TooltipHandler.TipRegion(new Rect(rect.x, rect.y, rect.width, this.ySpace + 12), (label + "Tip").Translate());
            Widgets.LabelFit(new Rect(rect.x, rect.y, rect.width, this.ySpace), label.Translate() + boolStr);
            rect.y += this.ySpace;
            value = Widgets.HorizontalSlider(new Rect(rect.x, rect.y, rect.width, this.ySpace), value, -1, 1);
            rect.y += 12;
            value = (float)Math.Round(value, 0, MidpointRounding.AwayFromZero);

            if (value > 0.25)
            {
                var = null;
            }
            else
            {
                value = Mathf.Clamp(value, -1, 0);
                if (value == -1)
                {
                    var = false;
                }
                else
                {
                    var = true;
                }
            }


        }



        bool update = false;

        Vector2 scrollPos = new();
        Vector2 scrollPawnCategory = new();
        Vector2 stageScroll = new();
        Vector2 editScroll1 = new();
        Vector2 editScroll2 = new();
        int editScroll1Height = 0;
        int editScroll2Height = 0;


        bool confirmDefaultAll;
        bool defaultGeneral = false;
        bool defaultGeneralImmortal = false;
        bool defaultRegrowth = false;
        bool defaultDefaultImmortal = false;
        bool defaultImmortalClasses = false;
        bool defaultQuickeningChange = false;
        bool defaultHediffs = false;
        bool defaultTransfer = false;

        bool confirmStripImmortals;
        bool confirmRerollImmortals;


        bool lastDisease = false;
        bool pawnCategorySelection;
        Immortal_Class pawnClassSelection;

        bool hediffSelection;




        bool reloadPawnCategory = true;
        bool reloadHediffCategory = true;
        bool deleteClass = false;

        string classSearch = "search";
        string hediffSearch = "search";
        string hediffSearchLast;
        bool hediffReSearch = false;
        IEnumerable<HediffDef> hediffsSearch;
        bool hediffOnlyBad = true;
        bool hediffOnlyCondition = true;
        bool hediffOnlyCustom;
        bool hediffShowNonLethal;
        HediffDef hediffEditDef;
        Hediff_Setting hediffEditSetting;
        Hediff_Setting hediffFakeSetting;

        int totalHeight = 0;
        int section1Height = 0;
        int section2Height = 0;
        int section3Height = 0;

        int ySpace = 24;
        int ySpace2 = 36;

        int deleteConfirm;
        bool shouldDelete = false;

        Color editingRed = new(0.45f, 0.3f, 0.3f);
        Color editingGreen = new(0.3f, 0.45f, 0.3f);
        Color editingYellow = new(0.65f, 0.55f, 0.0f);
        Color editingCyan = new(0.0f, 0.5f, 0.5f);
        Color editingGray = new(0.2f, 0.2f, 0.2f);


    }
}
