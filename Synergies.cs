using Alexandria.ItemAPI;
using LOLItems.active_items;
using LOLItems.passive_items;
using LOLItems.weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// maybe move synergies in here later (MANAMUNE)

namespace LOLItems
{
    public static class LOLItemsSynergies
    {
        private static int _NUM_SYNERGIES = Enum.GetNames(typeof(Synergy)).Length;                                              // number of synergies our mod adds
        public static List<CustomSynergyType> _Synergies = Enumerable.Repeat<CustomSynergyType>(0, _NUM_SYNERGIES).ToList();    // list of the actual new synergies
        public static List<string> _SynergyNames = Enumerable.Repeat<string>(null, _NUM_SYNERGIES).ToList();                    // list of friendly names of synergies
        public static List<string> _SynergyEnums = new(Enum.GetNames(typeof(Synergy)));                                         // list of enum names of synergies
        public static List<int> _SynergyIds = Enumerable.Repeat<int>(0, _NUM_SYNERGIES).ToList();                               // list of ids of synergies in the AdvancedSynergyEntry database
        
        //NOTE: needs to be done early so guns and items can reference synergy ids properly
        public static void InitEnums()
        {
            // Extend the base game's CustomSynergyType enum to make room for our new synergy
            for (int i = 0; i < _SynergyNames.Count; ++i)
                _Synergies[i] = _SynergyEnums[i].ExtendEnum<CustomSynergyType>();
        }
        public static T ExtendEnum<T>(this string s) where T : System.Enum
        {
            return ETGModCompatibility.ExtendEnum<T>("LOLItems".ToUpper(), s);
        }

        public static void Init()
        {
            /* NOTE:
                - Each synergy entry below must have a comment before the line and the name of the synergy as the first quoted string in the following line
                  This is to ensure that our automatic item tips generation script parses it correctly.
            */

            #region Synergies
            //example: NewSynergy(LOLItems.Synergy., "", new[] { IName(MainItem.ItemName) }, new[] { });

            //PASSIVE ITEMS ======================================================================================================================================================================================================================================
            //Blade of the Ruined King
            NewSynergy(LOLItems.Synergy.YOU_DARE_FACE_A_KING, "You dare face a king?!", new[] { IName(BladeOfTheRuinedKing.ItemName) }, new[] { "crown_of_guns", "gilded_bullets", "coin_crown"});
            NewSynergy(LOLItems.Synergy.FOR_ISOLDE, "For Isolde!", new[] { IName(BladeOfTheRuinedKing.ItemName) }, new[] { "excaliber", "blasphemy"});
            //NewSynergy(LOLItems.Synergy.BORK3, "3", new[] { IName(BladeOfTheRuinedKing.ItemName), IName(GuinsoosRageblade.ItemName) });

            //Cloak of Starry Night
            NewSynergy(LOLItems.Synergy.HEAVEN_AND_EARTH_COMBINED, "Heaven and Earth Combined", new[] { IName(CloakOfStarryNight.ItemName), IName(ShieldOfMoltenStone.ItemName)});

            //Collector
            NewSynergy(LOLItems.Synergy.RETURN_ON_INVESTMENT, "Return on Investment", new[] { IName(Collector.ItemName) }, new[] { "loot_bag", "briefcase_of_cash" });
            NewSynergy(LOLItems.Synergy.STROKE_OF_LUCK, "Stroke of Luck", new[] { IName(Collector.ItemName), "fortunes_favor" });
            NewSynergy(LOLItems.Synergy.AN_OFFERING, "An offering", new[] { IName(Collector.ItemName), "daruma" });
            NewSynergy(LOLItems.Synergy.BETTER_RNG, "Better RNG", new[] { IName(Collector.ItemName), "chance_bullets" });

            //Cull
            NewSynergy(LOLItems.Synergy.WEAK_EARLY_GAME, "weak early game...", new[] { IName(Cull.ItemName), "unfinished_gun" });
            NewSynergy(LOLItems.Synergy.BAUSEN_LAW, "Bausen Law", new[] { IName(Cull.ItemName), "huntsman" });

            //Experimental Hexplate
            NewSynergy(LOLItems.Synergy.THAT_GOOD_SHIT, "that GOOD $#%&", new[] { IName(ExperimentalHexplate.ItemName), "cigarettes" });
            NewSynergy(LOLItems.Synergy.SPEED_BLITZ, "Speed Blitz", new[] { IName(ExperimentalHexplate.ItemName) }, new[] { "bionic_leg", "shotgun_coffee", "shotga_cola", "ballistic_boots", "magic_sweet" });
            NewSynergy(LOLItems.Synergy.FILLER_UP, "Fill'Er-Up", new[] { IName(ExperimentalHexplate.ItemName), "gungine" });

            //Frozen Heart
            NewSynergy(LOLItems.Synergy.ICE_TO_THE_CORE, "Ice to the core", new[] { IName(FrozenHeart.ItemName) }, new[] { "frost_bullets", "snowballets", "heart_of_ice" });
            NewSynergy(LOLItems.Synergy.FROZEN_BULLETS, "Frozen Bullets", new[] { IName(FrozenHeart.ItemName) }, new[] { "cold_45", "frost_giant", "freeze_ray", "glacier", "snowballer"});

            //Guardian Angel
            NewSynergy(LOLItems.Synergy.DIVINE_JUDGEMENT, "Divine Judgement!", new[] { IName(GuardianAngel.ItemName), VirtueForm3.internalName });
            NewSynergy(LOLItems.Synergy.WHY_WONT_YOU_DIE, "Why won't you DIE!", new[] { IName(GuardianAngel.ItemName) }, new[] { "clone", "gun_soul", "pig" });

            //Guinsoo's Rageblade
            NewSynergy(LOLItems.Synergy.BLADES_OF_CHAOS, "Blades of Chaos", new[] { IName(GuinsoosRageblade.ItemName)}, new[] { "chaos_ammolet", "chaos_bullets" });
            NewSynergy(LOLItems.Synergy.POSEIGUNS_WRATH, "Poseigun's Wrath", new[] { IName(GuinsoosRageblade.ItemName), "trident" });
            NewSynergy(LOLItems.Synergy.TRIPLE_DELUXE, "TRIPLE DELUXE", new[] { IName(GuinsoosRageblade.ItemName), IName(BladeOfTheRuinedKing.ItemName), IName(KrakenSlayer.ItemName) });
            //NewSynergy(LOLItems.Synergy.ONHIT_SYNERGY_WITH_KRAKENSLAYER, "On-hit synergy", new[] { IName(GuinsoosRageblade.ItemName), IName(KrakenSlayer.ItemName) });

            //Rod of Ages
            NewSynergy(LOLItems.Synergy.SUPER_TRAINING, "Super Training", new[] { IName(RodOfAges.ItemName), "macho_brace" });
            NewSynergy(LOLItems.Synergy.AGE_OLD_WISDOM, "Age old wisdom", new[] { IName(RodOfAges.ItemName) }, new[] { "old_knights_shield", "old_knights_helm", "old_knights_flask" });

            //ACTIVE ITEMS ==============================================================================================================================================================================================================================================================================
            //Galeforce
            NewSynergy(LOLItems.Synergy.GALEFORCE_FOUR, "FOUR!", new[] { IName(Galeforce.ItemName), Whisper.internalName });
            NewSynergy(LOLItems.Synergy.BOW_MASTERY, "Bow Mastery", new[] { IName(Galeforce.ItemName) }, new[] { "bow", "charmed_bow", "gunbow" });

            //Refillable Potion
            NewSynergy(LOLItems.Synergy.BUNCH_O_POTIONS, "BunchO Potions", new[] { IName(RefillablePotion.ItemName), "old_knights_flask" });
            NewSynergy(LOLItems.Synergy.COCKTAIL_POTION, "Cocktail Potion", new[] { IName(RefillablePotion.ItemName) }, new[] { "potion_of_lead_skin", "potion_of_gun_friendship" });

            //WEAPONS ====================================================================================================================================================================================================================================================
            //Crossblade
            NewSynergy(LOLItems.Synergy.BOUNCEMAXXING, "Bouncemaxxing", new[] { Crossblade.internalName }, new[] { "bouncy_bullets", "boomerang" });
            
            //Electric Rifle
            // not sure how this synergy should work with this type of setup, shouldn't cause much issues if i keep it the old setup method
            //NewSynergy(LOLItems.Synergy.PASSIVE_CHARGE, "Passive charge", new[] { IName(ElectricRifle.internalName) }, new[] { "battery_bullets", "shock_rounds", "thunderclap", "shock_rifle", "laser_lotus" });

            //Hextech Rifle
            NewSynergy(LOLItems.Synergy.ME_MISS_NOT_BY_A_LONG_SHOT, "\"Me, miss? Not by a long shot.\"", new[] { HextechRifle.internalName }, new[] { "scope", "laser_sight" });

            //Virtue
            NewSynergy(LOLItems.Synergy.EXP_SHARE_FORM_1, "EXP. Share", new[] { VirtueForm1.internalName }, new[] { "macho_brace", "scouter", "life_orb" });
            NewSynergy(LOLItems.Synergy.EXP_SHARE_FORM_2, "EXP. Share", new[] { VirtueForm2.internalName }, new[] { "macho_brace", "scouter", "life_orb" });
            NewSynergy(LOLItems.Synergy.EXP_SHARE_FORM_3, "EXP. Share", new[] { VirtueForm3.internalName }, new[] { "macho_brace", "scouter", "life_orb" });

            #endregion
        }

        private static AdvancedSynergyEntry NewSynergy(Synergy synergy, string name, string[] mandatory, string[] optional = null, bool ignoreLichEyeBullets = false)
        {
            // Get the enum index of our synergy
            int index = (int)synergy;
            // Register the AdvancedSynergyEntry so that the game knows about it
            AdvancedSynergyEntry ase = RegisterSynergy(_Synergies[index], name, mandatory.ToList(), (optional != null) ? optional.ToList() : null, ignoreLichEyeBullets);
            // Index the friendly name of our synergy
            _SynergyNames[index] = name;
            // Get the actual ID of our synergy entry in the AdvancedSynergyDatabase, which doesn't necessarily match the CustomSynergyType enum
            _SynergyIds[index] = GameManager.Instance.SynergyManager.synergies.Length - 1;
            // Return the AdvancedSynergyEntry
            return ase;
        }

        public static AdvancedSynergyEntry RegisterSynergy(CustomSynergyType synergy, string name, List<string> mandatoryConsoleIDs, List<string> optionalConsoleIDs = null, bool ignoreLichEyeBullets = false)
        {
            List<int> itemIDs = new();
            List<int> gunIDs = new();
            List<int> optItemIDs = new();
            List<int> optGunIDs = new();
            foreach (var id in mandatoryConsoleIDs)
            {
                PickupObject pickup = Gungeon.Game.Items[id];
                if (pickup && pickup.GetComponent<Gun>())
                    gunIDs.Add(pickup.PickupObjectId);
                else if (pickup && (pickup.GetComponent<PlayerItem>() || pickup.GetComponent<PassiveItem>()))
                    itemIDs.Add(pickup.PickupObjectId);
            }

            if (optionalConsoleIDs != null)
            {
                foreach (var id in optionalConsoleIDs)
                {
                    PickupObject pickup = Gungeon.Game.Items[id];
                    if (pickup && pickup.GetComponent<Gun>())
                        optGunIDs.Add(pickup.PickupObjectId);
                    else if (pickup && (pickup.GetComponent<PlayerItem>() || pickup.GetComponent<PassiveItem>()))
                        optItemIDs.Add(pickup.PickupObjectId);
                }
            }

            // Add our synergy's name to the string manager so it displays properly when activated
            string nameKey = $"#{name.ToID().ToUpperInvariant()}";
            ETGMod.Databases.Strings.Synergy.Set(nameKey, name);

            AdvancedSynergyEntry entry = new AdvancedSynergyEntry()
            {
                NameKey = nameKey,
                MandatoryItemIDs = itemIDs,
                MandatoryGunIDs = gunIDs,
                OptionalItemIDs = optItemIDs,
                OptionalGunIDs = optGunIDs,
                bonusSynergies = new() { synergy },
                statModifiers = new List<StatModifier>(),
                IgnoreLichEyeBullets = ignoreLichEyeBullets,
            };

            int oldLength = GameManager.Instance.SynergyManager.synergies.Length;
            Array.Resize(ref GameManager.Instance.SynergyManager.synergies, oldLength + 1);
            GameManager.Instance.SynergyManager.synergies[oldLength] = entry;
            return entry;
        }

        private static string IName(string itemName) => "LOLItems:" + itemName.ToID();
        public static CustomSynergyType Synergy(this Synergy synergy) => _Synergies[(int)synergy];
        public static string SynergyName(this Synergy synergy) => _SynergyNames[(int)synergy];
        public static bool HasSynergy(this PlayerController player, Synergy synergy) => player.ActiveExtraSynergies.Contains((int)_SynergyIds[(int)synergy]);

        
    }

    public enum Synergy
    {
        // Synergies
        RETURN_ON_INVESTMENT,
        STROKE_OF_LUCK,
        AN_OFFERING,
        BETTER_RNG,
        HEAVEN_AND_EARTH_COMBINED,
        ICE_TO_THE_CORE,
        FROZEN_BULLETS,
        GALEFORCE_FOUR,
        BOW_MASTERY,
        SUPER_TRAINING,
        AGE_OLD_WISDOM,
        //PASSIVE_CHARGE,
        YOU_DARE_FACE_A_KING,
        FOR_ISOLDE,
        //BORK3,
        BOUNCEMAXXING,
        ME_MISS_NOT_BY_A_LONG_SHOT,
        EXP_SHARE_FORM_1,
        EXP_SHARE_FORM_2,
        EXP_SHARE_FORM_3,
        WEAK_EARLY_GAME,
        BAUSEN_LAW,
        THAT_GOOD_SHIT,
        SPEED_BLITZ,
        FILLER_UP,
        DIVINE_JUDGEMENT,
        WHY_WONT_YOU_DIE,
        BUNCH_O_POTIONS,
        COCKTAIL_POTION,
        BLADES_OF_CHAOS,
        POSEIGUNS_WRATH,
        TRIPLE_DELUXE,
        //ONHIT_SYNERGY_WITH_KRAKENSLAYER,

    };
}
