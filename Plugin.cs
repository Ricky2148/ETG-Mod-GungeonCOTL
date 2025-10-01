using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.SoundAPI;
using BepInEx;
using LOLItems.active_items;
using LOLItems.guon_stones;
using LOLItems.passive_items;
using LOLItems.weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

//bother balancing these items later

namespace LOLItems
{
    [BepInDependency(Alexandria.Alexandria.GUID)] // this mod depends on the Alexandria API: https://enter-the-gungeon.thunderstore.io/package/Alexandria/Alexandria/
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Ricky2148.etg.LOLItems";
        public const string NAME = "League of legends Items";
        public const string VERSION = "1.1.100";
        public const string TEXT_COLOR = "#FF007F";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {
            ETGMod.Assets.SetupSpritesFromAssembly(Assembly.GetExecutingAssembly(), "LOLItems/Resources/weapon_sprites");
            SoundManager.LoadSoundbanksFromAssembly();
            BladeOfTheRuinedKing.Init();
            //ExamplePassive.Register();
            ExperimentalHexplate.Init();
            GuardianAngel.Init();
            GuinsoosRageblade.Init();
            Hubris.Init();
            KrakenSlayer.Init();
            LiandrysTorment.Init();
            MuramanaSynergyActivation.Init();
            Manamune.Init();
            Muramana.Init();
            StatikkShiv.Init();
            Stridebreaker.Init();
            SunfireAegis.Init();
            Thornmail.Init();
            ZhonyasHourglass.Init();
            //Redemption.Init();

            //new update?
            Collector.Init();
            FrozenHeart.Init();
            RodOfAges.Init();
            HorizonFocus.Init();
            Puppeteer.Init();
            Galeforce.Init();
            RylaisCrystalScepter.Init();
            Shadowflame.Init();
            NavoriQuickblades.Init();

            //testing
            //CarefreeMelody.Init();
            debugItem.Init();

            //guon stones
            BraumsShield.Init();

            //weapons
            //BasicGun.Add();
            //TemplateGun.Add();
            PowPow.Add();
            PowPowAltForm.Add();
            HextechRifle.Add();

            //new items
            ShieldOfMoltenStone.Init();
            CloakOfStarryNight.Init();
            ZekesConvergence.Init();

            //npcs?
            Bubbs.Init();
            Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);
        }

        public static void Log(string text, string color= "#FF007F")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
    }
}
