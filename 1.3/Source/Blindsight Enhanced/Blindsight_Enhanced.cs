using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blindsight_Enhanced
{
    [DefOf]
    public static class PsysightHediffDefOf
    {
        public static HediffDef Psysight;

        static PsysightHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PsysightHediffDefOf));
        }
    }

    /* Maybe don't need this?
    public class Psysight : HediffDef
    {

    }
    */

    public  class Hediff_Psysight : Hediff_Level
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
            BackCompatibility.PostExposeData(this);
            Scribe_Values.Look(ref level, "level", 0);
        }
    }

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            new Harmony("Blindsight_Enhanced").PatchAll();
            Log.Message("Blindsight Enhanced is Loaded and Patched, Thank you for using my mod!");
        }

        public static bool ShouldHavePsysightHediff(this Pawn pawn)
        {
            bool hasSight = true;
            int missingSources = 0;
            // Check if pawn has sight 
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                // Check if eyes are missing, then check if prothetic adds sight
                if (((hediffs[i] as Hediff_MissingPart) != null && hediffs[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || ((hediffs[i] as Hediff_AddedPart) != null && hediffs[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource) && hediffs[i].def.addedPartProps.partEfficiency == 0))
                {
                    missingSources += 1;
                }
            }
            if (missingSources >= 2)
            {
                hasSight = false;
            }

            return !pawn.Dead && pawn.Ideo != null && pawn.Ideo.IdeoApprovesOfBlindness() && !hasSight;
        }
    }



    public static class PsysightHandler
    {
        public static void Giver(Pawn pawn)
        {
            if (pawn.ShouldHavePsysightHediff() && pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight));
            {
                pawn.health.AddHediff(PsysightHediffDefOf.Psysight);

                int psyLevel = pawn.GetPsylinkLevel();
                Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
                if (psyLevel >= 1)
                {
                    def.SetLevelTo(psyLevel);
                } else
                {
                    def.SetLevelTo(1);
                }
            }
        }

        public static void Remover(Pawn pawn)
        {
            if (!pawn.ShouldHavePsysightHediff() && pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
                def.ageTicks = 0;
                def.SetLevelTo(0);
                def.Severity = 0f;
            }
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

    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public static class Pawn_SpawnSetup_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.ShouldHavePsysightHediff())
            {
                PsysightHandler.Updater(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
    public static class Pawn_CheckForStateChange_Patch
    {
        public static void Postfix(Pawn ___pawn)
        {
            PsysightHandler.Updater(___pawn);
        }
    }

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
                __result = 0.01f; //must not be 0 otherwise the game will clamp Sight stat at 0, therefore all offsets of mod hediffs are 1 unit below what they should be in XML
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
}
