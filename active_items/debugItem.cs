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

namespace LOLItems
{
    internal class debugItem : PlayerItem
    {
        public static int ID;

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

        public static void Init()
        {
            string itemName = "Debug Item";
            string resourceName = "LOLItems/Resources/example_item_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<debugItem>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            //ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.PerRoom, 20f);
            item.consumable = false;

            item.usableDuringDodgeRoll = true;
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

        public override void DoEffect(PlayerController player)
        {
            //Plugin.Log($"Manamune is {Manamune.ID}");
            //Plugin.Log($"Muramana is {Muramana.ID}");
            //Plugin.Log($"MuramanaSynergyActivation is {MuramanaSynergyActivation.ID}");
            //Plugin.Log($"PowPow is {PowPow.ID}");
            //Plugin.Log($"PowPowAltForm is {PowPowAltForm.ID}");
            //Plugin.Log($"BladeOfTheRuinedKing is {BladeOfTheRuinedKing.ID}");
            //Plugin.Log($"CloakOfStarryNight is {CloakOfStarryNight.ID}");
            //Plugin.Log($"ShieldOfMoltenStone is {ShieldOfMoltenStone.ID}");
            //Plugin.Log($"BraumsShield is {BraumsShield.ID}");
            //StartCoroutine(EffectCoroutine(player));
            //string enemyEventName = player.CurrentGun.projectile.enemyImpactEventName;
            //string objectEventName = player.CurrentGun.projectile.objectImpactEventName;
            //Plugin.Log($"enemyEventName: {enemyEventName}, objectEventName: {objectEventName}");
            /*
            List<string> spreadVFXSpritePath = new List<string>
            {
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_01",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_02",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_03",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_04",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_05",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_06",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_07",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_08",
                "LOLItems/Resources/vfxs/test_vfx/test_vfx_09"
            };

            GameObject customVFXPrefab = VFXBuilder.CreateVFX
            (
                "test_vfx",
                spreadVFXSpritePath,
                10,
                new IntVector2(9, 9),
                tk2dBaseSprite.Anchor.MiddleCenter,
                false,
                0,
                -1,
                Color.cyan,
                tk2dSpriteAnimationClip.WrapMode.Loop,
                true
            );

            GameObject customVFX = SpawnManager.SpawnVFX(customVFXPrefab, true);
            customVFX.transform.position = player.CenterPosition;

            GameObject customVFX2 = Instantiate(customVFXPrefab, player.CenterPosition, Quaternion.identity);
            customVFX2.SetActive(true);
            */

            player.carriedConsumables.Currency += 10;

            //AkSoundEngine.PostEvent("Play_WPN_sniperrifle_shot_01", player.gameObject);

            Plugin.Log("debug item finished");
        }

        private System.Collections.IEnumerator EffectCoroutine(PlayerController player)
        {
            foreach (string name in ShaderBase.Shaders)
            {
                if (name != null)
                {
                    activeVFXObject = player.PlayEffectOnActor(EffectVFX, new Vector3(0, 0, 0), true, false, false);
                    var sprite = activeVFXObject.GetComponent<tk2dSprite>();

                    if (sprite != null)
                    {
                        sprite.HeightOffGround = -1f;
                        sprite.UpdateZDepth();

                        sprite.usesOverrideMaterial = true;

                        Material mat = sprite.renderer.material;

                        mat.shader = ShaderCache.Acquire(name);

                        mat.SetFloat("_EmissivePower", 10f);

                        sprite.UpdateMaterial();
                        sprite.UpdateColors();
                        //sprite.ForceUpdateMaterial();
                        //sprite.UpdateColorsImpl();

                        Plugin.Log($"shader name: {name}");

                        yield return new WaitForSeconds(1f);
                    }
                }
                else
                {
                    Plugin.Log($"failed shader name: {name}");

                    yield return new WaitForSeconds(0.2f);
                }

                Destroy(activeVFXObject);

                yield return new WaitForSeconds(0.2f);
            }
        }

        /*private System.Collections.IEnumerator StasisCoroutine(PlayerController player)
        {
            player.healthHaver.TriggerInvulnerabilityPeriod(StasisDuration);
            player.CurrentInputState = PlayerInputState.NoInput;
            player.healthHaver.PreventAllDamage = true;

            Color originalPlayerColor = player.sprite.color;
            Color originalGunColor = player.CurrentGun.sprite.color;

            // find a better color later
            player.sprite.color = ExtendedColours.honeyYellow;
            player.CurrentGun.sprite.color = ExtendedColours.honeyYellow;

            AkSoundEngine.PostEvent("zhonyas_hourglass_activation_SFX", GameManager.Instance.gameObject);

            yield return new WaitForSeconds(StasisDuration);

            player.sprite.color = originalPlayerColor;
            player.CurrentGun.sprite.color = originalGunColor;

            player.ForceBlank();
            player.CurrentInputState = PlayerInputState.AllInput;

            // wait an extra 0.25 seconds to prevent player from immediate collision damage
            yield return new WaitForSeconds(0.25f);
            player.healthHaver.PreventAllDamage = false;

            AkSoundEngine.PostEvent("zhonyas_hourglass_ending_SFX", GameManager.Instance.gameObject);
        }*/
    }
}
