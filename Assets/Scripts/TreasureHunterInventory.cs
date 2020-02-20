using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreasureHunterInventory : MonoBehaviour
{
   [Serializable]
   public class CollectibleNumberDict : SerializableDictionary<collectible, int> {}
   public CollectibleNumberDict collectibleAmount;

}

