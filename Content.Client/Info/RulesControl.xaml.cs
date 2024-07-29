﻿using Content.Client.Guidebook;
using Content.Client.Guidebook.RichText;
using Content.Client.UserInterface.Systems.Info;
using Content.Shared.Guidebook;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.Info;

[GenerateTypedNameReferences]
public sealed partial class RulesControl : BoxContainer, ILinkClickHandler
{
    [Dependency] private readonly DocumentParsingManager _parsingMan = default!;

    private string? _currentEntry;
    private readonly Stack<string> _priorEntries = new();

    public RulesControl()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        SetGuide();

        HomeButton.OnPressed += _ => SetGuide();

        BackButton.OnPressed += _ => SetGuide(_priorEntries.Pop(), false);
    }

    public void HandleClick(string link)
    {
        SetGuide(link);
    }

    private void SetGuide(ProtoId<GuideEntryPrototype>? entry = null, bool addToPrior = true)
    {
        var coreEntry = UserInterfaceManager.GetUIController<InfoUIController>().GetCoreRuleEntry();
        entry ??= coreEntry;

        Scroll.SetScrollValue(default);
        RulesContainer.Children.Clear();
        if (!_parsingMan.TryAddMarkup(RulesContainer, entry.Value))
            return;

        if (addToPrior && _currentEntry != null)
            _priorEntries.Push(_currentEntry);
        _currentEntry = entry.Value;

        HomeButton.Visible = entry.Value != coreEntry.Id;
        BackButton.Visible = _priorEntries.Count != 0 && _priorEntries.Peek() != entry.Value;
    }
}
