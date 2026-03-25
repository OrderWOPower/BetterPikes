using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(BehaviorComponent), "OnBehaviorActivated")]
	public class BetterPikesBehaviorComponent
	{
		internal static void Postfix(BehaviorComponent __instance)
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
	}
}
