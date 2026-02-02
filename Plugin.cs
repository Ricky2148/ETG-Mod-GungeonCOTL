using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.SoundAPI;
using BepInEx;
using GungeonCOTL.passive_items;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using SGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

//bother balancing these items later
// update bubbs with item list

namespace GungeonCOTL
{
    [BepInDependency(Alexandria.Alexandria.GUID)] // this mod depends on the Alexandria API: https://enter-the-gungeon.thunderstore.io/package/Alexandria/Alexandria/
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ITEM_PREFIX = "GungeonCOTL";
        public const string GUID = "Ricky2148.etg.GungeonCOTL";
        public const string NAME = "Gungeon Cult of the Lamb";
        public const string VERSION = "1.0.0";
        public const string TEXT_COLOR = "#F1C232";

        internal static Harmony _Harmony;

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {
            ETGMod.Assets.SetupSpritesFromAssembly(Assembly.GetExecutingAssembly(), "LOLItems/Resources/weapon_sprites");
            SoundManager.LoadSoundbanksFromAssembly();

            _Harmony = new Harmony(GUID);

            CarefreeMelody.Init();
            RedCrown.Init();
            CrownUpgradeResurrection.Init();
            CrownUpgradeDarknessWithin.Init();

            Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);
        }

        public static void Log(string text, string color= "#FF007F")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
    }
}
