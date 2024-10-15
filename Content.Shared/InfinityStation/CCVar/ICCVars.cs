using Robust.Shared.Configuration;

namespace Content.Shared.InfinityStation.CCVar;

/// <summary>
/// ALLTI specific cvars.
/// </summary>
[CVarDefs]
public sealed class ICCVars
{
    /// <summary>
    /// Define if the round end no EORG message should be displayed.
    /// </summary>
    public static readonly CVarDef<bool> SkipRoundEndNoEorgMessage =
        CVarDef.Create("infinity.skip_roundend_noeorg", false, CVar.CLIENTONLY | CVar.ARCHIVE);
}
