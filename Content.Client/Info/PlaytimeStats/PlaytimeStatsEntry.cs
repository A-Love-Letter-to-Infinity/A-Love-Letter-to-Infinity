using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Info.PlaytimeStats;

[GenerateTypedNameReferences]
public sealed partial class PlaytimeStatsEntry : ContainerButton
{
    public TimeSpan Playtime { get; private set; }  // new TimeSpan property

    public PlaytimeStatsEntry(string role, TimeSpan playtime, StyleBox styleBox)
    {
        RobustXamlLoader.Load(this);

        RoleLabel.Text = role;
        Playtime = playtime;  // store the TimeSpan value directly
        PlaytimeLabel.Text = ConvertTimeSpanToHoursMinutes(playtime);  // convert to string for display
        BackgroundColorPanel.PanelOverride = styleBox;
    }

    private static string ConvertTimeSpanToHoursMinutes(TimeSpan timeSpan)
    {
        var hours = (int)timeSpan.TotalHours;
        var minutes = timeSpan.Minutes;

        var formattedTimeLoc = Loc.GetString("ui-playtime-time-format", ("hours", hours), ("minutes", minutes));
        return formattedTimeLoc;
    }

    public void UpdateShading(StyleBoxFlat styleBox)
    {
        BackgroundColorPanel.PanelOverride = styleBox;
    }
    public string? PlaytimeText => PlaytimeLabel.Text;

    public string? RoleText => RoleLabel.Text;
}
