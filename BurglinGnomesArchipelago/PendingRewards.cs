using System.Collections.Generic;
using UnityEngine;

namespace BurglinGnomesArchipelago
{
    /// <summary>
    /// Holds items received from the Archipelago multiworld that haven't
    /// been claimed yet. Items are NEVER pushed directly to the player
    /// inventory on receipt -- they queue here instead, so the player
    /// can claim them from the F6 overlay when they have inventory space
    /// and are in a valid game state (in a run, as the server/owner).
    ///
    /// Persists across scene loads via PlayerPrefs so unclaimed items
    /// survive session restarts. Uses a simple JSON-lines format so no
    /// extra serialization dependency is needed.
    /// </summary>
    public static class PendingRewards
    {
        private const string PrefsKey = "APMod_PendingRewards";

        private static List<string> _queue = null;

        private static List<string> Queue
        {
            get
            {
                if (_queue == null) Load();
                return _queue;
            }
        }

        public static int Count => Queue.Count;

        public static IReadOnlyList<string> All => Queue.AsReadOnly();

        /// <summary>Add an item to the pending queue and persist.</summary>
        public static void Enqueue(string itemName)
        {
            Queue.Add(itemName);
            Save();
            Plugin.Log.LogInfo($"[PendingRewards] Queued '{itemName}'. Total pending: {Queue.Count}");
        }

        /// <summary>
        /// Try to claim the next pending item into the local player's
        /// inventory. Returns the claimed item name on success, null if
        /// no items are pending, the player isn't in a game yet, or the
        /// inventory is full.
        /// </summary>
        public static string TryClaim()
        {
            if (Queue.Count == 0) return null;

            var localPlayer = ServerManager.GetLocalPlayer();
            if (localPlayer == null)
            {
                Plugin.Log.LogWarning("[PendingRewards] Cannot claim: not spawned into a game yet.");
                return null;
            }

            string itemName = Queue[0];

            // "Gnomium Payout" is a synthetic AP progression token.
            // Map it to real gnomium so it's not a dead claim.
            string grantName = itemName == "Gnomium Payout" ? "gnomium" : itemName;

            bool added = localPlayer.Inventory.TryAddItem(grantName);
            if (!added)
            {
                Plugin.Log.LogWarning($"[PendingRewards] Could not claim '{grantName}' -- inventory full or item name mismatch. Item stays queued.");
                return null;
            }

            Queue.RemoveAt(0);
            Save();
            Plugin.Log.LogInfo($"[PendingRewards] Claimed '{grantName}' (AP item: '{itemName}'). Remaining: {Queue.Count}");
            return grantName;
        }

        /// <summary>Claim ALL pending items at once (skips any that fail).</summary>
        public static List<string> TryClaimAll()
        {
            var claimed = new List<string>();
            string result;
            while (Queue.Count > 0)
            {
                result = TryClaim();
                if (result == null) break; // inventory full or player missing
                claimed.Add(result);
            }
            return claimed;
        }

        private static void Save()
        {
            PlayerPrefs.SetString(PrefsKey, string.Join("\n", Queue));
            PlayerPrefs.Save();
        }

        private static void Load()
        {
            _queue = new List<string>();
            string raw = PlayerPrefs.GetString(PrefsKey, "");
            if (!string.IsNullOrEmpty(raw))
            {
                foreach (var line in raw.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        _queue.Add(line.Trim());
                }
                if (_queue.Count > 0)
                    Plugin.Log.LogInfo($"[PendingRewards] Loaded {_queue.Count} unclaimed item(s) from previous session.");
            }
        }
    }
}
