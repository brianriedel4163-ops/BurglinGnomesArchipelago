using NodeCanvas.Framework;
using UnityEngine;

namespace BurglinGnomesArchipelago
{
    /// <summary>
    /// Real hooks wired against the actual decompiled game code.
    ///
    ///   - PlayerTaskManager.OnTaskCompleted fires for every completed
    ///     High-Gnome task (found via GlobalBlackboard).
    ///   - Received AP items go into PendingRewards queue, NOT directly
    ///     into the player inventory. Claim them from the F6 overlay.
    ///     This prevents the backpack/equipment crash and stops items
    ///     being lost to a full inventory.
    ///
    /// Location name convention: "Task: {PlayerTaskBase.asset name}"
    /// </summary>
    public static class GameHooks
    {
        private static PlayerTaskManager _taskManager;
        private static bool _subscribedToGameStart;

        public static void TryHook()
        {
            if (!_subscribedToGameStart)
            {
                var progression = GameProgressionManager.Instance;
                if (progression == null) return;

                progression.onGameStarted += OnGameStarted;
                progression.onGameEnded += OnGameEnded;
                _subscribedToGameStart = true;
                TryHookTaskManager();
            }
        }

        private static void OnGameStarted()
        {
            TryHookTaskManager();
        }

        private static void OnGameEnded(ServerManager.GameEndedReason reason)
        {
            UnhookTaskManager();
        }

        private static void TryHookTaskManager()
        {
            if (_taskManager != null) return;

            var board = GlobalBlackboard.Find("Global");
            if (board == null) return;

            var manager = board.GetVariableValue<PlayerTaskManager>("taskManager");
            if (manager == null) return;

            _taskManager = manager;
            _taskManager.OnTaskCompleted += OnTaskCompleted;
            Plugin.Log.LogInfo("[GameHooks] Hooked PlayerTaskManager.OnTaskCompleted.");
        }

        private static void UnhookTaskManager()
        {
            if (_taskManager != null)
            {
                _taskManager.OnTaskCompleted -= OnTaskCompleted;
                _taskManager = null;
            }
        }

        private static void OnTaskCompleted(PlayerTaskManager.TaskEventData eventData)
        {
            string taskName = eventData.taskRef != null ? eventData.taskRef.name : "Unknown Task";
            string locationName = $"Task: {taskName}";

            var locationId = Plugin.Archipelago.LocationIdByName(locationName);
            if (locationId.HasValue)
            {
                Plugin.Archipelago.SendLocationCheck(locationId.Value);
                Plugin.Log.LogInfo($"[GameHooks] Task completed -> location check '{locationName}'.");
            }
            else
            {
                Plugin.Log.LogInfo($"[GameHooks] Task '{locationName}' completed but no matching AP location found (name mismatch or apworld not loaded).");
            }
        }

        /// <summary>
        /// Called by ArchipelagoManager when an item arrives from the
        /// multiworld. Goes to PendingRewards queue -- claimed from the
        /// F6 overlay, NOT pushed directly to inventory.
        /// </summary>
        public static void ApplyReceivedItem(string itemName)
        {
            PendingRewards.Enqueue(itemName);
        }
    }
}
