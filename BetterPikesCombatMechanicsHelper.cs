using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "CalculateBaseMeleeBlowMagnitude")]
    public class BetterPikesCombatMechanicsHelper
    {
        public static void Postfix(ref float __result, AttackInformation attackInformation, MissionWeapon weapon)
        {
            Agent victim = attackInformation.VictimAgent;
            string itemUsage = weapon.CurrentUsageItem?.ItemUsage;

            if (victim != null && (victim.HasMount || victim.IsMount) && (itemUsage == "polearm_pike" || itemUsage == "polearm_bracing"))
            {
                __result *= 10;
            }
        }
    }
}
