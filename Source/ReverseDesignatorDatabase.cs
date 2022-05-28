using System;
using Verse;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immortals.Source
{
    [HarmonyLib.HarmonyPatch(typeof(ReverseDesignatorDatabase), nameof(ReverseDesignatorDatabase.InitDesignators))]
    class ReverseDesignatorDatabasePatch
    {
        static void Postfix(ReverseDesignatorDatabase __instance)
        {
            __instance.desList.Add(new Designator_Behead());
            __instance.desList.Add(new Designator_ExtractorTrigger());
            __instance.desList.Add(new Designator_Impale());
        }
    }
}
