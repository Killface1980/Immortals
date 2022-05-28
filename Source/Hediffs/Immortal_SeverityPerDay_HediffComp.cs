using Verse;

namespace Immortals
{
    public class Immortal_SeverityPerDay_HediffComp : HediffComp_SeverityPerDay
    {
        HediffDef immortalHediffDef;


        private Immortal_HediffCompProperties_SeverityPerDay Props
        {
            get
            {
                return (Immortal_HediffCompProperties_SeverityPerDay)this.props;
            }
        }


        public override float SeverityChangePerDay()
        {
            if (this.immortalHediffDef == null)
            {
                this.immortalHediffDef = HediffDefOf_Immortals.IH_Immortal;
            }

            return this.Props.severityPerDay;
        }
    }
}
