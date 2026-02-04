using Alexandria.ItemAPI;
using GungeonCOTL;
using GungeonCOTL.passive_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// maybe move synergies in here later (MANAMUNE)

namespace GungeonCOTL
{
    public static class GungeonCOTLSynergies
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
            return ETGModCompatibility.ExtendEnum<T>(Plugin.ITEM_PREFIX.ToUpper(), s);
        }

        public static void Init()
        {
            /* NOTE:
                - Each synergy entry below must have a comment before the line and the name of the synergy as the first quoted string in the following line
                  This is to ensure that our automatic item tips generation script parses it correctly.
            */

            //example: NewSynergy(LOLItems.Synergy., "", new[] { IName(MainItem.ItemName) }, new[] { });

            NewSynergy(GungeonCOTL.Synergy.HEARTOFTHEFAITHFUL_TWO, "Heart of the Faithful II", new[] { IName(HeartOfTheFaithful1.ItemName), IName(HeartOfTheFaithful2.ItemName) });

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

        private static string IName(string itemName) => Plugin.ITEM_PREFIX + ":" + itemName.ToID();
        public static CustomSynergyType Synergy(this Synergy synergy) => _Synergies[(int)synergy];
        public static string SynergyName(this Synergy synergy) => _SynergyNames[(int)synergy];
        public static bool HasSynergy(this PlayerController player, Synergy synergy) => player.ActiveExtraSynergies.Contains((int)_SynergyIds[(int)synergy]);

        
    }

    public enum Synergy
    {
        // Synergies
        HEARTOFTHEFAITHFUL_TWO,
        MIGHTOFTHEDEVOUT_TWO,
        MIGHTOFTHEDEVOUT_THREE,
        MIGHTOFTHEDEVOUT_FOUR,
        MIGHTOFTHEDEVOUT_FIVE,
    };
}
