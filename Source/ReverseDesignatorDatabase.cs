using Verse;

namespace Immortals.Source
{
    [HarmonyLib.HarmonyPatch(typeof(ReverseDesignatorDatabase), nameof(ReverseDesignatorDatabase.InitDesignators))]
    public static class ReverseDesignatorDatabase_Postfix
    {
        static void Postfix(ReverseDesignatorDatabase __instance)
        {
            __instance.desList.Add(new Designator_Behead());
            __instance.desList.Add(new Designator_ExtractorTrigger());
            __instance.desList.Add(new Designator_Impale());
        }
    }
}
