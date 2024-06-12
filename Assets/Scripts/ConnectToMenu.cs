using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;

public class ConnectToMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text connectingStatus;
    private void Start()
    {
        connectingStatus.text = "Connecting";
        // Automatically connect to Photon server if not connected
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Callback when connected to Photon server
    public override void OnConnectedToMaster()
    {
        connectingStatus.text = "Connected To Server";
        ChangeTextColor(Color.blue);
        Debug.Log("Connected to Photon server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        connectingStatus.text = "Joined To Lobby";
        ChangeTextColor(Color.green);
        Debug.Log("Lobby Joined");
        SceneManager.LoadScene("MenuScene");
    }

    public void ChangeTextColor(Color newColor)
    {
        if (connectingStatus != null)
        {
            connectingStatus.color = newColor;
        }
        else
        {
            Debug.LogError("TMP_Text reference is not set.");
        }
    }
}
