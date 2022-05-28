using System;
using Verse;

namespace Immortals
{
    // Token: 0x020002C6 RID: 710
    public class Immortal_HediffCompProperties_SeverityPerDay : HediffCompProperties
    {
        // Token: 0x06001348 RID: 4936 RVA: 0x0006ED29 File Offset: 0x0006CF29
        public Immortal_HediffCompProperties_SeverityPerDay()
        {
            this.compClass = typeof(Immortal_SeverityPerDay_HediffComp);
        }

        // Token: 0x04000E86 RID: 3718
        public float severityPerDay;

        public bool speedUpWithPower;

        public float powerMultiplier;
    }
}
