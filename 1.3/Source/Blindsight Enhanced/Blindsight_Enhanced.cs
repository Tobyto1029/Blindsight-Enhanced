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
            // Log.Message("Psysight was successfully added");
        }

        public override void Tick()
        {
            base.Tick();
            PsysightHandler.Updater(pawn);
        }

        // Taking ExposeData() from Hediff instead of Hediff_Level
        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving && this.combatLogEntry != null)
            {
                LogEntry target = this.combatLogEntry.Target;
                if (target == null || !Current.Game.battleLog.IsEntryActive(target))
                {
                    this.combatLogEntry = null;
                }
            }
            Scribe_Values.Look(ref loadID, "loadID", 0);
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref ageTicks, "ageTicks", 0);
            Scribe_Defs.Look(ref source, "source");
            Scribe_Defs.Look(ref sourceBodyPartGroup, "sourceBodyPartGroup");
            Scribe_Defs.Look(ref sourceHediffDef, "sourceHediffDef");
            Scribe_Values.Look(ref severityInt, "severity", 0f);
            Scribe_Values.Look(ref causesNoPain, "causesNoPain", defaultValue: false);
            Scribe_References.Look(ref combatLogEntry, "combatLogEntry");
            Scribe_Values.Look(ref combatLogText, "combatLogText");
            Scribe_Values.Look(ref level, "level", 0);
            BackCompatibility.PostExposeData(this);
        }
    }

    #endregion

    #region Helper Functions

    public static class PsysightHandler
    {

        public static void Giver(Pawn pawn)
        {
            if (!pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight));
            {
                // Grant psysight on pawn Brain
                Hediff_Psysight psysight = (HediffMaker.MakeHediff(PsysightHediffDefOf.Psysight, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Psysight);
                pawn.health.AddHediff(psysight, null, null, null);
            }
            // Set the level of psysight to the psylink level (even if it's 0) 
            Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
            def.SetLevelTo(pawn.GetPsylinkLevel());
            
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
            if (!pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight) && pawn.ShouldHavePsysightHediff())
            {
                Giver(pawn);
            }
            if (pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight) && !pawn.ShouldHavePsysightHediff())
            {
                Remover(pawn);
            }
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
            Log.Message("Blindsight Enhanced is Loaded and Patched, Thank you for using my mod!");
        }

        // Accessable on all pawns
        public static bool ShouldHavePsysightHediff(this Pawn pawn)
        {
            bool hasSight = true;
            int missingSources = 0;
            // Check if pawn has sight 
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                // Check if eyes are missing OR if prothetic is nonfunctional
                if (((hediffs[i] as Hediff_MissingPart) != null && hediffs[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || ((hediffs[i] as Hediff_AddedPart) != null && hediffs[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource) && hediffs[i].def.addedPartProps.partEfficiency == 0))
                {
                    missingSources += 1;
                }
            }
            // if both eyes are missing or non-functional, the pawn doesn't have sight
            if (missingSources >= 2)
            {
                hasSight = false;
            }

            return !pawn.Dead && pawn.Ideo != null && pawn.Ideo.IdeoApprovesOfBlindness() && !hasSight;
        }
    }

    // Run updater when pawns are first created
    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public static class BE_SpawnSetup_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.ShouldHavePsysightHediff())
            {
                PsysightHandler.Updater(__instance);
            }
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
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn pawn)
        {
            if (pawn.ShouldHavePsysightHediff())
            {
                __result = true;
            }
        }
    }

    // I could probably do something with a transpiler but it's above my skills
    [HarmonyPatch(typeof(StatPart_Glow), "ActiveFor")]
    public static class BE_ActiveFor_Patch
    {
        public static void Postfix(ref bool __result, ref bool ___humanlikeOnly, Thing t)
        {
            if (___humanlikeOnly)
            {
                Pawn pawn = t as Pawn;
                if (pawn != null && !pawn.RaceProps.Humanlike)
                {
                    __result = false;
                    return;
                }
            }
            Pawn pawn2;
            if ((pawn2 = (t as Pawn)) != null && pawn2.ShouldHavePsysightHediff())
            {
                __result = false;
                return;
            }
        }
    }
    #endregion
}
