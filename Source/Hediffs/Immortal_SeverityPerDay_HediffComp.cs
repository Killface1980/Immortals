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
    class Immortal_SeverityPerDay_HediffComp : HediffComp_SeverityPerDay
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
                this.immortalHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail("IH_Immortal");
            }

            return this.Props.severityPerDay;
        }
    }
}
