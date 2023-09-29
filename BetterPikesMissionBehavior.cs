using HarmonyLib;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            // Determine which side to remove pikes from in siege battles.
            if (Mission.IsSiegeBattle && agent.IsHuman && ((agent.Team.IsAttacker && settings.SidesToRemovePikes.SelectedIndex == 1) || (agent.Team.IsDefender && settings.SidesToRemovePikes.SelectedIndex == 2) || settings.SidesToRemovePikes.SelectedIndex == 3))
            {
                for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
                {
                    if (agent.Equipment[index].CurrentUsageItem?.WeaponLength >= 400)
                    {
                        // Remove pikes in siege battles.
                        agent.RemoveEquippedWeapon(index);
                    }
                }
            }
        }

        public override void OnAgentPanicked(Agent affectedAgent) => affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);

        public override void OnMissionTick(float dt)
        {
            foreach (Team team in Mission.Teams)
            {
                foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty.Where(f => f.QuerySystem.IsInfantryFormation))
                {
                    bool shouldWalk = formation.CountOfUnits > 1 && formation.GetCountOfUnitsWithCondition(a => a.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation && formation.HasAnyEnemyFormationsThatIsNotEmpty() && !formation.IsLoose;

                    foreach (Agent agent in formation.UnitsWithoutLooseDetachedOnes.Cast<Agent>())
                    {
                        if (shouldWalk)
                        {
                            // If the pikemen's enemies are not routing, make the pikemen walk.
                            agent.SetScriptedFlags(agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun);
                            agent.SetMaximumSpeedLimit(agent.MaximumForwardUnlimitedSpeed, false);
                        }
                        else
                        {
                            agent.SetScriptedFlags(agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
                        }
                    }
                }
            }
        }

        public override void OnDeploymentFinished() => BetterPikesSubModule.HarmonyInstance.Patch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix")));

        protected override void OnEndMission() => BetterPikesSubModule.HarmonyInstance.Unpatch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix"));
    }
}
