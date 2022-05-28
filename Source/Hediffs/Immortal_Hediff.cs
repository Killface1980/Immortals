using Verse;

namespace Immortals
{
    public class Immortal_Hediff : Hediff
    {
        public override void Notify_PawnDied()
        {
            Immortal_Component immortalComponent = Current.Game.GetComponent<Immortal_Component>();
            if (immortalComponent != null)
            {
                if (immortalComponent.loaded)
                {
                    immortalComponent.AddDeadImmortal(this.pawn);
                }
            }
        }

        public override string SeverityLabel
        {

            get
            {
                Immortals_Settings settings = LoadedModManager.GetMod<Immortals_Mod>().GetSettings<Immortals_Settings>();
                if (settings.revealImmortalityNumber)
                {
                    return this.Severity.ToString("0.0");
                }

                return "";
            }
        }
    }


}
