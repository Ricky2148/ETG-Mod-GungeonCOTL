using Alexandria.ItemAPI;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonCOTL.passive_items
{
    internal class CrownUpgradeResurrection : PassiveItem
    {
        public static string ItemName = "Crown Upgrade Resurrection";

        private static bool ConsumedOnUse = true;

        private static float ReviveDuration = 2f;

        private bool hasRevived = false;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/resurrection_placeholder_sprite";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<CrownUpgradeResurrection>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "idk";
            string longDesc = "idk";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, Plugin.ITEM_PREFIX);

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.SetName("Resurrection");
            ID = item.PickupObjectId;
            //Plugin.Log($"ID: {ID}, pickupID: {item.PickupObjectId}");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");

            player.healthHaver.OnPreDeath += Rebirth;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.healthHaver.OnPreDeath -= Rebirth;
            }
        }

        private void Rebirth(Vector2 DeathPositon)
        {
            if (!hasRevived && this.Owner is PlayerController player)
            {
                hasRevived = true;
                player.StartCoroutine(ReviveCoroutine(player));
            }
        }

        // revives the player with half health and invulnerability for a short time, activates a blank after invul
        private System.Collections.IEnumerator ReviveCoroutine(PlayerController player)
        {
            // makes player character invulnerable, reset health, take no inputs from player, and remove revive effect
            player.healthHaver.TriggerInvulnerabilityPeriod(ReviveDuration + 0.1f);
            //player.TriggerInvulnerableFrames(4.1f);
            player.healthHaver.ForceSetCurrentHealth(player.healthHaver.GetMaxHealth());
            player.CurrentInputState = PlayerInputState.NoInput;
            player.healthHaver.OnPreDeath -= Rebirth;

            Color originalPlayerColor = player.sprite.color;
            Color originalGunColor = player.CurrentGun.sprite.color;

            player.sprite.color = ExtendedColours.maroon;
            player.CurrentGun.sprite.color = ExtendedColours.maroon;

            Material mat = SpriteOutlineManager.GetOutlineMaterial(player.sprite);
            if (mat)
            {
                mat.SetColor("_OverrideColor", new Color(105f * 0.3f, 7f * 0.3f, 9f * 0.3f));
            }

            //AkSoundEngine.PostEvent("guardian_angel_passive_SFX", GameManager.Instance.gameObject);

            // animations for the revive: animation of the player's health being restored
            // and the player being invulnerable for a short time
            // including sound effects and visual effects
            // during the invulnerability period, enemies be frozen in time???

            yield return new WaitForSeconds(ReviveDuration);

            player.sprite.color = originalPlayerColor;
            player.CurrentGun.sprite.color = originalGunColor;

            if (mat)
            {
                mat.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
            }

            // trigger blank to push away enemies and clear bullets, restore input, and remove invulerability
            player.ForceBlank();

            player.CurrentInputState = PlayerInputState.AllInput;
            player.healthHaver.PreventAllDamage = false;

            if (ConsumedOnUse)
            {
                player.RemovePassiveItem(ID);
            }
        }
    }
}
