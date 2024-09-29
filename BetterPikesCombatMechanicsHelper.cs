using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(MissionCombatMechanicsHelper))]
    public class BetterPikesCombatMechanicsHelper
    {
        [HarmonyPostfix]
        [HarmonyPatch("DecideAgentDismountedByBlow")]
        public static void Postfix1(ref bool __result, WeaponComponentData attackerWeapon)
        {
            if (IsPike(attackerWeapon?.WeaponDescriptionId))
            {
                // Make pikes always dismount riders.
                __result = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("DecideMountRearedByBlow")]
        public static void Postfix2(ref bool __result, WeaponComponentData attackerWeapon)
        {
            if (IsPike(attackerWeapon?.WeaponDescriptionId))
            {
                // Make pikes always rear horses.
                __result = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("CalculateBaseMeleeBlowMagnitude")]
        public static void Postfix3(ref float __result, AttackInformation attackInformation, MissionWeapon weapon)
        {
            if (IsPike(weapon.CurrentUsageItem?.WeaponDescriptionId))
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

        private static bool IsPike(string weaponDescription) => weaponDescription != null && (weaponDescription.Contains("Pike") || weaponDescription.Contains("Bracing"));
    }
}
