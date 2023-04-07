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

        [SettingPropertyInteger("Pike Damage", 1, 100, "0", Order = 0, RequireRestart = false, HintText = "Multiplier for pike damage. Default is 20.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 0)]
        public int PikeDamageMultiplier { get; set; } = 20;

        [SettingPropertyFloatingInteger("Minimum Pikemen in Pike Formation", 0.5f, 1.0f, "#0%", Order = 1, RequireRestart = false, HintText = "Minimum percentage of pikemen in a formation to be treated as a pike formation. Default is 50%.")]
        [SettingPropertyGroup("Limits", GroupOrder = 1)]
        public float MinPikemenPercentInPikeFormation { get; set; } = 0.5f;
    }
}
