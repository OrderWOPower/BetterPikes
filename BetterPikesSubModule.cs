using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	// This mod gives pikes realistic lengths, fixes the pike bracing animation so that the longer pikes can hit enemy cavalry, and makes pikemen use pike formations that are as functional as the game allows.
	public class BetterPikesSubModule : MBSubModuleBase
	{
		private Harmony _harmony;
		private Type _typeofAgentAi, _typeofCinematicCombatMissionLogic;

		protected override void OnSubModuleLoad()
		{
			_harmony = new Harmony("mod.bannerlord.betterpikes");
			_harmony.PatchAll();
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			_typeofAgentAi = AccessTools.TypeByName("RBMAI.AgentAi");
			_typeofCinematicCombatMissionLogic = AccessTools.TypeByName("CinematicCombatMissionLogic");

			// Check whether RBM is loaded.
			if (_typeofAgentAi != null)
			{
				_harmony.Patch(AccessTools.Method(AccessTools.Inner(_typeofAgentAi, "ChargeDamageCallbackPatch"), "PanicFromCharge"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesAgentAi), "Prefix")));
			}

			// Check whether Artem's Cinematic Combat is loaded.
			if (_typeofCinematicCombatMissionLogic != null)
			{
				_harmony.Patch(AccessTools.Method(_typeofCinematicCombatMissionLogic, "OnAgentHit"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix1")));
				_harmony.Patch(AccessTools.Method(_typeofCinematicCombatMissionLogic, "StartKillmoveLogic"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix2")));
				_harmony.Patch(AccessTools.Method(_typeofCinematicCombatMissionLogic, "StartMatchedCombatLogic"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix2")));
				_harmony.Patch(AccessTools.Method(AccessTools.TypeByName("CinematicCombatMasterstrikeLogic"), "CinematicCombatMasterStrikePlayerLogic"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix2")));
			}
		}

		public override void OnBeforeMissionBehaviorInitialize(Mission mission)
		{
			mission.AddMissionBehavior(new BetterPikesMissionBehavior());

			_harmony.Unpatch(AccessTools.Method(typeof(Formation), "SetMovementOrder"), AccessTools.Method(typeof(BetterPikesFormation), "Prefix1"));
			_harmony.Patch(AccessTools.Method(typeof(Formation), "SetMovementOrder"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesFormation), "Prefix1"), after: new string[] { "com.rbmai" }));
		}

		public override void OnGameEnd(Game game)
		{
			if (_typeofAgentAi != null)
			{
				_harmony.Unpatch(AccessTools.Method(AccessTools.Inner(_typeofAgentAi, "ChargeDamageCallbackPatch"), "PanicFromCharge"), AccessTools.Method(typeof(BetterPikesAgentAi), "Prefix"));
			}

			if (_typeofCinematicCombatMissionLogic != null)
			{
				_harmony.Unpatch(AccessTools.Method(_typeofCinematicCombatMissionLogic, "OnAgentHit"), AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix1"));
				_harmony.Unpatch(AccessTools.Method(_typeofCinematicCombatMissionLogic, "StartKillmoveLogic"), AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix2"));
				_harmony.Unpatch(AccessTools.Method(_typeofCinematicCombatMissionLogic, "StartMatchedCombatLogic"), AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix2"));
				_harmony.Unpatch(AccessTools.Method(AccessTools.TypeByName("CinematicCombatMasterstrikeLogic"), "CinematicCombatMasterStrikePlayerLogic"), AccessTools.Method(typeof(BetterPikesCinematicCombat), "Prefix2"));
			}
		}
	}
}
