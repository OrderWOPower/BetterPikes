using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesPostureLogic
    {
        private static void Postfix(MethodBase __originalMethod, ref float __result, Agent defenderAgent, Agent attackerAgent)
        {
            if ((__originalMethod.Name == "calculateDefenderPostureDamage" && BetterPikesHelper.IsPike(defenderAgent.WieldedWeapon)) || (__originalMethod.Name == "calculateAttackerPostureDamage" && BetterPikesHelper.IsPike(attackerAgent.WieldedWeapon)))
            {
                // Half the drain rate of posture for pikemen.
                __result /= 2;
            }
        }
    }
}
