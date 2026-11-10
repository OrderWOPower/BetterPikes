using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(ArrangementOrder), "OnApply")]
	public class BetterPikesArrangementOrder
	{
		public static void Prefix(ArrangementOrder __instance, ref int ____unitSpacing, Formation formation)
		{
			if (BetterPikesHelper.IsPikeFormation(formation) && __instance == ArrangementOrder.ArrangementOrderCircle)
			{
				// If the pikemen are in circle formation, make the circle as tight as possible.
				____unitSpacing = 0;
			}
		}
	}
}
