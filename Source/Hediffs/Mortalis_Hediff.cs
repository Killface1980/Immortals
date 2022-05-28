using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Immortals
{
    class Mortalis_Hediff : Hediff_High
    {
        const float changePerDayMult = 0.0033333334f;

        float daysPerRelativeSeverity = 60;


        HediffDef immortalHediffDef;

        public override bool CauseDeathNow()
        {
            if (this.Severity < 1)
            {
                return false;
            }

            if (this.immortalHediffDef == null)
            {
                this.immortalHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail("IH_Immortal");
            }

            Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(this.immortalHediffDef);
            if (immortalHediff != null)
            {
                if (immortalHediff.Severity < this.Severity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public override void Tick()
        {
            base.Tick();
            float relativeSeverity = this.Severity;
            if (!base.pawn.IsHashIntervalTick(200))
            {
                return;
            }

            if (this.immortalHediffDef == null)
            {
                this.immortalHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail("IH_Immortal");
            }

            float changeRate = 1 / this.daysPerRelativeSeverity * changePerDayMult;

            Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(this.immortalHediffDef, true);
            if (immortalHediff != null && immortalHediff.Severity > 1)
            {
                changeRate *= immortalHediff.Severity;
            }

            this.Severity -= changeRate;
        }

        public override int CurStageIndex
        {
            get
            {
                if (this.def.stages == null)
                {
                    return 0;
                }
                float severity = this.Severity;
                if (this.immortalHediffDef == null)
                {
                    this.immortalHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail("IH_Immortal");
                }

                Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(this.immortalHediffDef, true);
                if (immortalHediff != null)
                {
                    if (immortalHediff.Severity > 1)
                    {
                        severity = this.Severity / immortalHediff.Severity;
                    }
                }

                List<HediffStage> stages = this.def.stages;
                for (int i = stages.Count - 1; i >= 0; i--)
                {
                    if (severity >= stages[i].minSeverity)
                    {
                        return i;
                    }
                }
                return 0;
            }
        }

        public override string SeverityLabel
        {
            get
            {
                if (this.Severity <= 0f)
                {
                    return null;
                }
                float severity = this.Severity;
                if (this.immortalHediffDef == null)
                {
                    this.immortalHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail("IH_Immortal");
                }

                Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(this.immortalHediffDef, true);
                if (immortalHediff != null)
                {
                    if (immortalHediff.Severity > 1)
                    {
                        severity = this.Severity / immortalHediff.Severity;
                    }
                }
                string label = severity.ToStringPercent("F0");
                float daysLeft = severity * this.daysPerRelativeSeverity;
                label = label + "\n   (" + "IH_DaysLeftSeverity".Translate() + " " + daysLeft.ToString("0.0") + ")";

                return label;
            }
        }

    }


}
