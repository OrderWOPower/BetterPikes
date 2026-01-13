using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorCharge))]
    public class BetterPikesBehaviorCharge
    {
        [HarmonyPatch("OnBehaviorActivatedAux")]
        protected static void Postfix(BehaviorCharge __instance)
        {
            Formation formation = __instance.Formation;

            if (BetterPikesHelper.IsPikeFormation(formation))
            {
                formation.ApplyActionOnEachUnit(delegate (Agent agent)
                {
                    agent.ClearTargetFrame();
                });
            }
        }

        [HarmonyPatch("TickOccasionally")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            int index = 0;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].operand is MethodInfo method && method == AccessTools.Method(typeof(Formation), "SetMovementOrder"))
                {
                    index = i;
                }
            }

            // Remove the vanilla movement order.
            codes.RemoveAt(index);
            codes.Insert(index, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BetterPikesBehaviorCharge), "FormUpPikemen", new Type[] { typeof(Formation), typeof(MovementOrder) })));

            return codes;
        }

        private static void FormUpPikemen(Formation formation, MovementOrder currentOrder)
        {
            if (BetterPikesHelper.IsPikeFormation(formation))
            {
                if (formation.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
                {
                    // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                    formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
                    formation.SetFormOrder(FormOrder.FormOrderDeep);
                    currentOrder = MovementOrder.MovementOrderAdvance;
                }
                else
                {
                    int num = (int)MathF.Sqrt(formation.CountOfUnits), i = formation.Arrangement.UnitCount, num3 = 0;
                    float num2 = ((num * formation.UnitDiameter) + ((num - 1) * formation.Interval)) * 0.5f * 1.414213f, num4;

                    while (i > 0)
                    {
                        double a = (double)(num2 + (formation.Distance * num3) + (formation.UnitDiameter * (num3 + 1))) * 3.141592653589793 * 2.0 / (double)(formation.UnitDiameter + formation.Interval);

                        i -= MathF.Ceiling(a);
                        num3++;
                    }

                    num4 = num2 + (num3 * formation.UnitDiameter) + ((num3 - 1) * formation.Distance);
                    formation.SetFormOrder(FormOrder.FormOrderCustom(num4 * 2f));
                }
            }

            formation.SetMovementOrder(currentOrder);
        }
    }
}
