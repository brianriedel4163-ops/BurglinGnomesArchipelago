using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using BepInEx.Logging;

namespace BurglinGnomesArchipelago
{
    /// <summary>
    /// Wraps an Archipelago.MultiClient.Net session. This part talks real,
    /// documented Archipelago protocol via the official client library and
    /// doesn't need any game-specific reverse engineering.
    ///
    /// NOTE: I haven't been able to test this against a live server or the
    /// exact Archipelago.MultiClient.Net NuGet version you'll end up
    /// pulling in -- method names below (TryConnectAndLogin, session.Items,
    /// session.Locations, etc.) match the API as I know it, but if your
    /// package version differs slightly, check IntelliSense/the library's
    /// own docs for the current signatures.
    /// </summary>
    public class ArchipelagoManager
    {
        private const string GameName = "Burglin' Gnomes";

        private readonly ManualLogSource _log;
        private ArchipelagoSession _session;

        public bool Connected { get; private set; }

        public ArchipelagoManager(ManualLogSource log)
        {
            _log = log;
        }

        public void Connect(string address, int port, string slotName, string password)
        {
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(address, port);

                LoginResult result = _session.TryConnectAndLogin(
                    game: GameName,
                    name: slotName,
                    itemsHandlingFlags: ItemsHandlingFlags.AllItems,
                    version: new Version(0, 5, 0),
                    password: string.IsNullOrEmpty(password) ? null : password
                );

                if (!result.Successful)
                {
                    var failure = (LoginFailure)result;
                    _log.LogError($"[Archipelago] Connection failed: {string.Join(", ", failure.Errors)}");
                    Connected = false;
                    return;
                }

                Connected = true;
                _log.LogInfo("[Archipelago] Connected to " + address + ":" + port);

                _session.Items.ItemReceived += OnItemReceived;
                _session.Socket.SocketClosed += reason =>
                {
                    Connected = false;
                    _log.LogWarning($"[Archipelago] Disconnected: {reason}");
                };
            }
            catch (Exception e)
            {
                _log.LogError($"[Archipelago] Exception while connecting: {e}");
                Connected = false;
            }
        }

        public void Disconnect()
        {
            try { _session?.Socket?.DisconnectAsync(); }
            catch (Exception e) { _log.LogWarning($"[Archipelago] Error during disconnect: {e}"); }
            Connected = false;
        }

        private void OnItemReceived(IReceivedItemsHelper helper)
        {
            ItemInfo item = helper.DequeueItem();
            _log.LogInfo($"[Archipelago] Received '{item.ItemName}' from {item.Player.Name} (location: {item.LocationName})");

            // Game-specific translation lives in GameHooks -- see that file
            // for what still needs real hooks wired in.
            GameHooks.ApplyReceivedItem(item.ItemName);
        }

        /// <summary>
        /// Call this from a Harmony patch (see GameHooks.cs) when the player
        /// does the in-game action that should register as an Archipelago
        /// location check.
        /// </summary>
        public void SendLocationCheck(long locationId)
        {
            if (!Connected)
            {
                _log.LogWarning("[Archipelago] Tried to send a location check while disconnected.");
                return;
            }
            _session.Locations.CompleteLocationChecks(locationId);
        }

        /// <summary>
        /// Looks up a location's numeric ID by name. Names must exactly
        /// match whatever a Burglin' Gnomes apworld defines -- there isn't
        /// one yet (see README), so this will return null until that
        /// exists and is loaded on the server you connect to.
        /// </summary>
        public long? LocationIdByName(string name)
        {
            if (!Connected) return null;
            return _session.Locations.GetLocationIdFromName(GameName, name);
        }
    }
}
