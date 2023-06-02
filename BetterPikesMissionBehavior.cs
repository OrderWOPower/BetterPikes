using HarmonyLib;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnDeploymentFinished() => BetterPikesSubModule.HarmonyInstance.Patch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix")));

        public override void OnAgentPanicked(Agent affectedAgent) => affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);

        public override void OnMissionTick(float dt)
        {
            foreach (Team team in Mission.Teams)
            {
                foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty.Where(f => f.CountOfUnits > 1 && f.GetCountOfUnitsWithCondition(a => a.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400) >= f.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation))
                {
                    foreach (Agent agent in formation.UnitsWithoutLooseDetachedOnes)
                    {
                        // If the pikemen's enemies are not routing, make the pikemen walk.
                        agent.SetScriptedFlags(formation.QuerySystem.ClosestEnemyFormation?.Formation.CountOfUnits > 1 ? agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun : agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
                        agent.SetMaximumSpeedLimit(agent.MaximumForwardUnlimitedSpeed, false);
                    }
                }
            }
        }

        protected override void OnEndMission() => BetterPikesSubModule.HarmonyInstance.Unpatch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix"));
    }
}
