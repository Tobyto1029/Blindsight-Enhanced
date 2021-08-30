using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

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


    public class Hediff_Psysight : Hediff_Level
    {
        public static HediffDef Psysight;


        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
        }

        // The cause of the rapid add and remove bug
        public bool _shouldRemove = false;
        public override bool ShouldRemove
        {
            get { return _shouldRemove; }
        }

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                PsysightHandler.Updater(this.pawn);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
