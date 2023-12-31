using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace HighSellPrice
{
    public class HighSellPriceSettings : AttributeGlobalSettings<HighSellPriceSettings>
    {
        public override string Id => "HighSellPrice";

        public override string DisplayName => "High Sell Price Alert";

        public override string FolderName => "HighSellPrice";

        public override string FormatType => "json2";

        [SettingPropertyBool("{=HSPOpt001}Toggle Food", Order = 0, RequireRestart = false, HintText = "{=HSPOpt001Hint}Count food items. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=HSPOptG001}Food", GroupOrder = 0)]
        public bool ShouldCountFood { get; set; } = true;

        [SettingPropertyInteger("{=HSPOpt002}Minimum Food Count", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "{=HSPOpt002Hint}Minimum number of food items to trigger the alert. Default is 2.")]
        [SettingPropertyGroup("{=HSPOptG001}Food", GroupOrder = 0)]
        public int MinFoodCount { get; set; } = 2;

        [SettingPropertyBool("{=HSPOpt003}Toggle Craftables", Order = 0, RequireRestart = false, HintText = "{=HSPOpt003Hint}Count craftable items. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=HSPOptG002}Craftables", GroupOrder = 1)]
        public bool ShouldCountCraftables { get; set; } = true;

        [SettingPropertyInteger("{=HSPOpt004}Minimum Craftable Count", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "{=HSPOpt004Hint}Minimum number of craftable items to trigger the alert. Default is 2.")]
        [SettingPropertyGroup("{=HSPOptG002}Craftables", GroupOrder = 1)]
        public int MinCraftableCount { get; set; } = 2;

        [SettingPropertyBool("{=HSPOpt005}Toggle Others", Order = 0, RequireRestart = false, HintText = "{=HSPOpt005Hint}Count other items. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=HSPOptG003}Others", GroupOrder = 2)]
        public bool ShouldCountOthers { get; set; } = true;

        [SettingPropertyInteger("{=HSPOpt006}Minimum Other Count", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "{=HSPOpt006Hint}Minimum number of other items to trigger the alert. Default is 1.")]
        [SettingPropertyGroup("{=HSPOptG003}Others", GroupOrder = 2)]
        public int MinOtherCount { get; set; } = 1;
    }
}
