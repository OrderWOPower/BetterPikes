﻿using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnDeploymentFinished() => BetterPikesSubModule.HarmonyInstance.Patch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix")));

        public override void OnAgentPanicked(Agent affectedAgent) => affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);

        protected override void OnEndMission() => BetterPikesSubModule.HarmonyInstance.Unpatch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix"));
    }
}
