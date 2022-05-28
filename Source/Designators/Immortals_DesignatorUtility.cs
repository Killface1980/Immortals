using Verse;

namespace Immortals
{
    [HotSwappable]
    public static class Immortals_DesignatorUtility
    {

        private static Immortals_Settings settings = LoadedModManager.GetMod<Immortals_Mod>().GetSettings<Immortals_Settings>();

        // static HediffDef immortalHediff = DefDatabase<HediffDef>.GetNamed("IH_Immortal");

        public static bool CanBeBeheaded(Thing thing)
        {
            //Here is a thingerdo
            if (thing is Corpse)
            {
                if (HasConciousnessPart((thing as Corpse).InnerPawn))
                {
                    return true;
                }
            }//And another one woo!
            if (thing is Pawn)
            {
                if (HasConciousnessPart(thing as Pawn) && (thing as Pawn).Downed)
                {
                    return true;
                }
                else
                {
                    Designation designation = thing.Map.designationManager.DesignationOn(thing, DefDatabase<DesignationDef>.GetNamed("IH_Behead"));
                    if (designation != null)
                    {
                        designation.Delete();
                    }
                }

            }
            return false;
        }

        public static bool CanBeImpaled(Thing thing)
        {
            if (!CanBeBeheaded(thing))
            {
                return false;
            }
            //Here is a thingerdo
            if (thing is Corpse)
            {
                if (HasBloodPumpingPart((thing as Corpse).InnerPawn))
                {
                    return true;
                }
            }//And another one woo!
            if (thing is Pawn)
            {
                if (HasBloodPumpingPart(thing as Pawn) && (thing as Pawn).Downed)
                {
                    return true;
                }
                else
                {
                    Designation designation = thing.Map.designationManager.DesignationOn(thing, DefDatabase<DesignationDef>.GetNamed("IH_Impale"));
                    if (designation != null)
                    {
                        designation.Delete();
                    }
                }

            }
            return false;
        }

        public static bool CanBeUsedToTrigger(Thing thing)
        {
            if (!CanBeBeheaded(thing))
            {
                return false;
            }
            //Here is a thingerdo
            if (thing is Corpse)
            {
                if ((thing as Corpse).InnerPawn.IsVisibleImmortal())
                {
                    return true;
                }
            }//And another one woo!
            if (thing is Pawn)
            {
                if ((thing as Pawn).IsVisibleImmortal() && ((thing as Pawn).Downed || (thing as Pawn).IsPrisoner))
                {
                    return true;
                }
                else
                {
                    Designation designation = thing.Map.designationManager.DesignationOn(thing, DefDatabase<DesignationDef>.GetNamed("IH_ExtractorTrigger"));
                    if (designation != null)
                    {
                        designation.Delete();
                    }
                }

            }
            return false;
        }

        public static bool HasConciousnessPart(Pawn pawn)
        {
            foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts())
            {
                foreach (BodyPartTagDef tag in part.def.tags)
                {
                    if (tag.vital)
                    {
                        if (tag.defName == "ConsciousnessSource")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsConciousnessPart(BodyPartDef part)
        {
            foreach (BodyPartTagDef tag in part.tags)
            {
                if (tag.vital)
                {
                    if (tag.defName == "ConciousnessSource")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Immortal_Class GetPawnType(Pawn pawn, bool defaultType = false)
        {
            if (pawn != null)
            {
                if (settings.immortalClasses != null)
                {
                    foreach (Immortal_Class immortalClass in settings.immortalClasses)
                    {
                        if (immortalClass.pawnTypes != null && immortalClass.pawnTypes.Contains(pawn.def))
                        {
                            return immortalClass;
                        }
                    }
                }
            }

            return null;
        }

        public static bool HasBloodPumpingPart(Pawn pawn)
        {
            foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts())
            {
                foreach (BodyPartTagDef tag in part.def.tags)
                {
                    if (tag.vital)
                    {
                        if (tag.defName == "BloodPumpingSource")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public enum PawnType
        {
            humanoid,
            mechanoid,
            animal,
            unknown
        }

    }
}