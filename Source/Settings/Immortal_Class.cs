using System.Collections.Generic;
using Verse;

namespace Immortals
{
    public class Immortal_Class : IExposable
    {
        public bool canBeImmortal           = true;
        public bool canSpawnImmortal        = true;
        public bool customHediffSettings    = false;
        public float? firstDeathHealFactor  = null;
        public float? healingRate           = null;
        public bool? healScars              = null;
        public float? immortalChance        = null;
        public float? immortalChanceVisible = null;
        public float? immortalSpawnMax      = null;
        public float? immortalSpawnMin      = null;
        public float? immortalTransferIn    = null;
        public float? immortalTransferOut   = null;
        public string name                  = "IH_Undefined";
        public List<ThingDef> pawnTypes     = new();
        public List<string> pawnTypesLoad;
        public float? regrowthFoodAccumulation = null;
        public float? regrowthFoodCost = null;
        public float? regrowthSpeed = null;

        public Immortal_Class()
        {
        }

        public Immortal_Class(string name)
        {
            this.name = name;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.canBeImmortal, "canBeImmortal", true);
            Scribe_Values.Look(ref this.canSpawnImmortal, "canSpawnImmortal", true);
            Scribe_Values.Look(ref this.immortalChance, "immortalChance", null);
            Scribe_Values.Look(ref this.immortalChanceVisible, "immortalChanceVisible", null);
            Scribe_Values.Look(ref this.immortalTransferOut, "immortalTransferOut", null);
            Scribe_Values.Look(ref this.immortalTransferIn, "immortalTransferIn", null);
            Scribe_Values.Look(ref this.immortalSpawnMin, "immortalSpawnMin", null);
            Scribe_Values.Look(ref this.immortalSpawnMax, "immortalSpawnMax", null);
            Scribe_Values.Look(ref this.healScars, "healScars", null);
            Scribe_Values.Look(ref this.name, "name", "IH_Noone");



            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (this.pawnTypesLoad == null)
                {
                    this.pawnTypesLoad = new List<string>();
                }

                foreach (ThingDef pawnType in this.pawnTypes)
                {
                    if (!this.pawnTypesLoad.Contains(pawnType.defName))
                    {
                        this.pawnTypesLoad.Add(pawnType.defName);
                    }
                }
            }
            Scribe_Collections.Look(ref this.pawnTypesLoad, "pawnTypesSave", LookMode.Value);
        }

    }
}
