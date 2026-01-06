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

        [SettingPropertyInteger("{=BetterPikes04}Pike Blow Magnitude", 1, 100, "0", Order = 0, RequireRestart = false, HintText = "{=BetterPikes05}Multiplier for blow magnitude of pikes. Default is 3.")]
        [SettingPropertyGroup("{=BetterPikes01}Multipliers", GroupOrder = 0)]
        public int PikeBlowMagnitudeMultiplier { get; set; } = 3;

        [SettingPropertyInteger("{=BetterPikes06}Pike Knockback Magnitude", 0, 10000, "0", Order = 1, RequireRestart = false, HintText = "{=BetterPikes07}Knockback magnitude of pikes. Default is 200.")]
        [SettingPropertyGroup("{=BetterPikes01}Multipliers", GroupOrder = 0)]
        public int PikeKnockbackMagnitude { get; set; } = 200;

        [SettingPropertyFloatingInteger("{=BetterPikes08}Minimum Pikemen in Pike Formation", 0.5f, 1.0f, "#0%", Order = 0, RequireRestart = false, HintText = "{=BetterPikes09}Minimum percentage of pikemen in a formation to be treated as a pike formation. Default is 50%.")]
        [SettingPropertyGroup("{=BetterPikes02}Limits", GroupOrder = 1)]
        public float MinPikemenPercentInPikeFormation { get; set; } = 0.5f;

        [SettingPropertyInteger("{=BetterPikes10}Maximum Distance to Ready Pikes", 0, 1000, "0m", Order = 1, RequireRestart = false, HintText = "{=BetterPikes11}Maximum distance to nearby enemies for pikemen to ready their pikes. Default is 50m.")]
        [SettingPropertyGroup("{=BetterPikes02}Limits", GroupOrder = 1)]
        public int MaxDistanceToReadyPikes { get; set; } = 50;

        [SettingPropertyBool("{=BetterPikes12}Pikemen Can Block", Order = 0, RequireRestart = false, HintText = "{=BetterPikes13}Pikemen can block with their pikes (unrealistic). Disabled by default.")]
        [SettingPropertyGroup("{=BetterPikes03}Combat", GroupOrder = 2)]
        public bool CanPikemenBlock { get; set; } = false;

        [SettingPropertyBool("{=BetterPikes14}Pikemen Can Attack Overhead", Order = 1, RequireRestart = false, HintText = "{=BetterPikes15}Pikemen can perform overhead attacks (unrealistic). Disabled by default.")]
        [SettingPropertyGroup("{=BetterPikes03}Combat", GroupOrder = 2)]
        public bool CanPikemenAttackUp { get; set; } = false;
    }
}
