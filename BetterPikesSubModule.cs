using System.Reflection;
using BetterPikes.InteractionWithOtherMods;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	// This mod gives pikes realistic lengths, fixes the pike bracing animation so that the longer pikes can hit enemy cavalry, and makes pikemen use pike formations that are as functional as the game allows.
	public class BetterPikesSubModule : MBSubModuleBase
	{
		private Harmony _harmony;
		private MethodBase _panicFromChargeMethod, _canAttemptMethod;

		protected override void OnSubModuleLoad()
		{
			_harmony = new Harmony("mod.bannerlord.betterpikes");
			_harmony.PatchAll();
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			_panicFromChargeMethod = AccessTools.Method(AccessTools.Inner(AccessTools.TypeByName("RBMAI.AgentAi"), "ChargeDamageCallbackPatch"), "PanicFromCharge");
			_canAttemptMethod = AccessTools.Method(AccessTools.Inner(AccessTools.TypeByName("RBMStance.KickBash"), "AiKickBashComponent"), "CanAttempt");

			// Check whether RBM is loaded.
			if (_panicFromChargeMethod != null)
			{
				_harmony.Patch(_panicFromChargeMethod, prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesRbmInteraction), "Prefix")));
			}

			if (_canAttemptMethod != null)
			{
				_harmony.Patch(_canAttemptMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix")));
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
			if (_panicFromChargeMethod != null)
			{
				_harmony.Unpatch(_panicFromChargeMethod, AccessTools.Method(typeof(BetterPikesRbmInteraction), "Prefix"));
			}

			if (_canAttemptMethod != null)
			{
				_harmony.Unpatch(_canAttemptMethod, AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix"));
			}
		}
	}
}
