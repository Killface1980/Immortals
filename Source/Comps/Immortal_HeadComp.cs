using System;
using Verse;

namespace Immortals
{
    [HotSwappable]
    public class Immortal_HeadComp : ThingComp
    {
        public int ImmortalLevel { get => this.immortalLevel; }


        public Immortal_HeadComp()
        {
            this.immortalLevel = Rand.Range(0, 3);
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref this.immortalLevel, "IH_imLevel", 0);
        }

        public void SetImmortalLevel(float level)
        {
            int newLevel = (int)Math.Ceiling(level);
            if (newLevel == 2 || newLevel == 4 || newLevel == 7 || newLevel == 10)
            {
                newLevel -= 1;
            }

            if (level <= 0.5f)
            {
                level = 0;
            }
            else
            {
                this.immortalLevel = newLevel;
            }
        }

        public override string TransformLabel(string label)
        {
            string newLabel = label + " - ";
            if (this.immortalLevel == 0)
            {
                return label;
            }
            else if (this.immortalLevel <= 1)
            {
                newLabel += "IH_lesser".Translate();
            }
            else if (this.immortalLevel < 2)
            {
                newLabel += "IH_immortal".Translate();
            }
            else if (this.immortalLevel < 4)
            {
                newLabel += "IH_greater".Translate();
            }
            else if (this.immortalLevel < 7)
            {
                newLabel += "IH_high".Translate();
            }
            else if (this.immortalLevel < 10)
            {
                newLabel += "IH_grand".Translate();
            }
            else
            {
                newLabel += "IH_apex".Translate();
            }

            if (this.immortalLevel != 0)
            {
                newLabel = newLabel + " (" + this.immortalLevel + ")";
            }

            return newLabel;
        }


        public void SetUp(float level)
        {
            this.SetUp(level);
        }

        public override void PostIngested(Pawn ingester)
        {
            if (this.immortalLevel != 0)
            {
                //HediffDef imDifDef = HediffDefOf_Immortals.IH_Immortal;// DefDatabase<HediffDef>.GetNamed("IH_Immortal");
                Hediff imDif = ingester.GetImmortalHediff();// ingester.health.hediffSet.GetFirstHediffOfDef(imDifDef);
                if (imDif == null)
                {
                    float chance = 0;

                    Immortals_Settings settings;
                    settings = LoadedModManager.GetMod<Immortals_Mod>().GetSettings<Immortals_Settings>();

                    if (this.immortalLevel < 2)
                    {
                        chance = settings.stage1StealChance;
                    }
                    else if (this.immortalLevel < 4)
                    {
                        chance = settings.stage2StealChance;
                    }
                    else if (this.immortalLevel < 7)
                    {
                        chance = settings.stage3StealChance;
                    }
                    else if (this.immortalLevel < 10)
                    {
                        chance = settings.stage4StealChance;
                    }
                    else
                        if (this.immortalLevel >= 10)
                    {
                        chance = settings.stage5StealChance;
                    }

                    float val = Rand.Range(0, 1f);
                    if (val <= chance)
                    {
                        imDif = ingester.health.AddHediff(HediffDefOf_Immortals.IH_Immortal);
                        imDif.Severity = 0.5f;

                        val = Rand.Range(0, 1f);
                        if (val < settings.stuntedChanceHead)
                        {
                            ingester.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_StuntedProc);
                        }
                    }
                }
            }
        }

        public override bool AllowStackWith(Thing other)
        {
            Immortal_HeadComp otherHead;
            otherHead = other.TryGetComp<Immortal_HeadComp>();
            if (otherHead != null)
            {
                if (this.immortalLevel == otherHead.immortalLevel)
                {
                    return true;
                }
            }
            return false;
        }


        private int immortalLevel;
    }

}
