using RimWorld;
using Verse;
using System.Collections.Generic;

namespace Blindsight_Enhanced
{
    public static class PsysightHandler
    {

        public static void Giver(Pawn pawn)
        {
            if (!pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                // Log.Message(pawn + " Psysight_Giver");
                // Grant psysight on pawn Brain
                pawn.health.AddHediff(PsysightHediffDefOf.Psysight, pawn.health.hediffSet.GetBrain());
                Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
                def.SetLevelTo(pawn.GetPsylinkLevel());
            }
            
        }

        public static void Remover(Pawn pawn)
        {
            // Log.Message(pawn + " Psysight_Remover");
            Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
            def.ageTicks = 0;
            def._shouldRemove = true;
        }

        public static void Updater(Pawn pawn)
        {
            // Log.Message(pawn + " Psysight_Updater");
            if (pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight)) SetToPsylink(pawn);
            if (ShouldHavePsysightHediff(pawn) && !pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                Giver(pawn);
            }
            else if (pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight) && !ShouldHavePsysightHediff(pawn))
            {
                Remover(pawn);
            }
        }

        public static void SetToPsylink(Pawn pawn)
        {
            Hediff_Psysight def = (Hediff_Psysight)pawn.health.hediffSet.GetFirstHediffOfDef(PsysightHediffDefOf.Psysight);
            if (def != null && def.level != pawn.GetPsylinkLevel())
            {
                def.SetLevelTo(pawn.GetPsylinkLevel());
            }
        }

        public static bool HasSightSource(Pawn pawn)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            int MissingSightSourcesFound = 0;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff_MissingPart hediff_MissingPart;
                Hediff_AddedPart hediff_AddedPart;
                if (((hediff_MissingPart = hediffs[i] as Hediff_MissingPart) != null && hediff_MissingPart.Part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || ((hediff_AddedPart = hediffs[i] as Hediff_AddedPart) != null && hediff_AddedPart.Part.def.tags.Contains(BodyPartTagDefOf.SightSource) && hediff_AddedPart.def.addedPartProps.partEfficiency == 0))
                {
                    MissingSightSourcesFound += 1;
                }
            }
            if (MissingSightSourcesFound <= 1)
            {
                return true;
            }
            return false;
        }

        public static bool ShouldHavePsysightHediff(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                return !pawn.Dead && !HasSightSource(pawn);
            }
            else return !pawn.Dead && !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight);
        }
    }
}
