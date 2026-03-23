using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(BehaviorAdvance), "TickOccasionally")]
	public class BetterPikesBehaviorAdvance
	{
		public static void Postfix(BehaviorAdvance __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				float deviationOfPositions = formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents;
				Vec2 orderPosition = formation.OrderPosition;

				formation.ApplyActionOnEachUnit(delegate (Agent agent)
				{
					Vec2 currentGlobalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);

					if (deviationOfPositions >= 0.5f && agent.CanMoveDirectlyToPosition(currentGlobalPositionOfUnit) && agent.CanMoveDirectlyToPosition(orderPosition))
					{
						// Ensure that the pikemen maintain their formation.
						agent.SetTargetPosition(currentGlobalPositionOfUnit);
					}
					else
					{
						agent.ClearTargetFrame();
					}
				});
			}
		}
	}
}
