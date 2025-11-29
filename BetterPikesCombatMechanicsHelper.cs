using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(MissionCombatMechanicsHelper))]
    public class BetterPikesCombatMechanicsHelper
    {
        [HarmonyPostfix]
        [HarmonyPatch("DecideMountRearedByBlow")]
        public static void Postfix1(ref bool __result, Agent attackerAgent)
        {
            if (attackerAgent.IsHuman && BetterPikesHelper.IsPike(attackerAgent.WieldedWeapon))
            {
                // Make pikes always rear mounts.
                __result = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("CalculateBaseMeleeBlowMagnitude")]
        public static void Postfix2(ref float __result, AttackInformation attackInformation)
        {
            if (BetterPikesHelper.IsPike(attackInformation.AttackerWeapon))
            {
                // Multiply the base blow magnitude of pikes.
                __result *= BetterPikesSettings.Instance.PikeBlowMagnitudeMultiplier;

                if (attackInformation.VictimAgent != null && attackInformation.VictimAgent.HasMount)
                {
                    // Further multiply the base blow magnitude of pikes versus riders.
                    __result *= 10;
                }
            }
        }
    }
}
