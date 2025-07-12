using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using System;
using UnityEngine.UI;
using System.Globalization;
using TMPro;

public class TransactionUIHandler : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject transactionPanel;
    public TMP_Text destination;
    public TMP_InputField amount;

    [DllImport("__Internal")]
    private static extern string SendSolTransaction(string destination, float amount, Action<bool> callback);

    public static TransactionUIHandler trans;
    public GameObject transactionPopup;
    public Text transactionResult;
    public Text transactionStatus;


    // Start is called before the first frame update
    void Start()
    {
        trans = this;
    }

    void LateUpdate()
    {
        if (!loginPanel.activeInHierarchy) return;
        if (CryptoReceiver.CR.isConnected)
        {
            loginPanel.SetActive(false);
            transactionPanel.SetActive(true);
        }
    }

    public void OnSendTransaction()
    {
        float floatAmount = float.Parse(amount.text, NumberStyles.Any, CultureInfo.CurrentCulture);
        SendSolTransaction(destination.text, floatAmount, OnTransactionCallback);
        transactionStatus.text = "Awaiting confirmation....";
    }

    public void OnConnectButton(string service)
    {
        CryptoReceiver.CR.OnConnect(service);
    }

    [MonoPInvokeCallback(typeof(Action<bool>))]
    public static void OnTransactionCallback(bool result)
    {
        Debug.Log(result);
        trans.transactionPopup.SetActive(true);
        if (result)
        {
            trans.transactionResult.text = ("Success");
            trans.transactionStatus.text = "";
            trans.transactionPanel.SetActive(false);
        }
        else
        {
            trans.transactionResult.text = ("Failed");
        }
    }
}
