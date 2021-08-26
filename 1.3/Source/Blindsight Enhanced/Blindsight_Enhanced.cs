using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blindsight_Enhanced
{
    #region Hediff Definiton

    [DefOf]
    public static class PsysightHediffDefOf
    {
        public static HediffDef Psysight;

        static PsysightHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PsysightHediffDefOf));
        }
    }

    public class Hediff_Psysight : Hediff_Level
    {
        public static HediffDef Psysight;
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            Log.Message("Psysight was successfully added");
        }

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                PsysightHandler.Updater(this.pawn);
            }
        }
    }

    #endregion

    #region Helper Functions

    public static class PsysightHandler
    {

        public static void Giver(Pawn pawn)
        {
            if (!pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                // Grant psysight on pawn Brain
                pawn.health.AddHediff(PsysightHediffDefOf.Psysight, pawn.health.hediffSet.GetBrain());
                Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
                def.SetLevelTo(pawn.GetPsylinkLevel());
            }
            
        }

        public static void Remover(Pawn pawn)
        {
            Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
            def.ageTicks = 0;
            def.SetLevelTo(0);
            def.Severity = 0f;
        }

        public static void Updater(Pawn pawn)
        {
            if (ShouldHavePsysightHediff(pawn) && !pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                Giver(pawn);
            }
            else if (pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight) && !ShouldHavePsysightHediff(pawn))
            {
                Remover(pawn);
            }
        }

        public static bool ShouldHavePsysightHediff(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                int nonfunctionalEyes = 0;
                for (int i = 1; i < hediffs.Count; i++)
                {
                    if ((hediffs[i] is Hediff_MissingPart && hediffs[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || (hediffs[i] is Hediff_AddedPart && hediffs[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource) && hediffs[i].def.addedPartProps.partEfficiency == 0))
                    {
                        nonfunctionalEyes += 1;
                    }
                    if (nonfunctionalEyes == 2) return true;
                }
                return !(nonfunctionalEyes >= 2);
            }
            else return !pawn.Dead && !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight);
        }
    }

    #endregion

    #region Harmony Patches

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            new Harmony("Blindsight_Enhanced").PatchAll();
            Log.Message("[Blindsight Enhanced] Loaded and Patched, Thank you for using my mod!");
        }
    }

    
    // Run updater when pawns are first created
    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public static class BE_SpawnSetup_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            Log.Message("Psysight SpawnSetup");

            PsysightHandler.Updater(__instance);
        }
    }


    // Run updated when a pawns health changes (if they get new eyes)
    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
    public static class BE_CheckForStateChange_Patch
    {
        public static void Postfix(Pawn ___pawn)
        {
            Log.Message("Psysight CheckForStateChange");
            PsysightHandler.Updater(___pawn);
        }
    }

    // Update Psysight level when a new Psylink level is given
    [HarmonyPatch(typeof(Hediff_Psylink), "ChangeLevel", new Type[] { typeof(int) })]
    public static class BE_ChangeLevel_Patch
    {
        public static void Postfix(Hediff_Psylink __instance)
        {
            PsysightHandler.Giver(__instance.pawn);
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
                __result = 0.25f; // Unclamps sight from 0% to a base of 25%
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
    #endregion

}
