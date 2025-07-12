using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "Puppy Gaming/NFT")]
public class CryptoNFT : ScriptableObject
{
    public string nftName;
    public string spritelink;
    public string description;
    public List<NFTAttribute> attributes;
}
