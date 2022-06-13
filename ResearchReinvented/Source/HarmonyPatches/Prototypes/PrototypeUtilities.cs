﻿using PeteTimesSix.ResearchReinvented.Defs;
using PeteTimesSix.ResearchReinvented.Extensions;
using PeteTimesSix.ResearchReinvented.Managers;
using PeteTimesSix.ResearchReinvented.OpportunityComps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.ResearchReinvented.HarmonyPatches.Prototypes
{
    public static class PrototypeUtilities
    {

        private static float PROTOTYPE_QUALITY_MULTIPLIER = 0.3f;
        private static float PROTOTYPE_HEALTH_MULTIPLIER = 0.3f;

        public static float PROTOTYPE_WORK_MULTIPLIER = 3.0f;


        public static void DoPrototypeHealthDecrease(Thing product)
        {
            bool isPrototype = product.def.IsAvailableOnlyForPrototyping();
            if (isPrototype)
            {
                product.HitPoints = (int)Math.Max(1, (product.HitPoints * PROTOTYPE_HEALTH_MULTIPLIER));
            }
        }

        public static QualityCategory DoPrototypeQualityDecreaseThing(QualityCategory category, Thing product, Pawn worker)
        {
            bool isPrototype = product.def.IsAvailableOnlyForPrototyping();
            if (isPrototype)
            {
                byte asByte = (byte)category;
                var adjusted = (QualityCategory)Math.Max((byte)0, (byte)Math.Round((float)asByte * PROTOTYPE_QUALITY_MULTIPLIER));
                Log.Message($"adjusted quality for product {product} (worker {worker}): {category} to {adjusted}");
                return adjusted;
            }
            return category;
        }

        public static QualityCategory DoPrototypeQualityDecreaseRecipe(QualityCategory category, Thing product, RecipeDef recipe, Pawn worker)
        {
            bool isPrototype = recipe.IsAvailableOnlyForPrototyping();
            if (isPrototype)
            {
                byte asByte = (byte)category;
                var adjusted = (QualityCategory)Math.Max((byte)0, (byte)Math.Round((float)asByte * PROTOTYPE_QUALITY_MULTIPLIER));
                Log.Message($"adjusted quality for product {product} (recipe: {recipe} worker {worker}): {category} to {adjusted}");
                return adjusted;
            }
            return category;
        }

        public static void DoPostFinishThingResearch(Thing product, Pawn worker, float totalWork, RecipeDef usedRecipe = null)
        {
            bool isPrototype = product.def.IsAvailableOnlyForPrototyping() || (usedRecipe != null && usedRecipe.IsAvailableOnlyForPrototyping());
            if (isPrototype)
            {
                var opportunity = ResearchOpportunityManager.instance.CurrentOpportunities
                    .Where(o => o.def.handledBy == HandlingMode.Special_Prototype && o.requirement is ROComp_RequiresThing)
                    .Where(o => (o.requirement as ROComp_RequiresThing).targetDef == product.def)
                    .Where(o => !o.IsFinished).FirstOrDefault();

                if (opportunity != null)
                {
                    opportunity.ResearchChunkPerformed(worker);
                    var xp = totalWork;
                    Log.Message($"adding {xp} to intellectual skill");
                    worker.skills.Learn(SkillDefOf.Intellectual, xp, false);
                }
            }
        }

        public static void DoPostFinishTerrainResearch(TerrainDef terrainDef, Pawn worker, float totalWork)
        {
            bool isPrototype = terrainDef.IsAvailableOnlyForPrototyping();
            if (isPrototype)
            {
                var opportunity = ResearchOpportunityManager.instance.CurrentOpportunities
                    .Where(o => o.def.handledBy == HandlingMode.Special_Prototype && o.requirement is ROComp_RequiresTerrain)
                    .Where(o => (o.requirement as ROComp_RequiresTerrain).terrainDef == terrainDef)
                    .Where(o => !o.IsFinished).FirstOrDefault();

                if (opportunity != null)
                {
                    opportunity.ResearchChunkPerformed(worker);
                    var xp = totalWork;
                    Log.Message($"adding {xp} to intellectual skill");
                    worker.skills.Learn(SkillDefOf.Intellectual, xp, false);
                }
            }
        }
    }
}
