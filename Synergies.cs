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

            //Frozen Heart
            NewSynergy(LOLItems.Synergy.ICE_TO_THE_CORE, "Ice to the core", new[] { IName(FrozenHeart.ItemName) }, new[] { "frost_bullets", "snowballets", "heart_of_ice" });
            NewSynergy(LOLItems.Synergy.FROZEN_BULLETS, "Frozen Bullets", new[] { IName(FrozenHeart.ItemName) }, new[] { "cold_45", "frost_giant", "freeze_ray", "glacier", "snowballer"});

            //Galeforce
            NewSynergy(LOLItems.Synergy.GALEFORCE_FOUR, "FOUR!", new[] { IName(Galeforce.ItemName), Whisper.internalName });
            NewSynergy(LOLItems.Synergy.BOW_MASTERY, "Bow Mastery", new[] { IName(Galeforce.ItemName) }, new[] { "bow", "charmed_bow", "gunbow" });

            //Rod of Ages
            NewSynergy(LOLItems.Synergy.TRAINING_UP, "Training up!", new[] { IName(RodOfAges.ItemName), "macho_brace" });
            NewSynergy(LOLItems.Synergy.AGE_OLD_WISDOM, "Age old wisdom", new[] { IName(RodOfAges.ItemName) }, new[] { "old_knights_shield", "old_knights_helm", "old_knights_flask" });

            //Electric Rifle
            // not sure how this synergy should work with this type of setup, shouldn't cause much issues if i keep it the old setup method
            //NewSynergy(LOLItems.Synergy.PASSIVE_CHARGE, "Passive charge", new[] { IName(ElectricRifle.internalName) }, new[] { "battery_bullets", "shock_rounds", "thunderclap", "shock_rifle", "laser_lotus" });

            //WEAPONS
            //Crossblade
            NewSynergy(LOLItems.Synergy.BOUNCEMAXXING, "Bouncemaxxing", new[] { Crossblade.internalName }, new[] { "bouncy_bullets", "boomerang" });

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
        TRAINING_UP,
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

    };
}
