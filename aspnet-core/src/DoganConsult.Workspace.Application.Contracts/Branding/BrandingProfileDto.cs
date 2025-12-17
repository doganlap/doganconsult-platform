namespace DoganConsult.Workspace.Branding;

public class BrandingProfileDto
{
    public string AppDisplayName { get; set; } = "DG.OS";
    public string? ProductName { get; set; }
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }

    public string PrimaryColor { get; set; } = "#0ea5a4";
    public string AccentColor { get; set; } = "#22c55e";

    public string DefaultLanguage { get; set; } = "en";
    public bool IsRtl { get; set; }
    public string HomeRoute { get; set; } = "/";
}
