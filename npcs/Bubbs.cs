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
    public static class Bubbs
    {
        public static GenericLootTable ShopKeeperLootTable;
        public static GameObject HandyGameObject;
        public static void Init()
        {
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Everything I say is waterproof! Or if not, water-absorbent!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Perhaps I could tell you about my impending odyssey!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Frugality is not a virtue, I assure you.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Purchase thoroughly! You'll never know when I shove off!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "You look like an aspiring patron of scientific research!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "The sea is the final frontier!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "I've been designing some new fins!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Did you know: clams can smell colours!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Once, people could breathe in water! Then we forgot how.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Whale crabs. Octo-salmon. Barnacle sharks. Who knows what I'll find?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "The helmet never comes off! Well, except when I get hungry.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "I theorize that beneath this ocean, there is another, wetter, ocean!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Undersea exploration isn't a job, it's a privilege!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "I've done several trial runs in local puddles. I'm ready for the big show!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Sea monsters are no more than misunderstood ambassadors of the deep.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Exploring the sea in a boat is like eating a melon by the rind!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Bilgewater is a lovely place—principally when facing out to sea.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "My research indicates that if you hold your breath long enough, you can kick the habit! You just need to stay awake.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "One day, the ocean will be my oyster—and the ocean's oysters will be my breakfast!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Oh, don't get me started on buoyancy.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Gadgetry in Piltover, extinct overlords in Freljord? Hmph! I'll show them!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "There's evidence to suggest that manatees once ruled the land and sea!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "I can't wait to school some fish.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "So many mysteries to explore.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "I can't wait to see what lies beneath!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "The real sunken treasure is aquatic knowledge.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_GENERIC_TALK", "Just as soon as I perfect my submersible, I'll be off!");

            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_STOPPER_TALK", "Soon, very soon...");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_STOPPER_TALK", "Every coin counts.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_STOPPER_TALK", "What mysteries await?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_STOPPER_TALK", "To knowledge!");

            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Perfect!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Delightful!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Impeccable taste!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "My, that is a beauty!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Every little bit helps!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "You've just supported science!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "That is a singular find… oh, but I have more, if you need them!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "It complements you perfectly.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "That is guaranteed for, at a minimum, ballast.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "I've sold four of those today!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "That will spur—or deter—violence swimmingly!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Yeah, I have absolutely no idea what that does. Good luck!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Thank you for contributing to my expedition.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Would you like that wrapped?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Uh, sorry, I haven't gotten around to drying that off!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "I prefer you only killed bad people with this—or nautical sceptics.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "That is, uh… most probably valuable!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "A bare minimum of rust.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "No price is too great for progress.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "That will serve you splendidly.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Keep this up, and I'll be diving in no time.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "You'll receive a special thanks in my paper.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Now that you've got the hang of it, purchase something else!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Why not pick up a gift… for science?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Your gold will be put to good use.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "Now you're part of the adventure!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "No need to explain what you plan to do with that.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "I can tell you're a collector.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_PURCHASE_TALK", "There's simply no better… for whatever that does.");

            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Ah, yes, but science, tragically, doesn’t operate on credit.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "I’d love to fund your dreams, but I can barely fund my own!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "That’s the spirit! The financial spirit, however, is lacking.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Sorry, but seawater doesn’t pay the bills.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "If only enthusiasm were a currency! You’d be rich indeed.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "That price is, regrettably, non-negotiable—even for pioneers.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Oh, don’t worry. Many great expeditions ended before they began.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Perhaps a smaller trinket? To, ah, stay afloat?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Close! Now, just a few more clams in the purse, as they say.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "I accept gold, doubloons, and… well, mostly just gold.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Research may be priceless, but my wares are not.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "I could loan it to you, but then I’d need to repossess it underwater.");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Come now, even barnacles save more diligently!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_NOSALE_TALK", "Ah, a true visionary! Sadly, visions don’t spend well at market.");

            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_INTRO_TALK", "All profits go to cutting-edge research!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_INTRO_TALK", "Have I read you my thesis on sea slugs?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_INTRO_TALK", "Welcome to the dive site!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_INTRO_TALK", "Would you like to join my expedition?");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_INTRO_TALK", "I brew my potions with salt water for that extra zing!");

            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_ATTACKED_TALK", "Uncalled for!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_ATTACKED_TALK", "No discounts!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_ATTACKED_TALK", "Not the equipment!");
            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_ATTACKED_TALK", "Science under siege!");

            ETGMod.Databases.Strings.Core.AddComplex("#BUBBS_STOLEN_TALK", "Robbery is hardly the scientific method!");

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
            
            GameObject BubbsObj = Alexandria.NPCAPI.ShopAPI.SetUpShop(
                "BUBBS", //name
                "LOLItems", //prefix
                new List<string>() //idle
                {
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_idle_01",
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_idle_02",
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_idle_03",
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_idle_04"
                },
                6, //idle fps
                // talk animation moves sprite 1 pixel to the right, try to fix it idk how lmao
                new List<string>() //talk
                {
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_talk_01",
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_talk_02",
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_talk_03",
                    "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_talk_04"
                },
                6, //talk fps
                ShopKeeperLootTable, //table
                Alexandria.NPCAPI.CustomShopItemController.ShopCurrencyType.COINS, //cost

                "#BUBBS_GENERIC_TALK",
                "#BUBBS_STOPPER_TALK",
                "#BUBBS_PURCHASE_TALK",
                "#BUBBS_NOSALE_TALK",
                "#BUBBS_INTRO_TALK",
                "#BUBBS_ATTACKED_TALK",
                "#BUBBS_STOLEN_TALK",
                    
                //talk stuff
                new Vector3(44/16f, 46/16f, 0), //talk point
                new Vector3(3/16f, 48/16f, 0), //NPC position
                Alexandria.NPCAPI.ShopAPI.VoiceBoxes.DOUG,
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
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_carpet",
                new Vector3(0, 0, 0), // carpet offset
                true, // minimap icon?
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_minimap", // minimap icon sprite
                true, // in main shop pool?
                1f // weight for main pool (default 1f)
            );
            
            List<string> purchaseAnimationSpritePathList = new List<string>
            {
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_01",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_02",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_03",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_04",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_05",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_06",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_07",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_08",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_09",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_10",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_11",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_12",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_13",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_14",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_15",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_16",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_17",
                "LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/bubbs_buy_18"
            };

            //ShopAPI.AddUnparentedAnimationToShop(BubbsObj, purchaseAnimationSpritePathList, 6, "purchase");
            ShopAPI.AddAdditionalAnimationsToShop(BubbsObj, purchaseAnimationSpritePathList, 6);

            PrototypeDungeonRoom Mod_Shop_Room = RoomFactory.BuildNewRoomFromResource("LOLItems/Resources/npc_sprites/shopkeeper/bubbs_sprites/test.newroom").room;
            RegisterShopRoom(BubbsObj, Mod_Shop_Room, new UnityEngine.Vector2(7.5f, 5f),1.2f); // 1.2

            GameManager.Instance.GlobalInjectionData.entries[2].injectionData.InjectionData.Add(new ProceduralFlowModifierData()
            {
                annotation = "bubbs",
                placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>()
                {
                    ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN
                },
                exactRoom = Mod_Shop_Room,
                prerequisites = new DungeonPrerequisite[0],
                CanBeForcedSecret = false,
            });

            HandyGameObject = BubbsObj;
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
            roomData.weight = 1f;
            RoomFactory.rooms.Add(shop.name, roomData);
            DungeonHandler.Register(roomData);
        }
    }
}