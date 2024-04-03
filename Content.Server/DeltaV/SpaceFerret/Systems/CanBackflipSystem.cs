﻿using Content.Server.Actions;
using Content.Shared.DeltaV.SpaceFerret;
using Content.Shared.DeltaV.SpaceFerret.Events;

namespace Content.Server.DeltaV.SpaceFerret.Systems;

public sealed class CanBackflipSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CanBackflipComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<CanBackflipComponent, BackflipActionEvent>(OnBackflipAction);
    }

    private void OnInit(EntityUid uid, CanBackflipComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.BackflipActionEntity, component.BackflipAction, uid);
    }

    public void OnBackflipAction(EntityUid uid, CanBackflipComponent comp, BackflipActionEvent args)
    {
        RaiseNetworkEvent(new DoABackFlipEvent(GetNetEntity(uid)));

        args.Handled = true;
    }
}
