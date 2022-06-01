// Verse.HediffCompProperties_Discoverable
using Verse;

namespace Immortals
{
    [HotSwappable]
    public class HediffCompProperties_DiscoverableImmortal : HediffCompProperties
    {
        public string discoverLetterLabel;
        public string discoverLetterText;
        public LetterDef letterType;
        public MessageTypeDef messageType;
        public bool sendLetterWhenDiscovered;
        
        public HediffCompProperties_DiscoverableImmortal()
        {
            compClass = typeof(HediffComp_DiscoverableImmortal);
        }
    }
}