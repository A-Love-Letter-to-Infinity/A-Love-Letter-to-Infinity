﻿using Content.Shared.DeltaV.SpaceFerret;
using Content.Shared.DeltaV.SpaceFerret.Events;
using Robust.Client.Animations;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Client.DeltaV.SpaceFerret;

public sealed class CanBackflipSystem : EntitySystem
{
    [Dependency] private readonly AnimationPlayerSystem _animation = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    public const string BackflipKey = "backflip";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<DoABackFlipEvent>(OnBackflipEvent);
    }

    public void OnBackflipEvent(DoABackFlipEvent args)
    {
        if (!TryGetEntity(args.Actioner, out var uid) || !TryComp<CanBackflipComponent>(uid, out var comp))
        {
            return;
        }

        _animation.Play(uid.Value, new Animation
        {
            Length = TimeSpan.FromSeconds(0.66),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty()
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Rotation),
                    InterpolationMode = AnimationInterpolationMode.Linear,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(0), 0f),
                        new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(180), 0.33f),
                        new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(360), 0.33f),
                    }
                }
            }
        }, BackflipKey);

        _audio.PlayPvs(comp.ClappaSfx, uid.Value);
    }
}
