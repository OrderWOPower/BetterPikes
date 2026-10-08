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
		private Type _typeofAgentAi;

		protected override void OnSubModuleLoad()
		{
			_harmony = new Harmony("mod.bannerlord.betterpikes");
			_harmony.PatchAll();
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			_typeofAgentAi = AccessTools.TypeByName("RBMAI.AgentAi");

			// Check whether RBM is loaded.
			if (_typeofAgentAi != null)
			{
				_harmony.Patch(AccessTools.Method(AccessTools.Inner(_typeofAgentAi, "ChargeDamageCallbackPatch"), "PanicFromCharge"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesAgentAi), "Prefix")));
				_harmony.Patch(AccessTools.Method(AccessTools.Inner(_typeofAgentAi, "AiKickBashComponent"), "CanAttempt"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesAgentAi), "Postfix")));
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
				_harmony.Unpatch(AccessTools.Method(AccessTools.Inner(_typeofAgentAi, "AiKickBashComponent"), "CanAttempt"), AccessTools.Method(typeof(BetterPikesAgentAi), "Postfix"));
			}
		}
	}
}
