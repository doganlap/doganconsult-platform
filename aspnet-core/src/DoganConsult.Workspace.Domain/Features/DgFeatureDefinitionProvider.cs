using Volo.Abp.Features;
using DoganConsult.Workspace.Features;

namespace DoganConsult.Workspace.Features;

public class DgFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(DgFeatures.GroupName);

        // Module features
        group.AddFeature(DgFeatures.Modules.Sbg, defaultValue: "false");
        group.AddFeature(DgFeatures.Modules.ShahinGrc, defaultValue: "false");
        group.AddFeature(DgFeatures.Modules.NextErp, defaultValue: "false");

        // Sub-features for ShahinGrc
        group.AddFeature(DgFeatures.SubFeatures.ShahinGrc.Risk, defaultValue: "false");
        group.AddFeature(DgFeatures.SubFeatures.ShahinGrc.Controls, defaultValue: "false");

        // Sub-features for SBG
        group.AddFeature(DgFeatures.SubFeatures.Sbg.Procurement, defaultValue: "false");
        group.AddFeature(DgFeatures.SubFeatures.Sbg.Contracts, defaultValue: "false");
    }
}
