using Verse;
using RimWorld;

namespace Immortals
{
    public class FakeEye_Hediff : Hediff_AddedPart
    {

        private ThingDef stuff;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref this.stuff, "IH_eyeStuff");
        }



        public ThingDef Stuff
        {
            get
            {
                return this.stuff;
            }
            set
            {
                this.stuff = value;
                foreach (StatModifier stat in this.stuff.stuffProps.statFactors)
                {
                    if (stat.stat.label.ToLower() == "beauty")
                    {
                        this.Severity = stat.value + 1;
                    }
                }
            }
        }

        static int GetSeverityFromBeauty(float beauty)
        {
            if (beauty <= 1)
            {
                return 1;
            }

            if (beauty <= 2)
            {
                return 2;
            }

            if (beauty <= 4)
            {
                return 3;
            }

            if (beauty <= 6)
            {
                return 4;
            }

            return 5;
        }

        public override string Label
        {
            get
            {
                if (this.stuff == null)
                {
                    return "Fake Eye";
                }

                return this.stuff.LabelAsStuff + " Fake Eye";
            }
        }
    }


}
