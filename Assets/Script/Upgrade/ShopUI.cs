* Author: Hafiz Saad Khawar
 * Description: 
 * This ShopUI script manages the in-game shop interface for upgrading bikes.
 * It uses a local currency named "SolBal" (representing Solana balance) stored in PlayerPrefs.
 * The SolBal is used to purchase upgrades like engine, tyres, rockets, EMP, and armour.
 * Note: The SolBal here is a local in-game currency and is not directly connected to the Solana blockchain.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ShopSystem
{
    
    public class ShopUI : MonoBehaviour
    {
        public int totalSolBal;
        public ShopData shopData;
        public SaveLoadData saveLoadData;

        
        public Text unlockBtnText, upgrageText, levelText, UsernameText;
        public Text engineText, tyresText, rocketsText, empText, armourText;
        public Text totalCoinsText;
        public Button unlockBtn, upgradeBtn;

        private int currentIndex = 0;
        private int selectedIndex = 0;

        private void Start()
        {

            
            totalSolBal = PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal);

            PlayerScores.playerSolBal = totalSolBal;

            saveLoadData.Initialized();
            selectedIndex = shopData.selectedIndex;
            currentIndex = selectedIndex;
            totalCoinsText.text = "SolBal : " + totalSolBal;
            SetBikeInfo();
            BikeSaveData();

            //unlockBtn.onClick.AddListener(() => UnlockSelectBtnMethod());
            upgradeBtn.onClick.AddListener(() => UpgradeBtnMethod());
            

            

            //UnlockBtnStatus();
            UpgradeBtnStatus();
        }

        void SetBikeInfo()
        {
            int currentLevel = PlayerPrefs.GetInt(PlayerScores.playerName + "CurrentLevel",PlayerScores.playerBikeLevel);
            PlayerScores.playerBikeLevel = currentLevel;
            levelText.text = "Level : " + (currentLevel + 1);
            engineText.text = "Engine : " + PlayerPrefs.GetInt(PlayerScores.playerName + "Engine",40);
            tyresText.text = "Tyres : " + PlayerPrefs.GetInt(PlayerScores.playerName + "Tyres",200);
            rocketsText.text = "Rockets : " + PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket",30);
            empText.text = "EMP : " + PlayerPrefs.GetInt(PlayerScores.playerName + "EMP",35);
            armourText.text = "Armour: " + PlayerPrefs.GetInt(PlayerScores.playerName + "Health",120);
        }

        private void Update()
        {
            SetBikeInfo();

            BikeSaveData();
        }

        //void UnlockSelectBtnMethod()
        //{
        //    bool selected = false;
        //    if (shopData.shopItems[currentIndex].isUnlocked)
        //    {
        //        selected = true;
        //    }
        //    else
        //    {
        //        if(totalCoins >= shopData.shopItems[currentIndex].unlockCost)
        //        {
        //            totalCoins -= shopData.shopItems[currentIndex].unlockCost;
        //            totalCoinsText.text = "Sol Balance : " + totalCoins;
        //            selected = true;
        //            shopData.shopItems[currentIndex].isUnlocked = true;
        //            UnlockBtnStatus();
        //        }
        //    }
        //    if (selected)
        //    {
        //        unlockBtnText.text = "Selected";
        //        selectedIndex = currentIndex;
        //        shopData.selectedIndex = selectedIndex;
        //        unlockBtn.interactable = false;
        //    }
        //}
        void UpgradeBtnMethod()
        {
            int nextLevelIndex = shopData.shopItems[currentIndex].unlockedLevel + 1;

            if(totalSolBal >= shopData.shopItems[currentIndex].bikeLevel[nextLevelIndex].unlockCost)
            {
                totalSolBal -= shopData.shopItems[currentIndex].bikeLevel[nextLevelIndex].unlockCost;
                PlayerPrefs.SetInt(PlayerScores.playerName + "TotalCoins", totalSolBal);
                totalCoinsText.text = "Sol Bal" + totalSolBal;
                shopData.shopItems[currentIndex].unlockedLevel++;

                if(shopData.shopItems[currentIndex].unlockedLevel < shopData.shopItems[currentIndex].bikeLevel.Length - 1)
                {
                    upgrageText.text = "Upgrade : " + shopData.shopItems[currentIndex].bikeLevel[nextLevelIndex + 1].unlockCost;
                }
                else
                {
                    upgradeBtn.interactable = false;
                    upgrageText.text = "Max Level Reached";
                }

                saveLoadData.SaveData();
                BikeSaveData();
                SetBikeInfo();
            }
        }

        //void UnlockBtnStatus() {
        //    if (shopData.shopItems[currentIndex].isUnlocked)
        //    {
        //        unlockBtn.interactable = selectedIndex != currentIndex;
        //        unlockBtnText.text = selectedIndex == currentIndex ? "Selected" : "Select";
        //    }
        //    else
        //    {
        //        unlockBtn.interactable = true;
        //        unlockBtnText.text = "Cost " + shopData.shopItems[currentIndex].unlockCost;
        //    }
        //}
        
        void UpgradeBtnStatus() {

            if (shopData.shopItems[currentIndex].isUnlocked)
            {
                if (shopData.shopItems[currentIndex].unlockedLevel < shopData.shopItems[currentIndex].bikeLevel.Length - 1)
                {
                    int nextLevelIndex = shopData.shopItems[currentIndex].unlockedLevel + 1;

                    upgradeBtn.interactable = true;
                    upgrageText.text = "Upgrade : " + shopData.shopItems[currentIndex].bikeLevel[nextLevelIndex + 1].unlockCost;
                }
                else
                {
                    upgradeBtn.interactable = false;
                    upgrageText.text = "Max Level Reached";
                }
            }
            else
            {
                upgradeBtn.interactable = false;
                upgrageText.text = "Locked";
            }
        }

        void BikeSaveData()
        {
            int currentLevel = PlayerPrefs.GetInt(PlayerScores.playerName + "CurrentLevel",PlayerScores.playerBikeLevel);
            PlayerScores.playerBikeLevel = currentLevel;
            PlayerPrefs.SetInt(PlayerScores.playerName + "CurrentLevel", shopData.shopItems[currentIndex].unlockedLevel);
            PlayerPrefs.SetInt(PlayerScores.playerName + "Engine" ,shopData.shopItems[currentIndex].bikeLevel[currentLevel].Engine);
            PlayerPrefs.SetInt(PlayerScores.playerName + "Tyres", shopData.shopItems[currentIndex].bikeLevel[currentLevel].Tyres);
            PlayerPrefs.SetInt(PlayerScores.playerName + "Rocket",shopData.shopItems[currentIndex].bikeLevel[currentLevel].Rocket);
            PlayerPrefs.SetInt(PlayerScores.playerName + "EMP", shopData.shopItems[currentIndex].bikeLevel[currentLevel].EMP);
            PlayerPrefs.SetInt(PlayerScores.playerName + "Health", shopData.shopItems[currentIndex].bikeLevel[currentLevel].Armour);
        }
    }
}
