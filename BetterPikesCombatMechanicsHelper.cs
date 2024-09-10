using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "CalculateBaseMeleeBlowMagnitude")]
    public class BetterPikesCombatMechanicsHelper
    {
        public static void Postfix(ref float __result, AttackInformation attackInformation, MissionWeapon weapon)
        {
            string weaponDescription = weapon.CurrentUsageItem?.WeaponDescriptionId;
            Agent victim = attackInformation.VictimAgent;

            if (weaponDescription != null && (weaponDescription.Contains("Pike") || weaponDescription.Contains("Bracing")))
            {
                // Multiply the base blow magnitude of pikes.
                __result *= BetterPikesSettings.Instance.PikeBlowMagnitudeMultiplier;

                if (victim != null && (victim.HasMount || victim.IsMount))
                {
                    // Further multiply the base blow magnitude of pikes versus cavalry.
                    __result *= 10;
                }
            }
        }
    }
}
