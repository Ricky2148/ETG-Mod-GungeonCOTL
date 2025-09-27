using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using LootTableAPI;
using Alexandria.NPCAPI;
using System.Reflection;
using Dungeonator;
using LOLItems.active_items;
using LOLItems.passive_items;
using Alexandria.DungeonAPI;

namespace LOLItems
{
    public static class ShopKeeper
    {
        public static GenericLootTable ShopKeeperLootTable;
        public static GameObject HandyGameObject;
        public static void Init()
        {
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_GENERIC_TALK", "I Handy. Arms Dealer. Deal Arms.  :)");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_GENERIC_TALK", "Can't Hear Well.");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_GENERIC_TALK", "Huh?");

            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_STOPPER_TALK", "Trade?");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_STOPPER_TALK", "Parts for parts?");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_STOPPER_TALK", "Parts for parts?");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_STOPPER_TALK", "Trade Parts?");

            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_PURCHASE_TALK", "Yes! Yes! Good Trade");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_PURCHASE_TALK", "Happy!");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_PURCHASE_TALK", "Enjoy! :D");

            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_NOSALE_TALK", "No Trade.");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_NOSALE_TALK", "More Health.");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_NOSALE_TALK", "Not Safe.");

            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_INTRO_TALK", "Friend?");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_INTRO_TALK", "Haii :3");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_INTRO_TALK", "Hellow");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_INTRO_TALK", "Welcom");

            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_ATTACKED_TALK", "AUGChK!");
            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_ATTACKED_TALK", "Bad!");

            ETGMod.Databases.Strings.Core.AddComplex("#SHOPKEEPER_STOLEN_TALK", "???");

            List<int> LootTable = new List<int>()
            {
                Galeforce.ID,
                //Redemption.ID,
                Stridebreaker.ID,
                ZhonyasHourglass.ID,
                
                BladeOfTheRuinedKing.ID,
                Collector.ID,
                ExperimentalHexplate.ID,
                FrozenHeart.ID,
                GuardianAngel.ID,
                GuinsoosRageblade.ID,
                HorizonFocus.ID,
                Hubris.ID,
                KrakenSlayer.ID,
                LiandrysTorment.ID,
                Manamune.ID,
                Muramana.ID,
                NavoriQuickblades.ID,
                Puppeteer.ID,
                RodOfAges.ID,
                RylaisCrystalScepter.ID,
                Shadowflame.ID,
                StatikkShiv.ID,
                SunfireAegis.ID,
                Thornmail.ID,
            };

            ShopKeeperLootTable = LootTableTools.CreateLootTable();
            foreach (int i in LootTable)
            {
                ShopKeeperLootTable.AddItemToPool(i);
            }
            
            GameObject ShopKeeperObj = Alexandria.NPCAPI.ShopAPI.SetUpShop(
                "Shop Keeper", //name
                "Ricky2148", //prefix
                new List<string>() //idle
                {
                    "LOLItems/Resources/npc_sprites/shopkeeper/idle"
                },
                6, //idle fps
                new List<string>() //talk
                {
                    "LOLItems/Resources/npc_sprites/shopkeeper/talk"
                },
                6, //talk fps
                ShopKeeperLootTable, //table
                Alexandria.NPCAPI.CustomShopItemController.ShopCurrencyType.COINS, //cost

                "#SHOPKEEPER_GENERIC_TALK",
                "#SHOPKEEPER_STOPPER_TALK",
                "#SHOPKEEPER_PURCHASE_TALK",
                "#SHOPKEEPER_NOSALE_TALK",
                "#SHOPKEEPER_INTRO_TALK",
                "#SHOPKEEPER_ATTACKED_TALK",
                "#SHOPKEEPER_STOLEN_TALK",
                    
                //talk stuff
                new Vector3(.6f, 3.3f, 0), //talk point
                new Vector3(1.5f, 3, 0), //NPC position
                Alexandria.NPCAPI.ShopAPI.VoiceBoxes.BROTHER_ALBERN,
                Alexandria.NPCAPI.ShopAPI.defaultItemPositions,

                1f, //cost mod
                false, //stats on purchase
                null, //no stats on purchase
                null, // custom can buy?
                null, // custom remove currency?
                null, // custom price
                null, // on buy
                null, // on rob
                null, // currency icon
                null, // currency name
                true, // can be robbed
                true, // has carpet
                "LOLItems/Resources/npc_sprites/shopkeeper/carpet",
                new Vector3(0, 0, 0), // carpet offset
                true, // minimap icon?
                "LOLItems/Resources/npc_sprites/shopkeeper/Handy_Icon", // minimap icon sprite
                true, // in main shop pool?
                1f // chance for main pool
            );

            PrototypeDungeonRoom Mod_Shop_Room = RoomFactory.BuildFromResource("LOLItems/Resources/npc_sprites/shopkeeper/ArmsDealerShop_bigger.room").room;
            RegisterShopRoom(ShopKeeperObj, Mod_Shop_Room, new UnityEngine.Vector2(7.5f, 5f),1.2f); // 1.2
                
            HandyGameObject = ShopKeeperObj;
        }

        public static void RegisterShopRoom(GameObject shop, PrototypeDungeonRoom protoroom, Vector2 vector, float m_weight = 1)
        {
            protoroom.category = PrototypeDungeonRoom.RoomCategory.NORMAL;
            DungeonPrerequisite[] array = shop.GetComponent<CustomShopController>()?.prerequisites != null ? shop.GetComponent<CustomShopController>().prerequisites : new DungeonPrerequisite[0];
            //Vector2 vector = new Vector2((float)(protoroom.Width / 2) + offset.x, (float)(protoroom.Height / 2) + offset.y);
            protoroom.placedObjectPositions.Add(vector);
            protoroom.placedObjects.Add(new PrototypePlacedObjectData
            {
                contentsBasePosition = vector,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = array,
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = new DungeonPlaceable
                {
                    width = 2,
                    height = 2,
                    respectsEncounterableDifferentiator = true,
                    variantTiers = new List<DungeonPlaceableVariant>
                    {
                        new DungeonPlaceableVariant
                        {
                            percentChance = 1f,
                            nonDatabasePlaceable = shop,
                            prerequisites = array,
                            materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
                        }
                    }
                }
            });
            RoomFactory.RoomData roomData = new RoomFactory.RoomData
            {
                room = protoroom,
                isSpecialRoom = true,
                category = "SPECIAL",
                specialSubCategory = "WEIRD_SHOP",

            };
            roomData.weight = m_weight;
            RoomFactory.rooms.Add(shop.name, roomData);
            DungeonHandler.Register(roomData);
        }
    }
}