using Verse;
using RimWorld;

namespace Blindsight_Enhanced
{
    // Fixes Blindfold Hediff
    public class Comp_BlindfoldPsysightComps : ThingComp
    {
        private CompProperties_BlindfoldPsysightComps Props
        {
            get
            {
                return (CompProperties_BlindfoldPsysightComps)this.props;
            }
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            if (pawn != null && pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight))
            {
                if (pawn.health.hediffSet.HasHediff(Props.hediff)) pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff));
            }
        }


    }

    public class CompProperties_BlindfoldPsysightComps : CompProperties
    {
        public CompProperties_BlindfoldPsysightComps()
        {
            this.compClass = typeof(Comp_BlindfoldPsysightComps);
        }

        public HediffDef hediff;
        public BodyPartDef part;
    }
}
