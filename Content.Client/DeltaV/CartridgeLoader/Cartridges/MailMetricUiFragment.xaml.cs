using Content.Shared.CartridgeLoader.Cartridges;
using FastAccessors.Monads;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.DeltaV.CartridgeLoader.Cartridges;

[GenerateTypedNameReferences]
public sealed partial class MailMetricUiFragment : BoxContainer
{
    public MailMetricUiFragment()
    {
        RobustXamlLoader.Load(this);
    }

    public void UpdateState(MailMetricUiState state)
    {
        OpenedMailCount.Text = state.OpenedMailCount.ToString();
        OpenedMailSpesos.Text = state.MailEarnings.ToString();
        TamperedMailCount.Text = state.TamperedMailCount.ToString();
        TamperedMailSpesos.Text = state.TamperedMailLosses.ToString();
        ExpiredMailCount.Text = state.ExpiredMailCount.ToString();
        ExpiredMailSpesos.Text = state.ExpiredMailLosses.ToString();
        DamagedMailCount.Text = state.DamagedMailCount.ToString();
        DamagedMailSpesos.Text = state.DamagedMailLosses.ToString();
        UnopenedMailCount.Text = state.UnopenedMailCount.ToString();
        TotalMailCount.Text = state.TotalMail.ToString();
        TotalMailSpesos.Text = state.TotalIncome.ToString();
        SuccessRateCounts.Text = Loc.GetString("mail-metrics-progress",
            ("opened", state.OpenedMailCount),
            ("total", state.TotalMail));
        SuccessRatePercent.Text = Loc.GetString("mail-metrics-progress-percent",
            ("successRate", state.SuccessRate));

        // OpenedMail.Text = $"Opened mail: {state.OpenedMailCount} (${state.MailEarnings})";
        // TamperedMail.Text = $"Tampered mail: {state.TamperedMailCount} (${state.TamperedMailLosses})";
        // ExpiredMail.Text = $"Expired mail: {state.ExpiredMailCount} (${state.ExpiredMailLosses})";
        // DamagedMail.Text = $"Damaged mail: {state.DamagedMailCount} (${state.DamagedMailLosses})";
        // TotalMail.Text = $"Total mail opened: {state.OpenedMailCount} out of {state.TotalMail} ({state.SuccessRate}%)";
        // TotalEarnings.Text = $"Total earnings: ${state.TotalIncome}";
    }
}
