using Verse;

namespace Immortals
{
    public class FakeEyeHolder_Hediff : Hediff
    {

        public BodyPartRecord place;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_BodyParts.Look(ref this.place, "IH_eyePlace");
        }
    }


}
