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

        private static bool ConsumedOnUse = false;

        private static float ReviveDuration = 1.3f;

        private bool hasRevived = false;

        public static int ID;

        public static void Init()
        {
            string itemName = ItemName;
            string resourceName = "GungeonCOTL/Resources/passive_item_sprites/resurrection_pixelart_sprite";

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

            player.healthHaver.OnPreDeath += Resurrection;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.healthHaver.OnPreDeath -= Resurrection;
            }
        }

        private void Resurrection(Vector2 DeathPositon)
        {
            if (!hasRevived && this.Owner is PlayerController player)
            {
                player.StartCoroutine(ReviveCoroutine(player));
            }
        }

        // revives the player with half health and invulnerability for a short time, activates a blank after invul
        private System.Collections.IEnumerator ReviveCoroutine(PlayerController player)
        {
            Plugin.Log($"{this.EncounterNameOrDisplayName} curhealth: {player.healthHaver.GetCurrentHealth()}, isAlive: {player.healthHaver.IsAlive}");

            if (!player.healthHaver.IsAlive)
            {
                Plugin.Log($"{this.EncounterNameOrDisplayName} activated");
                hasRevived = true;

                // makes player character invulnerable, reset health, take no inputs from player, and remove revive effect
                player.healthHaver.TriggerInvulnerabilityPeriod(ReviveDuration + 0.1f);
                //player.TriggerInvulnerableFrames(4.1f);
                player.healthHaver.ForceSetCurrentHealth(Mathf.Max((player.healthHaver.GetMaxHealth() / 4), 0.5f));
                player.CurrentInputState = PlayerInputState.NoInput;
                player.healthHaver.OnPreDeath -= Resurrection;

                Color originalPlayerColor = player.sprite.color;
                Color originalGunColor = player.CurrentGun.sprite.color;

                player.sprite.color = ExtendedColours.maroon;
                player.CurrentGun.sprite.color = ExtendedColours.maroon;

                Material mat = SpriteOutlineManager.GetOutlineMaterial(player.sprite);
                if (mat)
                {
                    mat.SetColor("_OverrideColor", new Color(105f * 0.3f, 7f * 0.3f, 9f * 0.3f));
                }

                AkSoundEngine.PostEvent("resurrection", player.gameObject);

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
                player.DoGhostBlank();

                player.CurrentInputState = PlayerInputState.AllInput;
                player.healthHaver.PreventAllDamage = false;

                if (ConsumedOnUse)
                {
                    player.RemovePassiveItem(ID);
                }
            }
        }
    }
}
