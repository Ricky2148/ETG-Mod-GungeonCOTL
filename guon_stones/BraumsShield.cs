using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria;
using Alexandria.ItemAPI;
using UnityEngine;

namespace LOLItems.guon_stones
{
    internal class BraumsShield : AdvancedPlayerOrbitalItem
    {
        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;

        public static int ID;

        public static void Init()
        {
            string itemName = "Braum's Shield"; //The name of the item
            string resourceName = "LOLItems/Resources/guon_sprites/braum_shield_sprites/braumshield_icon_001"; //(inventory sprite) MAKE SURE TO CHANGE THE SPRITE PATH TO YOUR MOD'S RESOURCES.

            GameObject obj = new GameObject();
            var item = obj.AddComponent<BraumsShield>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Stand behind me!";
            string longDesc = "A repurposed vault door used as a shield in a far away land. The original wielder was said to have such a kind soul that " +
                "his own protective instincts were instilled within the magic shield. Now it floats around to protect the user as best it can.\n";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "LOLItems");
            item.quality = PickupObject.ItemQuality.A;

            BuildPrefab();
            item.OrbitalPrefab = orbitalPrefab;
            BuildSynergyPrefab();

            item.HasAdvancedUpgradeSynergy = true; //Set this to true if you want a synergy that changes the appearance of the Guon Stone. All base game guons have a [colour]-er Guon Stone synergy that makes them bigger and brighter.
            item.AdvancedUpgradeSynergy = "Test Guon Stone Synergy"; //This is the name of the synergy that changes the appearance, if you have one.
            item.AdvancedUpgradeOrbitalPrefab = BraumsShield.upgradeOrbitalPrefab.gameObject;
            item.quality = ItemQuality.C;

            ID = item.PickupObjectId;
        }

        public static void BuildPrefab()
        {
            if (BraumsShield.orbitalPrefab != null) return;
            GameObject prefab = SpriteBuilder.SpriteFromResource("LOLItems/Resources/guon_sprites/braum_shield_sprites/braumshield_nofire_001"); //(ingame orbital sprite)MAKE SURE TO CHANGE THE SPRITE PATH TO YOUR MODS RESOURCES
            prefab.name = "Test Guon Stone Orbital"; //The name of the orbital used by the code. Barely ever used or seen, but important to change.
            var body = prefab.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(11, 12)); //This line sets up the hitbox of your guon, this one is set to 5 pixels across by 9 pixels high, but you can set it as big or small as you want your guon to be.           
            body.CollideWithTileMap = false;
            body.CollideWithOthers = true;
            body.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;

            orbitalPrefab = prefab.AddComponent<PlayerOrbital>();
            orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS; //You can ignore most of this stuff, but I've commented on some of it.
            orbitalPrefab.shouldRotate = false; //This determines if the guon stone rotates. If set to true, the stone will rotate so that it always faces towards the player. Most Guons have this set to false, and you probably should too unless you have a good reason for changing it.
            orbitalPrefab.orbitRadius = 2.5f; //This determines how far away from you the guon orbits. The default for most guons is 2.5.
            orbitalPrefab.orbitDegreesPerSecond = 120f; //This determines how many degrees of rotation the guon travels per second. The default for most guons is 120.
            orbitalPrefab.perfectOrbitalFactor = 0f; //This determines how fast guons will move to catch up with their owner (regular guons have it set to 0 so they lag behind). You can probably ignore this unless you want or need your guon to stick super strictly to it's orbit.
            orbitalPrefab.SetOrbitalTier(0);

            GameObject.DontDestroyOnLoad(prefab);
            FakePrefab.MarkAsFakePrefab(prefab);
            prefab.SetActive(false);
        }
        public static void BuildSynergyPrefab()
        {
            bool flag = BraumsShield.upgradeOrbitalPrefab == null;
            if (flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("LOLItems/Resources/npc_sprites/shopkeeper/talk", null); //(The orbital appearance with it's special synergy) MAKE SURE TO CHANGE THE SPRITE PATH TO YOUR OWN MODS
                gameObject.name = "Your Guon Orbital Synergy Form";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(11, 12));
                BraumsShield.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                BraumsShield.upgradeOrbitalPrefab.shouldRotate = false; //Determines if your guon rotates with it's special synergy
                BraumsShield.upgradeOrbitalPrefab.orbitRadius = 2.5f; //Determines how far your guon orbits with it's special synergy
                BraumsShield.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f; //Determines how fast your guon orbits with it's special synergy
                BraumsShield.upgradeOrbitalPrefab.perfectOrbitalFactor = 10f; //Determines how fast your guon will move to catch up with its owner with it's special synergy. By default, even though the regular guons have it at 0, the upgraded synergy guons all have a higher perfectOrbitalFactor. I find 10 to be about the same.
                BraumsShield.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            return base.Drop(player);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
