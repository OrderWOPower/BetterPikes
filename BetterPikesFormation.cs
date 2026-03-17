using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesFormation
	{
		public static void Prefix1(Formation __instance, ref MovementOrder input)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled)
			{
				input = MovementOrder.MovementOrderAdvance;
			}
		}

		public static void Prefix2(Formation __instance, ref ArrangementOrder order)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled && order != ArrangementOrder.ArrangementOrderCircle)
			{
				order = ArrangementOrder.ArrangementOrderShieldWall;
			}
		}
	}
}
