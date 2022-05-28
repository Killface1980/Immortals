using Verse;

namespace Immortals
{
    public class Hunger_Hediff : Hediff
    {
        public Hunger_Hediff()
        {
            ;
        }

        public override void Tick()
        {
            base.Tick();

            if (this.pawn.needs.food != null)
            {
                if (this.pawn.needs.food.CurLevel > 0)
                {
                    if (this.Severity < 2.5)
                    {
                        this.Severity -= this.pawn.needs.food.FoodFallPerTick / 10;
                    }
                    else if (this.Severity < 5)
                    {
                        this.Severity -= this.pawn.needs.food.FoodFallPerTick / 4;
                    }
                    else
                    {
                        this.Severity -= this.pawn.needs.food.FoodFallPerTick / 2;
                    }
                }
            }
        }
    }
}
