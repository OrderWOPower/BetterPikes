﻿using HarmonyLib;
using TaleWorlds.Core;

namespace BetterPikes
{
    [HarmonyPatch(typeof(WeaponComponentData), "Init")]
    public class BetterPikesComponentData
    {
        // Multiply the pike damage.
        public static void Prefix(int weaponLength, ref float thrustDamageFactor, ref int thrustDamage)
        {
            if (weaponLength >= 400)
            {
                BetterPikesSettings settings = BetterPikesSettings.Instance;
                thrustDamageFactor *= settings.PikeDamageMultiplier;
                thrustDamage *= settings.PikeDamageMultiplier;
            }
        }
    }
}
