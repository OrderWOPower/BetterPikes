using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace BetterPikes
{
	public class BetterPikesSettings : AttributeGlobalSettings<BetterPikesSettings>
	{
		public override string Id => "BetterPikes";

		public override string DisplayName => "Better Pikes";

		public override string FolderName => "BetterPikes";

		public override string FormatType => "json2";

		[SettingPropertyInteger("{=BetterPikes05}Pike Blow Magnitude", 1, 100, "0", Order = 0, RequireRestart = false, HintText = "{=BetterPikes06}Multiplier for blow magnitude of pikes. Default is 2.")]
		[SettingPropertyGroup("{=BetterPikes01}Multipliers", GroupOrder = 0)]
		public int PikeBlowMagnitudeMultiplier { get; set; } = 2;

		[SettingPropertyFloatingInteger("{=BetterPikes07}Minimum Pikemen in Pike Formation", 0.5f, 1.0f, "#0%", Order = 0, RequireRestart = false, HintText = "{=BetterPikes08}Minimum percentage of pikemen in a formation to be treated as a pike formation. Default is 50%.")]
		[SettingPropertyGroup("{=BetterPikes02}Limits", GroupOrder = 1)]
		public float MinPikemenPercentInPikeFormation { get; set; } = 0.5f;

		[SettingPropertyInteger("{=BetterPikes09}Maximum Distance to Ready Pikes", 0, 1000, "0m", Order = 1, RequireRestart = false, HintText = "{=BetterPikes10}Maximum distance to nearby enemies for pikemen to ready their pikes. Default is 50m.")]
		[SettingPropertyGroup("{=BetterPikes02}Limits", GroupOrder = 1)]
		public int MaxDistanceToReadyPikes { get; set; } = 50;

		[SettingPropertyBool("{=BetterPikes11}Pikemen Can Block", Order = 0, RequireRestart = false, HintText = "{=BetterPikes12}Pikemen can block with their pikes (unrealistic). Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes03}Combat", GroupOrder = 2)]
		public bool CanPikemenBlock { get; set; } = false;

		[SettingPropertyBool("{=BetterPikes13}Pikemen Can Attack Overhead", Order = 1, RequireRestart = false, HintText = "{=BetterPikes14}Pikemen can perform overhead attacks (unrealistic). Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes03}Combat", GroupOrder = 2)]
		public bool CanPikemenAttackUp { get; set; } = false;

		[SettingPropertyBool("{=BetterPikes15}Pikemen Can Turn Sideways", Order = 2, RequireRestart = false, HintText = "{=BetterPikes16}Pikemen can turn sideways when in pike formation. Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes03}Combat", GroupOrder = 2)]
		public bool CanPikemenTurnSideways { get; set; } = false;

		[SettingPropertyBool("{=BetterPikes17}Pikes Have Collision", Order = 3, RequireRestart = false, HintText = "{=BetterPikes18}Pikes can obstruct enemies who try to get past the pike head. Enabled by default.")]
		[SettingPropertyGroup("{=BetterPikes03}Combat", GroupOrder = 2)]
		public bool DoPikesHaveCollision { get; set; } = true;

		[SettingPropertyBool("{=BetterPikes19}Pikemen Can Panic from Cavalry Charges", Order = 0, RequireRestart = false, HintText = "{=BetterPikes20}Pikemen can panic from cavalry charges (a feature from RBM). Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes04}Other Mods", GroupOrder = 3)]
		public bool CanPikemenPanicFromCharge { get; set; } = false;

		[SettingPropertyBool("{=BetterPikes21}Pikemen Can Kick and Bash", Order = 1, RequireRestart = false, HintText = "{=BetterPikes22}Pikemen can kick and bash (a feature from RBM). Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes04}Other Mods", GroupOrder = 3)]
		public bool CanPikemenKickBash { get; set; } = false;

		[SettingPropertyBool("{=BetterPikes23}Pikemen Can Perform Cinematic Combat Actions", Order = 2, RequireRestart = false, HintText = "{=BetterPikes24}Pikemen can perform matched combat actions, kill moves and master strikes (a feature from Artem's Cinematic Combat). Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes04}Other Mods", GroupOrder = 3)]
		public bool CanPikemenPerformCinematicCombat { get; set; } = false;

		[SettingPropertyBool("{=BetterPikes25}Pikemen Can Perform Cinematic Charges", Order = 3, RequireRestart = false, HintText = "{=BetterPikes26}Pikemen can perform cinematic charges (a feature from Artem's Cinematic Charges). Disabled by default.")]
		[SettingPropertyGroup("{=BetterPikes04}Other Mods", GroupOrder = 3)]
		public bool CanPikemenPerformCinematicCharges { get; set; } = false;
	}
}
