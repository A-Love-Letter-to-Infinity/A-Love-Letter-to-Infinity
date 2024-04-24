using System.Numerics;
using Content.Client.Items;
using Content.Client.Resources;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory.VirtualItem;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using static Content.Client.IoC.StaticIoC;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

[GenerateTypedNameReferences]
public sealed partial class ItemStatusPanel : Control
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [ViewVariables] private EntityUid? _entity;

    // Tracked so we can re-run SetSide() if the theme changes.
    private HandLocation _side;

    public ItemStatusPanel()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        SetSide(HandLocation.Middle);
    }

    public void SetSide(HandLocation location)
    {
        // AN IMPORTANT REMINDER ABOUT THIS CODE:
        // In the UI, the RIGHT hand is on the LEFT on the screen.
        // So that a character facing DOWN matches the hand positions.

        Texture? texture;
        Texture? textureHighlight;
        StyleBox.Margin cutOut;
        StyleBox.Margin flat;
        Thickness contentMargin;

        switch (location)
        {
            case HandLocation.Right:
                texture = Theme.ResolveTexture("item_status_right");
                textureHighlight = Theme.ResolveTexture("item_status_right_highlight");
                cutOut = StyleBox.Margin.Left;
                flat = StyleBox.Margin.Right;
                contentMargin = MarginFromThemeColor("_itemstatus_content_margin_right");
                break;
            case HandLocation.Middle:
            case HandLocation.Left:
                texture = Theme.ResolveTexture("item_status_left");
                textureHighlight = Theme.ResolveTexture("item_status_left_highlight");
                cutOut = StyleBox.Margin.Right;
                flat = StyleBox.Margin.Left;
                contentMargin = MarginFromThemeColor("_itemstatus_content_margin_left");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }

        Contents.Margin = contentMargin;

        var panel = (StyleBoxTexture) Panel.PanelOverride!;
        panel.Texture = texture;
        panel.SetPatchMargin(flat, 4);
        panel.SetPatchMargin(cutOut, 7);

        var panelHighlight = (StyleBoxTexture) HighlightPanel.PanelOverride!;
        panelHighlight.Texture = textureHighlight;
        panelHighlight.SetPatchMargin(flat, 4);
        panelHighlight.SetPatchMargin(cutOut, 7);

        _side = location;
    }

    private Thickness MarginFromThemeColor(string itemName)
    {
        // This is the worst thing I've ever programmed
        // (can you tell I'm a graphics programmer?)
        // (the margin needs to change depending on the UI theme, so we use a fake color entry to store the value)

        var color = Theme.ResolveColorOrSpecified(itemName);
        return new Thickness(color.RByte, color.GByte, color.BByte, color.AByte);
    }

    protected override void OnThemeUpdated()
    {
        SetSide(_side);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        UpdateItemName();
    }

    public void Update(EntityUid? entity)
    {
        if (entity == null)
        {
            ClearOldStatus();
            _entity = null;
            VisWrapper.Visible = false;
            return;
        }

        if (entity != _entity)
        {
            _entity = entity.Value;
            BuildNewEntityStatus();

            UpdateItemName();
        }

        VisWrapper.Visible = true;
    }

    public void UpdateHighlight(bool highlight)
    {
        HighlightPanel.Visible = highlight;
    }

    private void UpdateItemName()
    {
        if (_entity == null)
            return;

        if (!_entityManager.TryGetComponent<MetaDataComponent>(_entity, out var meta) || meta.Deleted)
        {
            Update(null);
            return;
        }

        if (_entityManager.TryGetComponent(_entity, out VirtualItemComponent? virtualItem)
            && _entityManager.EntityExists(virtualItem.BlockingEntity))
        {
            // Uses identity because we can be blocked by pulling someone
            ItemNameLabel.Text = Identity.Name(virtualItem.BlockingEntity, _entityManager);
        }
        else
        {
            ItemNameLabel.Text = Identity.Name(_entity.Value, _entityManager);
        }
    }

    private void ClearOldStatus()
    {
        StatusContents.RemoveAllChildren();
    }

    private void BuildNewEntityStatus()
    {
        DebugTools.AssertNotNull(_entity);

        ClearOldStatus();

        var collectMsg = new ItemStatusCollectMessage();
        _entityManager.EventBus.RaiseLocalEvent(_entity!.Value, collectMsg, true);

        foreach (var control in collectMsg.Controls)
        {
            StatusContents.AddChild(control);
        }
    }
}
