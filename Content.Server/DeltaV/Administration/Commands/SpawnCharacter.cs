using System.Linq;
using Content.Server.Administration;
using Content.Server.GameTicking;
using Content.Server.Players;
using Content.Server.Preferences.Managers;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Network;

namespace Content.Server.DeltaV.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class SpawnCharacter : IConsoleCommand
{
    [Dependency] private readonly IEntitySystemManager _entitySys = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IServerPreferencesManager _prefs = default!;

    public string Command => "spawncharacter";
    public string Description => Loc.GetString("spawncharacter-command-description");
    public string Help => Loc.GetString("spawncharacter-command-help");

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player as IPlayerSession;
        if (player == null)
        {
            shell.WriteError(Loc.GetString("shell-only-players-can-run-this-command"));
            return;
        }

        var mindSystem = _entitySys.GetEntitySystem<SharedMindSystem>();

        var data = player.ContentData();

        if (data == null || data.UserId == null)
        {
            shell.WriteError(Loc.GetString("shell-entity-is-not-mob"));
            return;
        }


        HumanoidCharacterProfile character;

        if (args.Length >= 1)
        {
            // This seems like a bad way to go about it, but it works so eh?
            var name = string.Join(" ", args.ToArray());
            shell.WriteLine(Loc.GetString("loadcharacter-command-fetching", ("name", name)));

            if (!FetchCharacters(data.UserId, out var characters))
            {
                shell.WriteError(Loc.GetString("loadcharacter-command-failed-fetching"));
                return;
            }

            var selectedCharacter = characters.FirstOrDefault(c => c.Name == name);

            if (selectedCharacter == null)
            {
                shell.WriteError(Loc.GetString("loadcharacter-command-failed-fetching"));
                return;
            }

            character = selectedCharacter;
        }
        else
            character = (HumanoidCharacterProfile) _prefs.GetPreferences(data.UserId).SelectedCharacter;


        var coordinates = player.AttachedEntity != null
            ? _entityManager.GetComponent<TransformComponent>(player.AttachedEntity.Value).Coordinates
            : _entitySys.GetEntitySystem<GameTicker>().GetObserverSpawnPoint();

        if (player.AttachedEntity == null ||
            !mindSystem.TryGetMind(player.AttachedEntity.Value, out var mindId, out var mind))
            return;


        mindSystem.TransferTo(mindId, _entityManager.System<StationSpawningSystem>()
            .SpawnPlayerMob(coordinates, profile: character, entity: null, job: null, station: null));

        shell.WriteLine(Loc.GetString("spawncharacter-command-complete"));
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var player = shell.Player as IPlayerSession;
            if (player == null)
                return CompletionResult.Empty;

            var data = player.ContentData();
            var mind = data?.Mind;

            if (mind == null || data == null)
                return CompletionResult.Empty;

            if (FetchCharacters(data.UserId, out var characters))
                return CompletionResult.FromOptions(characters.Select(c => c.Name));

            return CompletionResult.Empty;
        }

        return CompletionResult.Empty;
    }

    private bool FetchCharacters(NetUserId player, out HumanoidCharacterProfile[] characters)
    {
        characters = null!;
        if (!_prefs.TryGetCachedPreferences(player, out var prefs))
            return false;

        characters = prefs.Characters
            .Where(kv => kv.Value is HumanoidCharacterProfile)
            .Select(kv => (HumanoidCharacterProfile) kv.Value)
            .ToArray();

        return true;
    }
}
