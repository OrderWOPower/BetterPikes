using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(BehaviorCharge))]
	public class BetterPikesBehaviorCharge
	{
		[HarmonyPostfix]
		[HarmonyPatch("OnBehaviorActivatedAux")]
		protected static void Postfix1(BehaviorCharge __instance)
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

		[HarmonyPostfix]
		[HarmonyPatch("TickOccasionally")]
		public static void Postfix2(BehaviorCharge __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				if (formation.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
				{
					formation.SetFormOrder(FormOrder.FormOrderDeep);
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
		}
	}
}
