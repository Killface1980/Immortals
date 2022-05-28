using Verse;

namespace Immortals
{

    public class Hediff_Setting : IExposable
    {
        public float? maxProgress;
        public float? healSpeed;
        public bool? needToCure;
        public bool? canGet;
        public bool? regrowHediff;
        public HediffType hediffType;

        public Hediff_Setting()
        {
            this.maxProgress = null;
            this.healSpeed = null;
            this.needToCure = null;
            this.canGet = null;
            this.regrowHediff = null;
            this.hediffType = HediffType.Other;
        }

        public Hediff_Setting(float? maxProgress, float? healSpeed, bool? needToCure, bool? canGet, bool? regrowHediff, HediffType hediffType)
        {
            this.maxProgress = maxProgress;
            this.healSpeed = healSpeed;
            this.needToCure = needToCure;
            this.canGet = canGet;
            this.regrowHediff = regrowHediff;
            this.hediffType = HediffType.Other;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.maxProgress, "maxProgress", null);
            Scribe_Values.Look(ref this.needToCure, "needToCure", null);
            Scribe_Values.Look(ref this.canGet, "canGet", null);
            Scribe_Values.Look(ref this.healSpeed, "healSpeed", null);
            Scribe_Values.Look(ref this.regrowHediff, "regrowHediff", null);
            Scribe_Values.Look(ref this.hediffType, "hediffType", HediffType.Other);
        }

        public bool nonDefault()
        {
            if (this.maxProgress != null)
            {
                return true;
            }

            if (this.needToCure != null)
            {
                return true;
            }

            if (this.canGet != null)
            {
                return true;
            }

            if (this.healSpeed != null)
            {
                return true;
            }

            if (this.regrowHediff != null)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            if (this.maxProgress != null)
            {
                Log.Message("MaxProg: " + this.maxProgress.Value);
            }

            if (this.needToCure != null)
            {
                Log.Message("needToCure " + this.needToCure);
            }

            if (this.canGet != null)
            {
                Log.Message("canGet: " + this.canGet);
            }

            if (this.healSpeed != null)
            {
                Log.Message("healSpeed: " + this.healSpeed);
            }

            if (this.regrowHediff != null)
            {
                Log.Message("regrow: " + this.regrowHediff);
            }

            return base.ToString();
        }

    }
}
