﻿using HarmonyLib;
using PeteTimesSix.ResearchReinvented.Extensions;
using PeteTimesSix.ResearchReinvented.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PeteTimesSix.ResearchReinvented.HarmonyPatches.Prototypes
{
    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanConstruct), new Type[] { typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool), typeof(JobDef) })]
    public static class GenConstruct_CanConstruct_Patches
    {
        [HarmonyPostfix]
        public static bool CanConstruct(bool __result, Thing t, Pawn p, bool checkSkills, bool forced, JobDef jobForReservation) 
        {
            if (__result == false)
                return __result;

            if (!checkSkills)
                return __result;

            BuildableDef def = t.def;
            if (t is Blueprint blueprint)
                def = blueprint.def.entityDefToBuild;
            else if (t is Frame frame)
                def = frame.def.entityDefToBuild;

            if (!def.IsAvailableOnlyForPrototyping(true))
                return __result;

            if (!p.CanEverDoResearch())
            {
                JobFailReason.Is(StringsCache.JobFail_IncapableOfResearch, null);
                return false;
            }
            if (!p.workSettings?.WorkIsActive(WorkTypeDefOf.Research) ?? true)
            {
                JobFailReason.Is("NotAssignedToWorkType".Translate(WorkTypeDefOf.Research.gerundLabel).CapitalizeFirst(), null);
                return false;
            }

            return __result;
        }
    }
}
