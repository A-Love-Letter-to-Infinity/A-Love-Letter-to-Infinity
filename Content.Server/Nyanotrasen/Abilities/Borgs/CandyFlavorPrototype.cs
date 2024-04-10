﻿using Content.Shared.Nutrition;
using Robust.Shared.Prototypes;

namespace Content.Server.Abilities.Borgs;

/// <summary>
/// Describes the color and flavor profile of lollipops and gumballs. Yummy!
/// </summary>
[Prototype("candyFlavor")]
public sealed partial class CandyFlavorPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The display name for this candy. Not localized.
    /// </summary>
    [DataField("name")] public string Name { get; private set; } = "";

    /// <summary>
    /// The color of the candy.
    /// </summary>
    [DataField("color")] public Color Color { get; private set; } = Color.White;

    /// <summary>
    /// How the candy tastes like.
    /// </summary>
    [DataField("flavors")]
    public HashSet<ProtoId<FlavorPrototype>> Flavors { get; private set; } = [];
}
