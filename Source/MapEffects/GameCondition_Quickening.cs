using Verse;
using RimWorld;

namespace Immortals
{
   public class GameCondition_Quickening : GameCondition
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Pawn>(ref this.pawn, "recievingPawn");
            Scribe_Values.Look(ref this.strikesLeft, "strikesLeft");

        }
        public override string Label => "Quickening";

        public override void Init()
        {
            base.Init();
            this.strikesLeft = Rand.RangeInclusive(10, 20);
            this.nextTick = Find.TickManager.TicksGame + Rand.RangeInclusive(30, 50);

        }

        public override void GameConditionTick()
        {

            if (this.pawn != null && ((!this.pawn.Dead && this.pawn.MapHeld != null) || (this.pawn.Corpse != null && this.pawn.Corpse.MapHeld != null) || this.lastPos != IntVec3.Zero))
            {
                if (Find.TickManager.TicksGame > this.nextTick)
                {
                    this.lastPos = this.pawn.Position;
                    int xOff = Rand.Range(-5, 5);
                    int zOff = Rand.Range(-5, 5);
                    IntVec3 pos = this.pawn.Position;
                    pos.x += xOff;
                    pos.z += zOff;
                    this.pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_Quickening(this.pawn.Map, pos, Rand.RangeInclusive(3, 10), false));
                    this.nextTick = Find.TickManager.TicksGame + Rand.RangeInclusive(20, 40);
                    this.strikesLeft -= 1;
                    if (this.strikesLeft <= 0)
                    {
                        this.End();
                    }
                }
            }
            else
            {
                this.End();
            }
        }

        public override void End()
        {
            base.End();
        }

        public IntVec3 lastPos = IntVec3.Zero;
        public Pawn pawn;
        int strikesLeft;
        int nextTick;
    }
}
