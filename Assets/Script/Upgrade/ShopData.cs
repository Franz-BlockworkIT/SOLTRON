using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShopSystem
{
    [System.Serializable]
    public class ShopData
    {
        public int selectedIndex;
        public ShopItem[] shopItems;
    }


    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public bool isUnlocked;
        public int unlockCost;
        public int unlockedLevel;

        public BikeInfo[] bikeLevel;
    }

    [System.Serializable]
    public class BikeInfo
    {
        public int unlockCost;
        public int Engine;
        public int Rocket;
        public int Tyres;
        public int EMP;
        public int Armour;
    }
}