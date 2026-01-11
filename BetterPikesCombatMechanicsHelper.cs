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
        [HarmonyPatch("ComputeBlowMagnitude")]
        public static void Postfix2(AttackInformation attackInformation, ref float baseMagnitude)
        {
            if (BetterPikesHelper.IsPike(attackInformation.AttackerWeapon))
            {
                // Multiply the blow magnitude of pikes.
                baseMagnitude *= BetterPikesSettings.Instance.PikeBlowMagnitudeMultiplier;

                if (attackInformation.VictimAgent != null && attackInformation.VictimAgent.HasMount)
                {
                    // Further multiply the blow magnitude of pikes versus riders.
                    baseMagnitude *= 5;
                }
            }
        }
    }
}
