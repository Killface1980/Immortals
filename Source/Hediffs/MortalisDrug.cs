using Verse;
using RimWorld;

namespace Immortals
{

    public class MortalisDrug : Hediff_High
    {

        public override void PostAdd(DamageInfo? dinfo)
        {
            Immortal_Component immortalComponent = Current.Game.GetComponent<Immortal_Component>();
            if (immortalComponent != null)
            {
                if (immortalComponent.loaded)
                {
                    immortalComponent.AddImmortal(this.pawn, false);

                }
            }
            base.PostAdd(dinfo);
        }
    }
}