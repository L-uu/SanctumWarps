using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Mirror;
using SanctumWarps;
using UnityEngine;

namespace SanctumWarps
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static GameObject _localPlayer;
        public static PlayerMove _playerMove;
        public static Player _player;
        public static readonly List<TeleportData> TeleportLocations = new List<TeleportData>
        {
            new TeleportData
            {
                _triggers = new[] { "/shop", "/sally" },
                _coords = new Vector3(196.4603f, 11.2302f, -112.2948f),
                _rot = Quaternion.Euler(0f, 90f, 0f),
                _name = "Sally"
            },
            new TeleportData
            {
                _triggers = new[] { "/dungeon", "/angela" },
                _coords = new Vector3(-8f, 32f, -180f),
                _rot = Quaternion.Euler(0f, 180f, 0f),
                _name = "Angela"
            },
            new TeleportData
            {
                _triggers = new[] { "/enhant", "/vivian" },
                _coords = new Vector3(258f, 5f, -265f),
                _rot = Quaternion.Euler(0f, 100f, 0f),
                _name = "Vivian"
            },
            new TeleportData
            {
                _triggers = new[] { "/arena", "/pvp", "/enok" },
                _coords = new Vector3(-145f, 27.5f, -598.5f),
                _rot = Quaternion.Euler(0f, 270f, 0f),
                _name = "Enok"
            }
        };

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Harmony harmony = new Harmony("SanctumWarps");
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    public class TeleportData
    {
        public string[] _triggers { get; set; }
        public Vector3 _coords { get; set; }
        public Quaternion _rot { get; set; }
        public string _name { get; set; }
    }

    [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.Send_ChatMessage))]
    public static class Send_ChatMessage_Patch
    {
        [HarmonyPrefix]
        public static bool Send_ChatMessage_Prefix(string _message)
        {
            if (!Plugin._localPlayer)
            {
                Plugin._localPlayer = NetworkClient.localPlayer.gameObject;
                Plugin._playerMove = Plugin._localPlayer.GetComponent<PlayerMove>();
                Plugin._player = Plugin._localPlayer.GetComponent<Player>();
                Plugin.Logger.LogInfo("Player object found and warps loaded!");
            }

            if (Plugin._player._mapName != "Sanctum") return false;

            foreach (var data in Plugin.TeleportLocations)
            {
                if (data._triggers.Any(trigger => _message.ToLower().StartsWith(trigger)))
                {
                    Plugin._playerMove.enabled = false;
                    Plugin._localPlayer.transform.position = data._coords;
                    Plugin._localPlayer.transform.rotation = data._rot;
                    Plugin._playerMove.enabled = true;
                    return false;
                }
            }
            return true;
        }
    }
}
