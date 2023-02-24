using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnDeploymentFinished() => BetterPikesSubModule.HarmonyInstance.Patch(AccessTools.Method(typeof(BehaviorCharge), "CalculateCurrentOrder"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesBehaviorCharge), "Postfix")));

        // Make pikemen drop their pike if they are hit by a melee weapon and their HP is below a certain limit.
        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
        {
            if (affectedAgent.IsActive() && affectedAgent.IsHuman && !affectedAgent.IsMainAgent && affectedAgent.Health < affectedAgent.HealthLimit * BetterPikesSettings.Instance.MaxPikemanHealthPercentToDropPike && affectedAgent.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400 && affectorAgent.IsHuman && affectorWeapon.CurrentUsageItem != null && affectorWeapon.CurrentUsageItem.IsMeleeWeapon)
            {
                for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
                {
                    if (affectedAgent.Equipment[index].CurrentUsageItem == affectedAgent.WieldedWeapon.CurrentUsageItem)
                    {
                        affectedAgent.HandleDropWeapon(false, index);
                    }
                }
            }
        }
    }
}
