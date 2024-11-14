using HarmonyLib;
using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorCharge), "TickOccasionally")]
    public class BetterPikesBehaviorCharge
    {
        public static void Postfix(BehaviorCharge __instance)
        {
            Formation formation = __instance.Formation;

            if (formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem != null && agent.WieldedWeapon.CurrentUsageItem.WeaponDescriptionId.Contains("Pike")) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                FormationQuerySystem closestEnemyQuerySystem = formation.QuerySystem.ClosestEnemyFormation;

                if (closestEnemyQuerySystem != null && !closestEnemyQuerySystem.IsCavalryFormation && !closestEnemyQuerySystem.IsRangedCavalryFormation)
                {
                    // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                    formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                    formation.FormOrder = FormOrder.FormOrderDeep;
                    formation.SetMovementOrder(MovementOrder.MovementOrderAdvance);
                }

                if (formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle)
                {
                    int num = (int)MathF.Sqrt(formation.CountOfUnits), i = formation.Arrangement.UnitCount, num3 = 0;
                    float num2 = ((num * formation.UnitDiameter) + ((num - 1) * formation.Interval)) * 0.5f * 1.414213f, num4;

                    while (i > 0)
                    {
                        double a = (double)(num2 + (formation.Distance * num3) + (formation.UnitDiameter * (num3 + 1))) * 3.141592653589793 * 2.0 / (double)(formation.UnitDiameter + formation.Interval);

                        i -= (int)Math.Ceiling(a);
                        num3++;
                    }

                    num4 = num2 + (num3 * formation.UnitDiameter) + ((num3 - 1) * formation.Distance);
                    formation.FormOrder = FormOrder.FormOrderCustom(num4 * 2f);
                }
            }
        }
    }
}
