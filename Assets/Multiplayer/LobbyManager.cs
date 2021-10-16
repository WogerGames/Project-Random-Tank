using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string sceneNameToLoad;

    [Space(3)]
    [SerializeField] byte maxPlayers = 10;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] Button btnConnect;
    [SerializeField] TMP_InputField inputField;

    ExitGames.Client.Photon.Hashtable roomProps;


    void Awake()
    {
        //inputField.text = maxPlayers.ToString();
        //CreatePlayer();
        OnChangeTestMode();

        // Первоначальные настройки клиента, тупо спизжено из тутора
        if (PhotonNetwork.NickName == string.Empty)
        {
            PhotonNetwork.NickName = "Пидор_" + Random.Range(100, 999);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";

        SceneManager.GetActiveScene();

        PhotonNetwork.ConnectUsingSettings();
        // ________________________________________________________
        roomProps = new ExitGames.Client.Photon.Hashtable();
        
        roomProps["idi_naxyi"] = PhotonNetwork.GameVersion;

        btnConnect.onClick.AddListener(JoinRoom);

    }

    public static string GetNickname()
    {
        return PhotonNetwork.NickName;
    }

    public override void OnConnectedToMaster()
    {
        Log("Некий хуежуй: " + PhotonNetwork.NickName + " присоеденился к пиздатой игруле");
    }

    public override void OnConnected()
    {
        Log("шо блять?");
    }

    void CreateRoom()
    {  
        string[] props = new string[1];
        props[0] = "idi_naxyi";

        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions
        {
            MaxPlayers = maxPlayers,
            CleanupCacheOnLeave = false,
            //CustomRoomProperties = roomProps,
            //CustomRoomPropertiesForLobby = props
        });
    }

    public void JoinRoom()
    {
        print(PhotonNetwork.CountOfRooms);
        print(PhotonNetwork.CurrentRoom);
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRandomRoom(roomProps, maxPlayers);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        print("Припиздяшил, Уебок " + PhotonNetwork.NickName);
        
        PhotonNetwork.LoadLevel(sceneNameToLoad);
        
    }

    public void OnChangedMaxPlayers()
    {
        //if (inputField.text.Length > 0)
        //{
        //    maxPlayers = byte.Parse(inputField.text);
        //}
    }

    public void OnChangeTestMode()
    {
        //GameManager.testMode = toggle.isOn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Log(string msg)
    {
        logText.text += "\n";
        logText.text += msg;
    }

    
}
