using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArtNFTPrefab : MonoBehaviour
{
    // This script hold all the Instanciated NFTPrefabs data in the Demo cards
    public string nftImageString;
    public Renderer rend;
    public Sprite defaultSprite;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (nftImageString.Contains("gif")) return;
        if (nftImageString.Contains("mp4")) return;
        // Once this instantiated NFT is laoded, we download the NFTs image
        StartCoroutine(DownloadImage(nftImageString));
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
            rend.material.mainTexture = (Texture)webTexture;
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
