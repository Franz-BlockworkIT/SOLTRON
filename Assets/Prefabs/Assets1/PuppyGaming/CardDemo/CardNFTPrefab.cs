using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class CardNFTPrefab : MonoBehaviour
{
    // This script hold all the Instantiated NFTPrefabs data in the Demo cards
    public TMP_Text nftName;
    public Image nftImage;
    public string nftImageString;
    public Texture imagePull;
    public TMP_Text nftDescription;
    public List<NFTAttribute> attributes;
    public AttributePrefab attributePrefab;
    public Transform content;
    public Sprite defaultSprite;
    // Start is called before the first frame update
    void Start()
    {
        CreateAttributes();
        if (nftImageString.Contains("gif") || nftImageString.Contains("mp4") || nftImageString.Contains("glf") || nftImageString == null) return;
        // Once this instantiated NFT is laoded, we download the NFTs image
        StartCoroutine(DownloadImage(nftImageString));
    }

    void CreateAttributes()
    {
        foreach (NFTAttribute n in attributes)
        {
            AttributePrefab item = Instantiate(attributePrefab);
            item.transform.SetParent(content.transform, false);
            item.type.text = n.attributeName;
            item.value.text = n.attributeValue;

            // Debug.Log(n.attributeName + " : " + n.attributeValue + " added to " + nftName.text);
        }
    }

    // This function downloaded the NFT image and converts it to a sprite to use as the nftImage sprite
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture as Texture2D;
            Sprite webSprite = SpriteFromTexture2D(webTexture);
            nftImage.sprite = webSprite;
        }
    }

    Sprite SpriteFromTexture2D(Texture2D texture)
    {
        if (texture == null)
        {
            return defaultSprite;
        }
        else
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

}
