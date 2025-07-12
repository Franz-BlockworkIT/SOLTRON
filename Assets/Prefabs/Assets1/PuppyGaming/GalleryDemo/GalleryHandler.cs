using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;
using UnityEngine.UI;

public class GalleryHandler : MonoBehaviour
{
    public ArtNFTPrefab artnftPrefab;
    public Transform[] artLocations;
    public Transform overflow;
    public GameObject playerObject;
    public GameObject loginPanel;
    public GameObject previewCamera;


    void Awake()
    {
        for (int i = 0; i < artLocations.Length; i++)
        {
            if (artLocations[i].childCount > 0) GameObject.Destroy(artLocations[i].GetChild(0).gameObject);
        }
    }
    void Update()
    {
        if (CryptoReceiver.CR.isConnected)
        {
            playerObject.SetActive(true);
            loginPanel.SetActive(false);
            previewCamera.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.R)) RefreshNFTList();
    }
    public void RefreshNFTList()
    {

        for (int i = 0; i < artLocations.Length; i++)
        {
            if (artLocations[i].childCount > 0) GameObject.Destroy(artLocations[i].GetChild(0).gameObject);
        }
        StartCoroutine(RefreshNFT());
    }
    IEnumerator RefreshNFT()
    {
        // For each NFT in the myNFTs list we create a new card to display our NFT
        var ownedNFT = CryptoReceiver.CR.myNFTs;
        for (int i = 0; i < ownedNFT.Count; i++)
        {
            if (i >= artLocations.Length)
            {
                yield return new WaitForSeconds(0.3f);
                ArtNFTPrefab item = Instantiate(artnftPrefab);
                item.transform.SetParent(overflow.transform, false);
                item.nftImageString = ownedNFT[i].spritelink;
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                ArtNFTPrefab item = Instantiate(artnftPrefab);
                item.transform.SetParent(artLocations[i].transform, false);
                item.nftImageString = ownedNFT[i].spritelink;
            }
        }
    }
}
