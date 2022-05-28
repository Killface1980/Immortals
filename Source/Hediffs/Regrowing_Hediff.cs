using System;
using Verse;

namespace Immortals
{
    public class Regrowing_Hediff : Hediff
    {

        public BodyPartRecord forPart;

        public int partHp;

        public Regrowing_Hediff()
        {
            if (this.Part != null)
            {
                this.partHp = (int)Math.Round(this.Part.def.hitPoints * this.pawn.HealthScale);
                this.forPart = this.Part;
            }
            else
            {
                this.partHp = 1;
            }
        }
        public Regrowing_Hediff(BodyPartRecord part)
        {
            this.forPart = part;
            this.partHp = (int)Math.Round(part.def.hitPoints * this.pawn.HealthScale);
        }


        public override string SeverityLabel
        {
            get
            {
                if (this.Part != null)
                {
                    return (this.severityInt * this.Part.def.hitPoints * this.pawn.HealthScale).ToString("0.0") + "/" + this.Part.def.hitPoints * this.pawn.HealthScale;
                }
                else if (this.forPart != null && this.forPart != this.Part)
                {
                    return (this.severityInt * this.partHp).ToString("0.0") + "/" + this.partHp + " (" + this.forPart.Label + ")";
                }
                else
                {
                    return (this.severityInt * this.partHp).ToString("0.0") + "/" + this.partHp;
                }
            }
        }

        public override bool Visible
        {
            get
            {
                Hediff imDiff = this.pawn.GetImmortalHediff();

                if (!this.pawn.Dead || (imDiff != null && imDiff.Severity > 0.5))
                {
                    return true;
                }

                return false;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.partHp, "partHp");
            Scribe_BodyParts.Look(ref this.forPart, "forPart");
        }

        public override bool TryMergeWith(Hediff other)
        {
            return false;
        }
    }
}
