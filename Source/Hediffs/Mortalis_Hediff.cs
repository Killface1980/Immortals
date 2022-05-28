using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Immortals
{
    public class Mortalis_Hediff : Hediff_High
    {
        const float changePerDayMult = 0.0033333334f;

        float daysPerRelativeSeverity = 60;


        public override bool CauseDeathNow()
        {
            if (this.Severity < 1)
            {
                return false;
            }

            // Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal);
            // if (immortalHediff != null)
            if (this.pawn.IsImmmortal(out Hediff immortalHediff))
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

            float changeRate = 1 / this.daysPerRelativeSeverity * changePerDayMult;

            Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal, true);
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

                Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal, true);
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

                Hediff immortalHediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Immortals.IH_Immortal, true);
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
