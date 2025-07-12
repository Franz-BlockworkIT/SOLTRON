using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CardNFTPanelHandler : MonoBehaviour
{
    public CardNFTPrefab nftPrefab;
    public Transform content;
    public GameObject loadingText;
    public GameObject loginPanel;

    void LateUpdate()
    {
        if (!loginPanel.activeInHierarchy) return;
        if (CryptoReceiver.CR.isConnected)
        {
            loginPanel.SetActive(false);
        }
    }
    public void OnConnectButton(string service)
    {
        CryptoReceiver.CR.OnConnect(service);
    }

    public void RefreshNFTList()
    {
        // Display loading panel while istantiating NFTs
        loadingText.SetActive(true);
        // Clear items to refresh to stop duplicates:
        foreach (Transform t in content)
        {
            Destroy(t.gameObject);
        }
        StartCoroutine(RefreshNFT());
    }
    IEnumerator RefreshNFT()
    {
        // For each NFT in the myNFTs list we create a new card to display our NFT
        var ownedNFT = CryptoReceiver.CR.myNFTs;
        for (int i = 0; i < ownedNFT.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            CardNFTPrefab item = Instantiate(nftPrefab);
            item.transform.SetParent(content.transform, false);
            item.nftName.text = ownedNFT[i].nftName;
            item.nftImageString = ownedNFT[i].spritelink;
            item.nftDescription.text = ownedNFT[i].description;
            item.attributes = ownedNFT[i].attributes;

            // This will remove the laoding panel once the last NFT has been instantiated
            if (content.childCount == ownedNFT.Count) loadingText.SetActive(false);
        }

    }
}
