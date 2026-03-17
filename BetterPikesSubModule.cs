using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	// This mod gives pikes realistic lengths, fixes the pike bracing animation so that the longer pikes can hit enemy cavalry, and makes pikemen use pike formations that are as functional as the game allows.
	public class BetterPikesSubModule : MBSubModuleBase
	{
		private Harmony _harmony;

		protected override void OnSubModuleLoad()
		{
			_harmony = new Harmony("mod.bannerlord.betterpikes");
			_harmony.PatchAll();
		}

		public override void OnBeforeMissionBehaviorInitialize(Mission mission)
		{
			mission.AddMissionBehavior(new BetterPikesMissionBehavior());

			_harmony.Unpatch(AccessTools.Method(typeof(Formation), "SetMovementOrder"), AccessTools.Method(typeof(BetterPikesFormation), "Prefix1"));
			_harmony.Unpatch(AccessTools.Method(typeof(Formation), "SetArrangementOrder"), AccessTools.Method(typeof(BetterPikesFormation), "Prefix2"));
			_harmony.Patch(AccessTools.Method(typeof(Formation), "SetMovementOrder"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesFormation), "Prefix1"), after: new string[] { "com.rbmai" }));
			_harmony.Patch(AccessTools.Method(typeof(Formation), "SetArrangementOrder"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesFormation), "Prefix2")));
		}
	}
}
