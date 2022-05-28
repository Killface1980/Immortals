using System;
using Verse;

namespace Immortals
{
    public class MortalisCrystal_Hediff : Hediff
    {
        const float changePerDayMult = 0.0033333334f;

        float daysLeft;

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            this.daysLeft = 60;

        }


        public override void Tick()
        {
            base.Tick();
            float relativeSeverity = this.Severity;
            if (!base.pawn.IsHashIntervalTick(200))
            {
                return;
            }

            this.daysLeft -= changePerDayMult;
            if (this.daysLeft < 0)
            {
                // HediffDef immortalHediffDef = DefDatabase<HediffDef>.GetNamed("IH_Immortal");
                // Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(immortalHediffDef);
                Hediff immortalHediff = pawn.GetImmortalHediff();
                //HediffDef stuntedHediffDef = DefDatabase<HediffDef>.GetNamed("IH_Stunted");
                Hediff stuntedHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Stunted);

                if (Immortals_DesignatorUtility.IsConciousnessPart(this.Part.def))
                {
                    if (immortalHediff != null)
                    {
                        immortalHediff.Severity += 1;
                    }
                    else
                    {
                        immortalHediff = this.pawn.health.AddHediff(HediffDefOf_Immortals.IH_Immortal);
                        immortalHediff.Severity = 1;
                    }
                    if (stuntedHediff != null)
                    {
                        this.pawn.health.RemoveHediff(stuntedHediff);
                    }
                }
                else
                {
                    if (immortalHediff != null)
                    {
                        immortalHediff.Severity += 0.5f;
                        if (stuntedHediff != null)
                        {
                            immortalHediff.Severity = Math.Min(immortalHediff.Severity, 2);
                            stuntedHediff.Severity += 0.5f;
                        }
                    }
                    else
                    {
                        immortalHediff = this.pawn.health.AddHediff(HediffDefOf_Immortals.IH_Immortal);
                        immortalHediff.Severity = 0.6f;
                        stuntedHediff = this.pawn.health.AddHediff(HediffDefOf_Immortals.IH_Stunted);
                        stuntedHediff.Severity = 1 + Rand.Range(0, 1f);
                    }
                }
                this.pawn.health.RemoveHediff(this);
            }
        }



        public override string SeverityLabel
        {
            get
            {
                return "(" + "IH_DaysLeftSeverity".Translate() + " " + this.daysLeft.ToString("0.0") + ")";
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.daysLeft, "IH_daysLeft");
        }


    }


}
