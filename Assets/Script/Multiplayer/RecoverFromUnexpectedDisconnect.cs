using System;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class RecoverFromUnexpectedDisconnect : MonoBehaviourPunCallbacks,IConnectionCallbacks
{

    public static RecoverFromUnexpectedDisconnect _inst;
    private LoadBalancingClient loadBalancingClient;
    private AppSettings appSettings;

    public bool shouldReconnect = true;
    bool reconnecting;



    private void Awake()
    {
        if (RecoverFromUnexpectedDisconnect._inst = null)
        {
            RecoverFromUnexpectedDisconnect._inst = this;
        }
        else
        {
            if (RecoverFromUnexpectedDisconnect._inst != this)
            {
                Destroy(this.gameObject);
            }

        }
        DontDestroyOnLoad(this.gameObject);
    }
    public RecoverFromUnexpectedDisconnect(LoadBalancingClient loadBalancingClient, AppSettings appSettings)
    {
        this.loadBalancingClient = loadBalancingClient;
        this.appSettings = appSettings;
        this.loadBalancingClient.AddCallbackTarget(this);
    }

    ~RecoverFromUnexpectedDisconnect()
    {
        this.loadBalancingClient.RemoveCallbackTarget(this);
    }

    void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
    {

        if (!shouldReconnect)
            return;
        if (this.CanRecoverFromDisconnect(cause))
        {
            this.Recover();
        }
    }

    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            // the list here may be non exhaustive and is subject to review
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    private void Recover()
    {
        if (!loadBalancingClient.ReconnectAndRejoin())
        {
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            if (!loadBalancingClient.ReconnectToMaster())
            {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!loadBalancingClient.ConnectUsingSettings(appSettings))
                {
                    Debug.LogError("ConnectUsingSettings failed");
                }
            }
            else
            {
                Debug.Log("Reconnect to Master Success!");
            }

        }
        else
        {
            Debug.Log("ReconnectAndRejoin (Master Server) Success");
            reconnecting = true;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        
        if (reconnecting)
            Debug.LogError("Reconnection - OnJoinRoomFailed");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        
        Debug.LogError("Reconnection : OnJoinRandomFailed");
    }

    public override void OnJoinedRoom()
    {
        reconnecting = false;
        Debug.Log("Reconnectin : OnJoinRoom");
    }

     
}