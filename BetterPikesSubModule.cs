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
		private MethodBase _panicFromChargeMethod, _attackSplitInfantryGetTacticWeightMethod, _defendSplitInfantryGetTacticWeightMethod, _canAttemptMethod, _shouldSprintMethod;

		protected override void OnSubModuleLoad()
		{
			_harmony = new Harmony("mod.bannerlord.betterpikes");
			_harmony.PatchAll();
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			_panicFromChargeMethod = AccessTools.Method(AccessTools.Inner(AccessTools.TypeByName("RBMAI.AgentAi"), "ChargeDamageCallbackPatch"), "PanicFromCharge");
			_attackSplitInfantryGetTacticWeightMethod = AccessTools.Method(AccessTools.TypeByName("RBMTacticAttackSplitInfantry"), "GetTacticWeight");
			_defendSplitInfantryGetTacticWeightMethod = AccessTools.Method(AccessTools.TypeByName("RBMTacticDefendSplitInfantry"), "GetTacticWeight");
			_canAttemptMethod = AccessTools.Method(AccessTools.Inner(AccessTools.TypeByName("RBMStance.KickBash"), "AiKickBashComponent"), "CanAttempt");
			_shouldSprintMethod = AccessTools.Method(AccessTools.TypeByName("SprintAgentComponent"), "ShouldSprint");

			// Check whether RBM is loaded.
			if (_panicFromChargeMethod != null && _attackSplitInfantryGetTacticWeightMethod != null && _defendSplitInfantryGetTacticWeightMethod != null)
			{
				_harmony.Patch(_panicFromChargeMethod, prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesRbmInteraction), "Prefix")));
				_harmony.Patch(_attackSplitInfantryGetTacticWeightMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix1")));
				_harmony.Patch(_defendSplitInfantryGetTacticWeightMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix1")));
			}

			if (_canAttemptMethod != null)
			{
				_harmony.Patch(_canAttemptMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix2")));
			}

			// Check whether Artem's Cinematic Charges is loaded.
			if (_shouldSprintMethod != null)
			{
				_harmony.Patch(_shouldSprintMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesCinematicChargesInteraction), "Postfix")));
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
			if (_panicFromChargeMethod != null && _attackSplitInfantryGetTacticWeightMethod != null && _defendSplitInfantryGetTacticWeightMethod != null)
			{
				_harmony.Unpatch(_panicFromChargeMethod, AccessTools.Method(typeof(BetterPikesRbmInteraction), "Prefix"));
				_harmony.Unpatch(_attackSplitInfantryGetTacticWeightMethod, AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix1"));
				_harmony.Unpatch(_defendSplitInfantryGetTacticWeightMethod, AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix1"));
			}

			if (_canAttemptMethod != null)
			{
				_harmony.Unpatch(_canAttemptMethod, AccessTools.Method(typeof(BetterPikesRbmInteraction), "Postfix2"));
			}

			if (_shouldSprintMethod != null)
			{
				_harmony.Unpatch(_shouldSprintMethod, AccessTools.Method(typeof(BetterPikesCinematicChargesInteraction), "Postfix"));
			}
		}
	}
}
