using AOT;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class CryptoReceiver : MonoBehaviour
{
    public static CryptoReceiver CR;
    public List<string> myNFTMints;
    public List<SPLToken> myTokens;
    public List<CryptoNFT> myNFTs;
    public string pubKey;
    public delegate void ConnectionCallback(System.IntPtr str);
    public string serviceName;
    
    [DllImport("__Internal")]
    private static extern string SolanaLogin(string service, string endpoint, Action<string> cb);
    [DllImport("__Internal")]
    private static extern string GetSolanaWebJS();
    public string walletAddress;
    public string shortAddress;
    public bool isConnected = false;
    [Header("RPC Endpoint")]
    public string endpoint = "https://api.mainnet-beta.solana.com";


    // When the script first loads, it makes sure this is the only running one and destroys any other.
    // It also persists across scenes
    void Awake()
    {
        if (CR != null)
        {
            GameObject.Destroy(CR);
        }
        else
        {
            CR = this;
        }
        DontDestroyOnLoad(this);
        GetSolanaWebJS();
    }

    // This function receives the string sent from the JavaScript function on the WebGL's
    // page and add each string (NFT Mint ID) into a list
    // This list can be accessed anywhere with CryptoReceiver.CR.myTokens
    public void ReceiveNFTMints(string mint)
    {
        if (!myNFTMints.Contains(mint))
        {
            int addLocation = 0;
            while (addLocation < myNFTMints.Count)
            {
                addLocation++;
            }
            myNFTMints.Add(mint);
            Debug.Log("Added: " + mint);
        }
    }

    // This function receives the string sent from the JavaScript function on the WebGL's
    // page and and parses the string into sections divided by the "|" character as the function only allows 1 string
    // It then populates a list with CryptoNFT objects which contain common NFT data
    public void ReceiveNFT(string data)
    {
        // Split up the NFT metadata
        string[] nftdata = data.Split('¬');
        string dataname = (nftdata[0]);
        string datasprite = (nftdata[1]);
        string datacontent = (nftdata[2]);
        string dataattributes = (nftdata[3]);
        // Instantiate a new CryptoNFT object
        CryptoNFT newNFT = CryptoNFT.CreateInstance<CryptoNFT>();
        // Handle NFT Attributes
        List<NFTAttribute> attributeList = new List<NFTAttribute>();
        string[] attributeString = dataattributes.Split('|');
        if (attributeString[0] != "")
        {
            foreach (string a in attributeString)
            {
                string[] att = a.Split('~');
                NFTAttribute newAttribute = NFTAttribute.CreateInstance<NFTAttribute>();
                newAttribute.attributeName = att[0];
                newAttribute.attributeValue = att[1];

                int newLocation = 0;
                while (newLocation < attributeList.Count)
                {
                    newLocation++;
                }
                attributeList.Add(newAttribute);
            }
        }

        newNFT.nftName = dataname;
        newNFT.spritelink = datasprite;
        newNFT.description = datacontent;
        newNFT.attributes = attributeList;

        // Add the instantiated CryptoNFT to the myNFTs List
        int addLocation = 0;
        while (addLocation < myNFTs.Count)
        {
            addLocation++;
        }
        myNFTs.Add(newNFT);
    }

    public void ReceiveSPLTokens(string data)
    {
        // Split up the SPL Token data
        string[] spldata = data.Split('|');
        string datamint = (spldata[0]);
        string dataamount = (spldata[1]);
        string datadecimals = (spldata[2]);
        // Instantiate a new SPL Token object
        SPLToken newToken = SPLToken.CreateInstance<SPLToken>();

        newToken.mint = datamint;
        newToken.amount = float.Parse(dataamount);
        newToken.decimals = int.Parse(datadecimals);

        Debug.Log(datamint + " " + dataamount + " " + datadecimals);

        // Add the instantiated SPL Token to the myTokens List
        int addLocation = 0;
        while (addLocation < myTokens.Count)
        {
            addLocation++;
        }
        myTokens.Add(newToken);
    }


    public void OnConnect(string service)
    {
        SolanaLogin(service, endpoint, OnConnected);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    public static void OnConnected(string address)
    {
        if (address != null)
        {
            Debug.Log("Received address on callback: " + address);
            CR.walletAddress = address;
            string namestart = address.Substring(0, 4);
            string nameend = address.Substring((address.Length - 4), 4);
            CR.shortAddress = namestart + "...." + nameend;
            CR.isConnected = true;
        }
        else
        {
            Debug.Log("not connected");
            
        }
    }


    public void ClearOnLogout()
    {
        myNFTs.Clear();
        myNFTMints.Clear();
        CR.isConnected = false;
        walletAddress = "";

    }

    public void OnConnectButton(string service)
    {
        serviceName = service;
        
        CryptoReceiver.CR.OnConnect(service);
    }
    
    

}