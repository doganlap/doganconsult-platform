namespace DoganConsult.Workspace.Features;

public static class DgFeatures
{
    public const string GroupName = "DG";

    public static class Modules
    {
        public const string Sbg = "DG.Modules.SBG";
        public const string ShahinGrc = "DG.Modules.ShahinGrc";
        public const string NextErp = "DG.Modules.NextERP";
    }

    public static class SubFeatures
    {
        public static class ShahinGrc
        {
            public const string Risk = "DG.Modules.ShahinGrc.Risk";
            public const string Controls = "DG.Modules.ShahinGrc.Controls";
        }

        public static class Sbg
        {
            public const string Procurement = "DG.Modules.SBG.Procurement";
            public const string Contracts = "DG.Modules.SBG.Contracts";
        }
    }
}
