using HarmonyLib;
using TaleWorlds.Core;

namespace BetterPikes
{
    [HarmonyPatch(typeof(WeaponComponentData), "Init")]
    public class BetterPikesComponentData
    {
        public static void Prefix(string itemUsage, ref float thrustDamageFactor, ref int thrustDamage)
        {
            if (itemUsage == "polearm_pike" || itemUsage == "polearm_bracing")
            {
                // Multiply the pike damage.
                thrustDamageFactor *= BetterPikesSettings.Instance.PikeDamageMultiplier;
                thrustDamage *= BetterPikesSettings.Instance.PikeDamageMultiplier;
            }
        }
    }
}
