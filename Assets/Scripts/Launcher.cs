using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Launcher : Photon.PunBehaviour {
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    public byte MaxPlayersPerRoom = 4;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    public Text progressLabel;

    public GameObject launchCam, launchEvent, launchCanvas;

    public bool first = false;
    public Vector3[] spawns = { new Vector3(-2.5f, 0, 0), new Vector3(2.5f, 0, 0) };

    bool isConnecting, waiting, once;
    string _gameVersion = "1";

    void Awake()
    {
        once = true;
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        PhotonNetwork.logLevel = Loglevel;
    }

    // MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {
        progressLabel.enabled = false;
        controlPanel.SetActive(true);
    }

    void Update()
    {
        if (once && waiting && PhotonNetwork.room.PlayerCount == 2)
        {
            // #Critical
            // Load the Room Level. 
            Debug.Log("We load the game.");
            launchCam.SetActive(false);
            launchCanvas.SetActive(false);
            launchEvent.SetActive(false);
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(launchCam);
            DontDestroyOnLoad(launchCanvas);
            DontDestroyOnLoad(launchEvent);
            PhotonNetwork.LoadLevel("Main");
            once = false;
        }
    }

    // Start the connection process. 
    // If already connected, we attempt joining a random room
    // - if not yet connected, Connect this application instance to Photon Cloud Network
    public void Connect()
    {
        waiting = false;
        isConnecting = true;
        progressLabel.enabled = true;
        controlPanel.SetActive(false);
        progressLabel.text = "Connecting...";
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected) {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        } else {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnectedFromPhoton() {
        progressLabel.enabled = false;
        controlPanel.SetActive(true);
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom() {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
        Debug.Log("We wait for our teammate.");
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            waiting = true;
            first = true;
            progressLabel.text = "Waiting for other player...";
        }
    }
}
