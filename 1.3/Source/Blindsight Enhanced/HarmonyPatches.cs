using HarmonyLib;
using RimWorld;
using Verse;
using System;


namespace Blindsight_Enhanced
{

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            new Harmony("Blindsight_Enhanced").PatchAll();
            Log.Message("[Blindsight Enhanced] Patched");
            ApplySettings();
        }

        private static void ApplySettings()
        {
            HediffDef Psysight = DefDatabase<HediffDef>.GetNamed("Psysight");
            Psysight.stages[0].capMods[0].offset = BE_Settings.Lvl0;
            Psysight.stages[1].capMods[0].offset = BE_Settings.Lvl1;
            Psysight.stages[2].capMods[0].offset = BE_Settings.Lvl2;
            Psysight.stages[3].capMods[0].offset = BE_Settings.Lvl3;
            Psysight.stages[4].capMods[0].offset = BE_Settings.Lvl4;
            Psysight.stages[5].capMods[0].offset = BE_Settings.Lvl5;
            Psysight.stages[6].capMods[0].offset = BE_Settings.Lvl5;
            Log.Message("[Blindsight Enhanced] Settings Loaded");
        }
    }
    // Run updater when pawns are first created
    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public static class BE_SpawnSetup_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            PsysightHandler.Updater(__instance);
        }
    }


    // Run updated when a pawns health changes (if they get new eyes)
    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
    public static class BE_CheckForStateChange_Patch
    {
        public static void Postfix(Pawn ___pawn)
        {
            PsysightHandler.Updater(___pawn);
        }
    }

    // Update Psysight level when a new Psylink level is given
    [HarmonyPatch(typeof(Hediff_Psylink), "ChangeLevel", new Type[] { typeof(int) })]
    public static class BE_ChangeLevel_Patch
    {
        public static void Postfix(Hediff_Psylink __instance)
        {
            PsysightHandler.SetToPsylink(__instance.pawn);
        }
    }

    [HarmonyPatch(typeof(PawnCapacityWorker_Sight), "CalculateCapacityLevel")]
    public static class BE_CalculateCapacityLevel_Patch
    {
        public static HediffSet currentPawndiffSet;
        public static PawnCapacityDef capacity;
        [HarmonyPostfix]
        public static void Postfix(ref float __result, HediffSet diffSet)
        {
            currentPawndiffSet = diffSet;
            if (diffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                __result = BE_Settings.BaseSight;
            }
        }
    }


    [HarmonyPatch(typeof(PawnCapacityWorker_Sight), "CanHaveCapacity")]
    public static class BE_CanHaveCapacity_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            HediffSet diffSet = BE_CalculateCapacityLevel_Patch.currentPawndiffSet;
            if (diffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                __result = true;
            }
        }
    }


    [HarmonyPatch(typeof(PawnUtility), "IsBiologicallyBlind")]
    public static class BE_IsBiologicallyBlind_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Pawn pawn)
        {
            if (pawn != null && pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    // I could probably do something with a transpiler but it's above my skills
    [HarmonyPatch(typeof(StatPart_Glow), "ActiveFor")]
    public static class BE_ActiveFor_Patch
    {
        public static void Postfix(ref bool __result, ref bool ___humanlikeOnly, Thing t)
        {
            /*
            if (___humanlikeOnly)
            {
                Pawn pawn = t as Pawn;
                if (pawn != null && !pawn.RaceProps.Humanlike)
                {
                    __result = false;
                    return;
                }
            }
            */
            Pawn pawn2;
            if ((pawn2 = (t as Pawn)) != null && pawn2.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                __result = false;
                return;
            }
        }
    }

}
