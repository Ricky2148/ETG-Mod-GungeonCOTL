using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using LOLItems.custom_class_data;
using LOLItems.guon_stones;
using LOLItems.passive_items;
using LOLItems.weapons;
using System;
using System.Collections;
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
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
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
            StartCoroutine(EffectCoroutine(player));

            //HelpfulMethods.CustomNotification("smth1", "smth2", this.sprite);

            Plugin.Log("debug item finished");
        }

        private System.Collections.IEnumerator EffectCoroutine(PlayerController player)
        {
            /*Material mat = SpriteOutlineManager.GetOutlineMaterial(player.sprite);
            if (mat)
            {
                mat.SetColor("_OverrideColor", new Color(255f * 0.3f, 180f * 0.3f, 18f * 0.3f));
            }
            yield return new WaitForSeconds(1f);
            if (mat)
            {
                mat.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
            }*/

            tk2dBaseSprite s = this.sprite;

            //GameUIRoot.Instance.RegisterDefaultLabel(s.transform, new Vector3(s.GetBounds().max.x + 0f, s.GetBounds().min.y + 0f, 0f), "something message here");

            GameUIRoot.Instance.RegisterDefaultLabel(s.transform, new Vector3(s.GetBounds().max.x - 1f, s.GetBounds().min.y - 2f, 0f), "something message here");


            yield return new WaitForSeconds(5f);

            GameUIRoot.Instance.DeregisterDefaultLabel(s.transform);
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
