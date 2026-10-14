using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.InteractionWithOtherMods
{
	public class BetterPikesCinematicCombatInteraction
	{
		[HarmonyPatch]
		public class BetterPikesCinematicCombatInteractionNonPlayer
		{
			private static MethodBase TargetMethod() => AccessTools.Method(AccessTools.TypeByName("CinematicCombatMissionLogic"), "OnAgentHit");

			// Check whether Artem's Cinematic Combat is loaded.
			private static bool Prepare() => TargetMethod() != null;

			public static bool Prefix(MissionWeapon affectorWeapon)
			{
				if (!BetterPikesSettings.Instance.CanPikemenPerformCinematicCombat && BetterPikesHelper.IsPike(affectorWeapon))
				{
					// Disable cinematic combat for pikemen.
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch]
		public class BetterPikesCinematicCombatInteractionPlayer
		{
			private static IEnumerable<MethodBase> TargetMethods()
			{
				yield return AccessTools.Constructor(AccessTools.TypeByName("CCKillmoveDataPlayer"), new Type[] { typeof(object), typeof(object) });
				yield return AccessTools.Constructor(AccessTools.TypeByName("CCMatchedCombatDataPlayer"), new Type[] { typeof(object), typeof(object) });
				yield return AccessTools.Method(AccessTools.TypeByName("CinematicCombatMasterstrikeLogic"), "CinematicCombatMasterStrikePlayerLogic");
			}

			// Check whether Artem's Cinematic Combat is loaded.
			private static bool Prepare() => TargetMethods().All(method => method != null);

			public static bool Prefix(Agent affectedAgent)
			{
				if (!BetterPikesSettings.Instance.CanPikemenPerformCinematicCombat && BetterPikesHelper.IsWieldingPike(affectedAgent))
				{
					// Disable cinematic combat for pikemen.
					return false;
				}

				return true;
			}
		}
	}
}
