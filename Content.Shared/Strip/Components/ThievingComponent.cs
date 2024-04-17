namespace Content.Shared.Strip.Components;

/// <summary>
/// Give this to an entity when you want to decrease stripping times
/// </summary>
[RegisterComponent]
public sealed partial class ThievingComponent : Component
{
    /// <summary>
    /// How much the strip time should be shortened by
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("stripTimeReduction")]
    public TimeSpan StripTimeReduction = TimeSpan.FromSeconds(0.5f);

    [ViewVariables(VVAccess.ReadWrite), DataField("stripTimeMultiplier")]
    public float StripTimeMultiplier = 1f;

    /// <summary>
    /// Should it notify the user if they're stripping a pocket?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("stealthy")]
    public bool Stealthy;

    /// <summary>
    ///  Should the user be able to see hidden items? (i.e pockets)
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("ignoreStripHidden")]
    public bool IgnoreStripHidden = false;
}
