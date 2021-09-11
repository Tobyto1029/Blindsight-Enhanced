using Verse;

namespace Blindsight_Enhanced
{
    // Fixes Blindfold Hediff
    public class HediffComp_BlindfoldPsysightComps : HediffComp
    {
        public HediffCompProperties_BlindfoldPsysightComps Props
        {
            get
            {
                return (HediffCompProperties_BlindfoldPsysightComps)this.props;
            }
        }

        public override bool CompShouldRemove
        {
            get
            {
                // Log.Message("BlindfoldPsysightComps CompShouldRemove");
                return this.parent.pawn.health.hediffSet.HasHediff(PsysightHediffDefOf.Psysight);
            }
        }
    }

    public class HediffCompProperties_BlindfoldPsysightComps : HediffCompProperties
    {
        public HediffCompProperties_BlindfoldPsysightComps()
        {
            this.compClass = typeof(HediffComp_BlindfoldPsysightComps);
        }
    }
}
