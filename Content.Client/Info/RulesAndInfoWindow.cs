using System.Numerics;
using Content.Client.DeltaV.UserInterface.Controls;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;

namespace Content.Client.Info
{
    public sealed partial class RulesAndInfoWindow : DefaultWindow // DeltaV - Converted to partial to inject custom tabs
    {
        [Dependency] private readonly IResourceCache _resourceManager = default!;
        [Dependency] private readonly RulesManager _rules = default!;

        public RulesAndInfoWindow()
        {
            MinWidth = 750; // DeltaV - Override width to increase base size
            MinWidth = 100; // DeltaV - Restore width to allow resizing

            IoCManager.InjectDependencies(this);

            Title = Loc.GetString("ui-info-title");

            var rootContainer = new TabContainer();

            var rulesList = new Info();
            var tutorialList = new Info();
            var sopList = new TabbedRules(); // DeltaV - Standard Operating Procedures

            rootContainer.AddChild(rulesList);
            rootContainer.AddChild(tutorialList);
            rootContainer.AddChild(sopList); // DeltaV - Standard Operating Procedures

            TabContainer.SetTabTitle(rulesList, Loc.GetString("ui-info-tab-rules"));
            TabContainer.SetTabTitle(tutorialList, Loc.GetString("ui-info-tab-tutorial"));
            TabContainer.SetTabTitle(sopList, Loc.GetString("ui-info-tab-sop")); // DeltaV - Standard Operating Procedures

            AddSection(rulesList, _rules.RulesSection());
            PopulateTutorial(tutorialList);
            PopulateSop(sopList); // DeltaV - Standard Operating Procedures

            Contents.AddChild(rootContainer);

            SetSize = new Vector2(650, 650);
        }

        private void PopulateTutorial(Info tutorialList)
        {
            AddSection(tutorialList, Loc.GetString("ui-info-header-intro"), "Intro.txt");
            var infoControlSection = new InfoControlsSection();
            tutorialList.InfoContainer.AddChild(infoControlSection);
            AddSection(tutorialList, Loc.GetString("ui-info-header-gameplay"), "Gameplay.txt", true);
            AddSection(tutorialList, Loc.GetString("ui-info-header-sandbox"), "Sandbox.txt", true);

            infoControlSection.ControlsButton.OnPressed += _ => UserInterfaceManager.GetUIController<OptionsUIController>().OpenWindow();
        }

        private static void AddSection(Info info, Control control)
        {
            info.InfoContainer.AddChild(control);
        }

        private void AddSection(Info info, string title, string path, bool markup = false)
        {
            AddSection(info, MakeSection(title, path, markup, _resourceManager));
        }

        private static Control MakeSection(string title, string path, bool markup, IResourceManager res)
        {
            return new InfoSection(title, res.ContentFileReadAllText($"/ServerInfo/{path}"), markup);
        }

    }
}
