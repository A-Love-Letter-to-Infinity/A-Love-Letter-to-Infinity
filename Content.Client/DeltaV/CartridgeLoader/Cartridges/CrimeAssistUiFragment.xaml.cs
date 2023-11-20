using System.Linq;
using System.Numerics;
using Content.Client.Nyanotrasen.UserInterface;
using Content.Shared.DeltaV.CartridgeLoader.Cartridges;
using Robust.Client.AutoGenerated;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.DeltaV.CartridgeLoader.Cartridges;

[GenerateTypedNameReferences]
public sealed partial class CrimeAssistUiFragment : BoxContainer
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;

    public event Action<bool>? OnSync;
    private CrimeAssistUiState.UiStates _currentState;

    public CrimeAssistUiFragment()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
        VerticalExpand = true;

        StartButton.OnPressed += _ => UpdateUI(CrimeAssistUiState.UiStates.IsItTerrorism);
        HomeButton.OnPressed += _ => UpdateUI(CrimeAssistUiState.UiStates.MainMenu);
        YesButton.OnPressed += _ => AdvanceState(_currentState, true);
        NoButton.OnPressed += _ => AdvanceState(_currentState, false);
    }

    public void AdvanceState(CrimeAssistUiState.UiStates currentState, bool yesPressed)
    {
        CrimeAssistUiState.UiStates newState = currentState switch
        {
            CrimeAssistUiState.UiStates.IsItTerrorism => yesPressed ? CrimeAssistUiState.UiStates.Result_Terrorism : CrimeAssistUiState.UiStates.WasSomeoneAttacked,

            //assault branch
            CrimeAssistUiState.UiStates.WasSomeoneAttacked => yesPressed ? CrimeAssistUiState.UiStates.WasItSophont : CrimeAssistUiState.UiStates.ForcedMindbreakerToxin,
            CrimeAssistUiState.UiStates.WasItSophont => yesPressed ? CrimeAssistUiState.UiStates.DidVictimDie : CrimeAssistUiState.UiStates.Result_AnimalCruelty,
            CrimeAssistUiState.UiStates.DidVictimDie => yesPressed ? CrimeAssistUiState.UiStates.IsVictimRemovedFromBody : CrimeAssistUiState.UiStates.Result_Assault,
            CrimeAssistUiState.UiStates.IsVictimRemovedFromBody => yesPressed ? CrimeAssistUiState.UiStates.Result_Decorporealisation : CrimeAssistUiState.UiStates.WasDeathIntentional,
            CrimeAssistUiState.UiStates.WasDeathIntentional => yesPressed ? CrimeAssistUiState.UiStates.Result_Murder : CrimeAssistUiState.UiStates.Result_Manslaughter,

            //mindbreaker branch
            CrimeAssistUiState.UiStates.ForcedMindbreakerToxin => yesPressed ? CrimeAssistUiState.UiStates.Result_Mindbreaking : CrimeAssistUiState.UiStates.HadIllegitimateItem,

            //theft branch
            CrimeAssistUiState.UiStates.HadIllegitimateItem => yesPressed ? CrimeAssistUiState.UiStates.WasItAPerson : CrimeAssistUiState.UiStates.WasSuspectInARestrictedLocation,
            CrimeAssistUiState.UiStates.WasItAPerson => yesPressed ? CrimeAssistUiState.UiStates.Result_Kidnapping : CrimeAssistUiState.UiStates.WasSuspectSelling,
            CrimeAssistUiState.UiStates.WasSuspectSelling => yesPressed ? CrimeAssistUiState.UiStates.Result_BlackMarketeering : CrimeAssistUiState.UiStates.WasSuspectSeenTaking,
            CrimeAssistUiState.UiStates.WasSuspectSeenTaking => yesPressed ? CrimeAssistUiState.UiStates.IsItemExtremelyDangerous : CrimeAssistUiState.UiStates.Result_Possession,
            CrimeAssistUiState.UiStates.IsItemExtremelyDangerous => yesPressed ? CrimeAssistUiState.UiStates.Result_GrandTheft : CrimeAssistUiState.UiStates.Result_Theft,

            //trespassing branch
            CrimeAssistUiState.UiStates.WasSuspectInARestrictedLocation => yesPressed ? CrimeAssistUiState.UiStates.WasEntranceLocked : CrimeAssistUiState.UiStates.DidSuspectBreakSomething,
            CrimeAssistUiState.UiStates.WasEntranceLocked => yesPressed ? CrimeAssistUiState.UiStates.Result_BreakingAndEntering : CrimeAssistUiState.UiStates.Result_Trespass,

            //vandalism branch
            CrimeAssistUiState.UiStates.DidSuspectBreakSomething => yesPressed ? CrimeAssistUiState.UiStates.WereThereManySuspects : CrimeAssistUiState.UiStates.WasCrimeSexualInNature,
            CrimeAssistUiState.UiStates.WereThereManySuspects => yesPressed ? CrimeAssistUiState.UiStates.Result_Rioting : CrimeAssistUiState.UiStates.WasDamageSmall,
            CrimeAssistUiState.UiStates.WasDamageSmall => yesPressed ? CrimeAssistUiState.UiStates.WasDestroyedItemImportantToStation : CrimeAssistUiState.UiStates.Result_Vandalism,
            CrimeAssistUiState.UiStates.WasDestroyedItemImportantToStation => yesPressed ? CrimeAssistUiState.UiStates.IsLargePartOfStationDestroyed : CrimeAssistUiState.UiStates.Result_Endangerment,
            CrimeAssistUiState.UiStates.IsLargePartOfStationDestroyed => yesPressed ? CrimeAssistUiState.UiStates.Result_GrandSabotage : CrimeAssistUiState.UiStates.Result_Sabotage,

            //sexual branch
            CrimeAssistUiState.UiStates.WasCrimeSexualInNature => yesPressed ? CrimeAssistUiState.UiStates.Result_SexualHarrassment : CrimeAssistUiState.UiStates.WasSuspectANuisance,

            //nuisance branch
            CrimeAssistUiState.UiStates.WasSuspectANuisance => yesPressed ? CrimeAssistUiState.UiStates.FalselyReportingToSecurity : CrimeAssistUiState.UiStates.Result_Innocent,
            CrimeAssistUiState.UiStates.FalselyReportingToSecurity => yesPressed ? CrimeAssistUiState.UiStates.Result_PerjuryOrFalseReport : CrimeAssistUiState.UiStates.HappenInCourt,
            CrimeAssistUiState.UiStates.HappenInCourt => yesPressed ? CrimeAssistUiState.UiStates.Result_ContemptOfCourt : CrimeAssistUiState.UiStates.DuringActiveInvestigation,
            CrimeAssistUiState.UiStates.DuringActiveInvestigation => yesPressed ? CrimeAssistUiState.UiStates.Result_ObstructionOfJustice : CrimeAssistUiState.UiStates.ToCommandStaff,
            CrimeAssistUiState.UiStates.ToCommandStaff => yesPressed ? CrimeAssistUiState.UiStates.Result_Sedition : CrimeAssistUiState.UiStates.WasItCommandItself,
            CrimeAssistUiState.UiStates.WasItCommandItself => yesPressed ? CrimeAssistUiState.UiStates.Result_AbuseOfPower : CrimeAssistUiState.UiStates.Result_Hooliganism,
            _ => CrimeAssistUiState.UiStates.MainMenu
        };

        UpdateUI(newState);
    }

    public void UpdateUI(CrimeAssistUiState.UiStates state)
    {
        _currentState = state;
        bool isResult = state.ToString().StartsWith("Result");

        StartButton.Visible = state == CrimeAssistUiState.UiStates.MainMenu;
        YesButton.Visible = state != CrimeAssistUiState.UiStates.MainMenu && !isResult;
        NoButton.Visible = state != CrimeAssistUiState.UiStates.MainMenu && !isResult;
        HomeButton.Visible = state != CrimeAssistUiState.UiStates.MainMenu;
        Explanation.Visible = state != CrimeAssistUiState.UiStates.MainMenu;

        Subtitle.Visible = isResult; // Crime severity is displayed here
        Punishment.Visible = isResult; // Crime punishment is displayed here

        if (!isResult)
        {
            Title.Text = GetQuestionLocString(state);
            Subtitle.Text = string.Empty;
            Explanation.Text = string.Empty;
            Punishment.Text = string.Empty;
        }
        else
        {
            Title.Text = GetCrimeNameLocString(state);
            Subtitle.Text = GetCrimeSeverityLocString(state);
            Explanation.Text = GetCrimeExplanationLocString(state);
            Punishment.Text = GetCrimePunishmentLocString(state);
        }
    }

    private string GetQuestionLocString(CrimeAssistUiState.UiStates state)
    {
        return Loc.GetString($"crime-assist-question-{state.ToString().ToLower()}");
    }

    private string GetCrimeExplanationLocString(CrimeAssistUiState.UiStates state)
    {
        return Loc.GetString($"crime-assist-crimedetail-{state.ToString().ToLower().Remove(0, 7)}");
    }

    private string GetCrimeNameLocString(CrimeAssistUiState.UiStates state)
    {
        return Loc.GetString($"crime-assist-crime-{state.ToString().ToLower().Remove(0, 7)}");
    }

    private string GetCrimePunishmentLocString(CrimeAssistUiState.UiStates state)
    {
        return Loc.GetString($"crime-assist-crimepunishment-{state.ToString().ToLower().Remove(0, 7)}");
    }

    private string GetCrimeSeverityLocString(CrimeAssistUiState.UiStates state)
    {
        return state switch
        {
            CrimeAssistUiState.UiStates.Result_AnimalCruelty => Loc.GetString("crime-assist-crimetype-misdemeanour"),
            CrimeAssistUiState.UiStates.Result_Theft => Loc.GetString("crime-assist-crimetype-misdemeanour"),
            CrimeAssistUiState.UiStates.Result_Trespass => Loc.GetString("crime-assist-crimetype-misdemeanour"),
            CrimeAssistUiState.UiStates.Result_Vandalism => Loc.GetString("crime-assist-crimetype-misdemeanour"),
            CrimeAssistUiState.UiStates.Result_Hooliganism => Loc.GetString("crime-assist-crimetype-misdemeanour"),
            CrimeAssistUiState.UiStates.Result_Manslaughter => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_GrandTheft => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_BlackMarketeering => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Sabotage => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Mindbreaking => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Assault => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_AbuseOfPower => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Possession => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Endangerment => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_BreakingAndEntering => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Rioting => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_ContemptOfCourt => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_PerjuryOrFalseReport => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_ObstructionOfJustice => Loc.GetString("crime-assist-crimetype-felony"),
            CrimeAssistUiState.UiStates.Result_Murder => Loc.GetString("crime-assist-crimetype-capital"),
            CrimeAssistUiState.UiStates.Result_Terrorism => Loc.GetString("crime-assist-crimetype-capital"),
            CrimeAssistUiState.UiStates.Result_GrandSabotage => Loc.GetString("crime-assist-crimetype-capital"),
            CrimeAssistUiState.UiStates.Result_Decorporealisation => Loc.GetString("crime-assist-crimetype-capital"),
            CrimeAssistUiState.UiStates.Result_Kidnapping => Loc.GetString("crime-assist-crimetype-capital"),
            CrimeAssistUiState.UiStates.Result_Sedition => Loc.GetString("crime-assist-crimetype-capital"),
            CrimeAssistUiState.UiStates.Result_SexualHarrassment => Loc.GetString("crime-assist-crimetype-capital"),
            _ => ""
        };
    }
}
