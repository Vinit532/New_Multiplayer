using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviourPunCallbacks
{
    public TMP_InputField userNameInput;
    public TMP_InputField roomNameInput;

    public GameObject roomButtonPrefab;
    public Transform roomListContent;

    public Transform playerListContent;
    public GameObject playerTextPrefab;

    public GameObject playerListPanel;
    public GameObject roomListPanel;

    public GameObject startGameButton;

    private void Start()
    {
        // Hide the Start Game button initially
        startGameButton.SetActive(false);
    }

    public void CreateRoom()
    {
        string userName = userNameInput.text;
        string roomName = roomNameInput.text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Username and Room Name cannot be empty");
            return;
        }

        PhotonNetwork.NickName = userName;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Clear existing buttons
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new buttons for each room
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList) continue; // Skip rooms that are no longer available

            GameObject roomButton = Instantiate(roomButtonPrefab, roomListContent);
            roomButton.name = roomInfo.Name;

            // Set the room name on the button
            TMP_Text buttonText = roomButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = roomInfo.Name;
            }

            // Add a click listener to join the room
            Button button = roomButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => JoinRoom(roomInfo.Name));
            }
        }
    }

    private void JoinRoom(string roomName)
    {
        string userName = userNameInput.text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Username and Room Name cannot be empty");
            return;
        }

        PhotonNetwork.NickName = userName;
        PhotonNetwork.JoinRoom(roomName);
    }

    public void RefreshRoomList()
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
        }
    }

    public void StartGame()
    {
        // Only the Master Client should load the level to synchronize all players
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(2); // Ensure "2" is the build index of your target scene
        }
        else
        {
            Debug.LogWarning("Only the Master Client can start the game.");
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // Return to the room list view and disable the Start Game button
        playerListPanel.SetActive(false);
        roomListPanel.SetActive(true);
        startGameButton.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        UpdatePlayerList();

        // Show the player list and hide the room list
        playerListPanel.SetActive(true);
        roomListPanel.SetActive(false);

        // Enable the Start Game button if the player is the Master Client
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join Room Failed: " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered: " + newPlayer.NickName);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player Left: " + otherPlayer.NickName);
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Update the Start Game button based on the new master client status
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    private void UpdatePlayerList()
    {
        // Clear existing player texts
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Create a text object for each player
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerText = Instantiate(playerTextPrefab, playerListContent);
            playerText.GetComponent<TMP_Text>().text = player.NickName;
        }
    }
}
