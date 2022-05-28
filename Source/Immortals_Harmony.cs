

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using OpCodes = System.Reflection.Emit.OpCodes;



namespace Immortals
{
    [StaticConstructorOnStartup]
    internal static class ImmortalsHarmony
    {
        static ImmortalsHarmony()
        {

            Harmony harmony = new("rimworld.immortals.Fishbrains");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Harmony.DEBUG = false;

            harmony.Patch(original: AccessTools.Method(type: typeof(PawnGenerator), name: "GenerateInitialHediffs"), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(ImmortalsHarmony), nameof(BookIconTranspiler)));

        }
        public static IEnumerable<CodeInstruction> BookIconTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];


                if (instruction.opcode == OpCodes.Call && instruction.operand == AccessTools.Method(type: typeof(PawnAddictionHediffsGenerator), name: "GenerateAddictionsAndTolerancesFor"))
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(ImmortalsHarmony), name: nameof(ImmortalsHarmony.ChanceAddImmortal)));
                }
                yield return instruction;
            }
        }



        public static void ChanceAddImmortal(Pawn pawn)
        {
            Immortals_Settings settings = LoadedModManager.GetMod<Immortals_Mod>().GetSettings<Immortals_Settings>();
            if (pawn != null)
            {
                bool addPawn = false;
                if (pawn.health != null)
                {
                    if (pawn.health.hediffSet != null)
                    {
                        float rand = Rand.Range(0, 1f);
                        //Humanoid Check
                        Immortal_Class immortalClass = Immortals_DesignatorUtility.GetPawnType(pawn);

                        float chance = settings.immortalChance;
                        if (immortalClass != null)
                        {
                            if (immortalClass.canBeImmortal == false)
                            {
                                return;
                            }

                            if (immortalClass.immortalChance != null)
                            {
                                chance = immortalClass.immortalChance.Value;
                            }
                        }
                        if (rand <= chance)
                        {
                            HediffDef immortal = DefDatabase<HediffDef>.GetNamed("IH_Immortal");
                            pawn.health.AddHediff(immortal);
                            addPawn = true;
                            rand = Rand.Range(0, 1f);
                            chance = settings.immortalChanceVisible;
                            if (immortalClass != null)
                            {
                                if (immortalClass.immortalChanceVisible != null)
                                {
                                    chance = immortalClass.immortalChanceVisible.Value;
                                }
                            }
                            if (rand <= chance)
                            {
                                float min = settings.immortalSpawnMin;
                                float max = settings.immortalSpawnMax;
                                if (immortalClass != null)
                                {
                                    if (immortalClass.immortalSpawnMin != null)
                                    {
                                        min = immortalClass.immortalSpawnMin.Value;
                                    }

                                    if (immortalClass.immortalSpawnMax != null)
                                    {
                                        max = immortalClass.immortalSpawnMax.Value;
                                    }

                                    rand = Rand.Range(min, max);
                                    pawn.health.hediffSet.GetFirstHediffOfDef(immortal).Severity = rand;
                                }
                            }
                        }
                        if (addPawn)
                        {
                            Immortal_Component immortalComponent = Current.Game.GetComponent<Immortal_Component>();
                            if (immortalComponent == null)
                            {
                                Log.Message("Component is null");
                            }

                            if (pawn.Dead)
                            {
                                immortalComponent.AddDeadImmortal(pawn, false);
                            }
                            else
                            {
                                immortalComponent.AddImmortal(pawn, false);
                            }
                        }

                    }

                }

            }

        }



    }
    [AttributeUsage(AttributeTargets.Class)]
    public class HotSwappableAttribute : Attribute
    {
    }

}

