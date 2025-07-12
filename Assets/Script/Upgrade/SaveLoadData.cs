using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystem
{
    public class SaveLoadData : MonoBehaviour
    {

        [SerializeField] ShopUI shopUi;
        int total;
        private void Start()
        {
            //SaveData();
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SaveData();
            }
#endif
        }



        private void OnApplicationPause(bool pause)
        {
#if !UNITY_EDITOR
            
                SaveData();
            
#endif
        }
        public void Initialized()
        {
            if (PlayerPrefs.GetInt(PlayerScores.playerName + "GameStarted",0) == 1)
            {
                LoadData();
            }
            else
            {
                SaveData();
                PlayerPrefs.SetInt(PlayerScores.playerName + "GameStarted", 1);
            }
        }
        public void SaveData()
        {
            string shopDataString = JsonUtility.ToJson(shopUi.shopData);
            

            try
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "/" + PlayerScores.playerName + "ShopData.json", shopDataString);
                Debug.Log("Saved");
            }
            catch(System.Exception e)
            {
                Debug.Log("Error Saving Data : " + e);
                throw;
            }
        }

        void LoadData()
        {
            try
            {
                string shopDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + PlayerScores.playerName + "ShopData.json");
                Debug.Log("Loaded");
                shopUi.shopData = new ShopData();
                shopUi.shopData = JsonUtility.FromJson<ShopData>(shopDataString);
            }

            catch(System.Exception e)
            {
                Debug.Log("Error Loading Data" + e);
                throw;
            }
        }

        void ClearData()
        {
            PlayerPrefs.SetInt(PlayerScores.playerName + "GameStarted", 0);
        }
    }
}
