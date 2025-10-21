using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using LOLItems.custom_class_data;
using LOLItems.guon_stones;
using LOLItems.passive_items;
using LOLItems.weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOLItems.active_items
{
    internal class TeemoShrooms : PlayerItem
    {
        public static int ID;

        private static int numShrooms = 5;
        private static float ShroomDPS = 25f;
        private static float ShroomSlowPercent = 0.5f;
        private static float ShroomEffectDuration = 4f;

        private static List<string> VFXSpritePath = new List<string>
            {
                "LOLItems/Resources/vfxs/test_vfx/image (1)",
                "LOLItems/Resources/vfxs/test_vfx/image (2)",
                "LOLItems/Resources/vfxs/test_vfx/image (3)",
                "LOLItems/Resources/vfxs/test_vfx/image (4)",
                "LOLItems/Resources/vfxs/test_vfx/image (5)",
                "LOLItems/Resources/vfxs/test_vfx/image (6)",
                "LOLItems/Resources/vfxs/test_vfx/image (7)",
                "LOLItems/Resources/vfxs/test_vfx/image (8)",
                "LOLItems/Resources/vfxs/test_vfx/image (9)"
            };

        private static GameObject EffectVFX = VFXBuilder.CreateVFX
        (
            "test_vfx",
            VFXSpritePath,
            10,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Loop,
            true
        );

        private GameObject activeVFXObject;

        private static GameActorSpeedEffect ShroomSlowEffect = new GameActorSpeedEffect
        {
            duration = ShroomEffectDuration,
            SpeedMultiplier = ShroomSlowPercent,
            effectIdentifier = "shroom_slow_effect",
            resistanceType = EffectResistanceType.None,
            AppliesTint = true,
            TintColor = ExtendedColours.plaguePurple,
            //AppliesDeathTint = true,
            //DeathTintColor = new Color(0, 0, 0)
        };

        private static Gun phoenix = PickupObjectDatabase.GetById(99) as Gun;
        private static GameActorFireEffect TormentBurnEffect = new GameActorFireEffect
        {
            duration = ShroomEffectDuration,
            DamagePerSecondToEnemies = 0f,
            effectIdentifier = "liandrys_torment_burn",
            ignitesGoops = false,
            FlameVfx = phoenix.DefaultModule.projectiles[0].fireEffect.FlameVfx,
        };

        public static void Init()
        {
            string itemName = "Teemo's Shrooms";
            string resourceName = "LOLItems/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<TeemoShrooms>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Poison";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
            item.consumable = true;
            item.canStack = true;

            item.usableDuringDodgeRoll = false;
            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }

        // subscribe to the player events
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
        }

        public DebrisObject Drop(PlayerController player)
        {
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            return base.Drop(player);
        }

        public void DoEffect(PlayerController player)
        {
            // spawn a projectile that lingers in and when hitting an enemy, explodes and does shroom
        }
    }
}