using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BurglinGnomesArchipelago
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid    = "yourname.burglingnomes.archipelago";
        public const string PluginName    = "Burglin' Gnomes Archipelago Client";
        public const string PluginVersion = "0.1.1";

        internal static ManualLogSource Log;
        internal static ArchipelagoManager Archipelago;

        public static ConfigEntry<string>  ServerAddress;
        public static ConfigEntry<int>     ServerPort;
        public static ConfigEntry<string>  SlotName;
        public static ConfigEntry<string>  Password;
        public static ConfigEntry<bool>    AutoConnectOnLaunch;
        public static ConfigEntry<KeyCode> ToggleGuiKey;

        private Harmony _harmony;
        private bool    _guiVisible;
        private string  _addressField, _portField, _slotField, _passwordField;
        private Vector2 _pendingScrollPos;
        private string  _lastClaimMessage = "";
        private float   _lastClaimMessageTimer;

        private void Awake()
        {
            Log = Logger;

            ServerAddress       = Config.Bind("Archipelago", "ServerAddress",       "archipelago.gg", "Hostname (no port).");
            ServerPort          = Config.Bind("Archipelago", "ServerPort",           38281,            "Port.");
            SlotName            = Config.Bind("Archipelago", "SlotName",             "Player1",        "Your slot name.");
            Password            = Config.Bind("Archipelago", "Password",             "",               "Room password (blank if none).");
            AutoConnectOnLaunch = Config.Bind("Archipelago", "AutoConnectOnLaunch",  false,            "Connect automatically on launch.");
            ToggleGuiKey        = Config.Bind("Debug",       "ToggleGuiKey",         KeyCode.F6,       "Toggle the AP overlay.");

            _addressField = ServerAddress.Value;
            _portField    = ServerPort.Value.ToString();
            _slotField    = SlotName.Value;
            _passwordField = Password.Value;

            Archipelago = new ArchipelagoManager(Log);
            _harmony    = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded. Press {ToggleGuiKey.Value} for the AP overlay.");

            if (AutoConnectOnLaunch.Value)
                Archipelago.Connect(ServerAddress.Value, ServerPort.Value, SlotName.Value, Password.Value);
        }

        private void Update()
        {
            if (Input.GetKeyDown(ToggleGuiKey.Value))
                _guiVisible = !_guiVisible;

            GameHooks.TryHook();

            if (_lastClaimMessageTimer > 0f)
                _lastClaimMessageTimer -= Time.deltaTime;
        }

        private void OnGUI()
        {
            if (!_guiVisible) return;

            // === LEFT PANEL: Connection ===
            GUI.Box(new Rect(20, 20, 320, 230), "Archipelago Connection");

            GUI.Label(new Rect(30, 48, 100, 20), "Server");
            _addressField = GUI.TextField(new Rect(120, 48, 210, 20), _addressField);

            GUI.Label(new Rect(30, 73, 100, 20), "Port");
            _portField = GUI.TextField(new Rect(120, 73, 210, 20), _portField);

            GUI.Label(new Rect(30, 98, 100, 20), "Slot name");
            _slotField = GUI.TextField(new Rect(120, 98, 210, 20), _slotField);

            GUI.Label(new Rect(30, 123, 100, 20), "Password");
            _passwordField = GUI.PasswordField(new Rect(120, 123, 210, 20), _passwordField, '*');

            string status = Archipelago.Connected ? "Connected" : "Disconnected";
            GUI.Label(new Rect(30, 150, 300, 20), $"Status: {status}");

            if (!Archipelago.Connected)
            {
                if (GUI.Button(new Rect(30, 175, 130, 30), "Connect"))
                {
                    if (int.TryParse(_portField, out int port))
                    {
                        ServerAddress.Value = _addressField;
                        ServerPort.Value    = port;
                        SlotName.Value      = _slotField;
                        Password.Value      = _passwordField;
                        Archipelago.Connect(_addressField, port, _slotField, _passwordField);
                    }
                    else Log.LogWarning("[AP] Port must be a number.");
                }
            }
            else
            {
                if (GUI.Button(new Rect(30, 175, 130, 30), "Disconnect"))
                    Archipelago.Disconnect();
            }

            GUI.Label(new Rect(30, 210, 300, 20), "Pending rewards: " + PendingRewards.Count);

            // === RIGHT PANEL: Pending Rewards ===
            float panelX  = 360f;
            float panelY  = 20f;
            float panelW  = 340f;
            float panelH  = 360f;

            GUI.Box(new Rect(panelX, panelY, panelW, panelH), "Pending Rewards (Unclaimed AP Items)");

            IReadOnlyList<string> all = PendingRewards.All;

            if (all.Count == 0)
            {
                GUI.Label(new Rect(panelX + 10, panelY + 30, panelW - 20, 30), "Nothing pending.");
            }
            else
            {
                // Scrollable list of pending items
                float listH    = panelH - 110f;
                float rowH     = 22f;
                float contentH = Mathf.Max(all.Count * rowH, listH);

                _pendingScrollPos = GUI.BeginScrollView(
                    new Rect(panelX + 10, panelY + 30, panelW - 20, listH),
                    _pendingScrollPos,
                    new Rect(0, 0, panelW - 40, contentH)
                );
                for (int i = 0; i < all.Count; i++)
                {
                    string label = $"{i + 1}. {all[i]}";
                    if (all[i] == "Gnomium Payout")
                        label += "  (grants gnomium)";
                    GUI.Label(new Rect(4, i * rowH, panelW - 50, rowH), label);
                }
                GUI.EndScrollView();

                // Claim buttons
                float btnY = panelY + panelH - 72f;

                if (GUI.Button(new Rect(panelX + 10, btnY, 150, 30), "Claim Next Item"))
                {
                    string claimed = PendingRewards.TryClaim();
                    _lastClaimMessage = claimed != null
                        ? $"Claimed: {claimed}"
                        : "Could not claim (full / not in game)";
                    _lastClaimMessageTimer = 3f;
                }

                if (GUI.Button(new Rect(panelX + 170, btnY, 160, 30), "Claim All"))
                {
                    var claimed = PendingRewards.TryClaimAll();
                    _lastClaimMessage = claimed.Count > 0
                        ? $"Claimed {claimed.Count} item(s)"
                        : "Could not claim (full / not in game)";
                    _lastClaimMessageTimer = 3f;
                }

                // Feedback message
                if (_lastClaimMessageTimer > 0f)
                    GUI.Label(new Rect(panelX + 10, panelY + panelH - 30, panelW - 20, 24), _lastClaimMessage);
            }
        }

        private void OnDestroy()
        {
            Archipelago?.Disconnect();
            _harmony?.UnpatchSelf();
        }
    }
}
