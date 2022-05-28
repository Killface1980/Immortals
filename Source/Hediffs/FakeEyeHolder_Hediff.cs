using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Immortals
{
    class FakeEyeHolder_Hediff : Hediff
    {

        public BodyPartRecord place;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_BodyParts.Look(ref this.place, "IH_eyePlace");
        }
    }


}
