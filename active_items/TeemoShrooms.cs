using Alexandria;
using Alexandria.BreakableAPI;
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
using System.Reflection;
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

        public static DebrisObject shroomObject;
        public static Projectile shroomExplosion;

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

            shroomObject = BreakableAPIToolbox.GenerateDebrisObject("LOLItems/Resources/example_item_sprite", true, 7, 10, 100, 0, null, .2f, null, null, 0);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById((int)Items.MarineSidearm) as Gun).DefaultModule.projectiles[0]);
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            projectile.baseData.damage = 2;
            projectile.baseData.speed = 0f;
            projectile.baseData.range = 20f;
            projectile.SuppressHitEffects = true;
            projectile.objectImpactEventName = null;
            PierceProjModifier pierce = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierce.penetratesBreakables = true;
            pierce.penetration = 100;

            shroomExplosion = projectile;

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

            if (player != null)
            {
                StartCoroutine(ThrowShroom());
                player.DidUnstealthyAction();
            }
        }

        private System.Collections.IEnumerator ThrowShroom()
        {
            Vector2 dir = this.LastOwner.unadjustedAimPoint - this.LastOwner.CurrentGun.barrelOffset.transform.position;
            GameObject shroomObjectInstance = UnityEngine.Object.Instantiate<GameObject>(shroomObject.gameObject, this.LastOwner.CenterPosition, Quaternion.identity);
            DebrisObject debris = LootEngine.DropItemWithoutInstantiating(shroomObjectInstance, this.LastOwner.CurrentGun.barrelOffset.transform.position, dir, 7, false, false, true, false);

            while (shroomObjectInstance.gameObject != null)
            {
                if (GetPrivateType<DebrisObject, bool>(debris, "onGround"))
                {
                    if (debris != null)
                    {
                        
                    }
                }
            }

            yield return null;
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
    }
}