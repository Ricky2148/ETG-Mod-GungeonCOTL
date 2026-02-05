using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using GungeonCOTL.custom_class_data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TieredPassiveItem : PassiveItem
{
    public bool isTieredPassiveItem = true;

    public int itemTier;

    public string TierGroupIdentifier = "";
}
