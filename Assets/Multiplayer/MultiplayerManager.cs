using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using MultiplayerEventData;

public class MultiplayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    [SerializeField] GameObject playerPrefab;

    bool applicationIsQuit;

    Dictionary<EventCode, EventAction> eventActions = new Dictionary<EventCode, EventAction>();

    public static MultiplayerManager Instance;

    public static bool IsMaster => PhotonNetwork.IsMasterClient;
    public static List<Player> Players { get; } = new List<Player>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Больше 1-го MultiplayerManager ");
        }
        Instance = this;

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }

        
    }

    void Start()
    {
        //StandOfPlayer[] stands = FindObjectsOfType<StandOfPlayer>();

        //Vector3 playerPos = new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(-3, 3));
        SpawnPlayer();
        
        PhotonPeer.RegisterType(typeof(EventHpContent), 012, SerializeEventHpContent, DeserializeEventHpContent);
        PhotonPeer.RegisterType(typeof(FirePhotonData), 011, SerializeEventTurnContent, DeserializeEventTurnContent);
        PhotonPeer.RegisterType(typeof(EnemyHealthPoint), 015, SerializeHealthPoint, DeserializeEnemyHealthPoint);
        //PhotonPeer.RegisterType(typeof(CreateDiceParams), 017, SerializeCreateDiceParams, DeserializeCreateDiceParams);

    }

    public GameObject SpawnPlayer()
    {
        return PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, Quaternion.identity);
    }

    public static void AddPlayer(Player player) => Players.Add(player);
    

    public static T Spawn<T>(T prefab, Vector3 position)
    {
        var prefabName = (prefab as MonoBehaviour).name;
        var spawned = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);

        return spawned.GetComponent<T>();
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position)
    {
        return PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
    }

    public static void DestroyByPhoton(GameObject target)
    {
        PhotonNetwork.Destroy(target);
    }

    public static void RaiseEvent<T>(EventCode eventCode, T data, int targetActor)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        option.TargetActors = new int[] { targetActor };
        PhotonNetwork.RaiseEvent((byte)eventCode, data, option, sendOptions);
    }

    public static void RaiseEvent<T>(PhotonView actor, EventCode eventCode, T data)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        option.TargetActors = new int[] { actor.OwnerActorNr };
        PhotonNetwork.RaiseEvent((byte)eventCode, data, option, sendOptions);
    }

    public static void RaiseEvent<T>(EventCode eventCode, T data)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)eventCode, data, option, sendOptions);
    }

    public static void RaiseEvent(PhotonView actor, EventCode eventCode)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)eventCode, actor.Owner.ActorNumber, option, sendOptions);
    }

    //Dictionary<PhotonView, Dictionary<EventCode, Action>> ActorMethods = new Dictionary<PhotonView, Dictionary<EventCode, Action>>();

    Dictionary<byte, List<ActorMethods>> actorMethods = new Dictionary<byte, List<ActorMethods>>();

    public class ActorMethods
    {
        public PhotonView actor;
        public Action method;
        public bool allActors;
        //public Action<object> objectMethod;
    }

    public class ParamMethods<T> : ActorMethods
    {
        public Action<T> paramMethod;
    }

    //public static void RegisterMethod(PhotonView actor, EventCode eventCode, Action<object> method)
    //{
    //    var actorMethods = Instance.actorMethods;
    //    byte code = (byte)eventCode;
    //    if (actorMethods.ContainsKey(code))
    //    {
    //        var actors = actorMethods[code];
    //        ActorMethods am = null;
    //        foreach (var item in actors)
    //        {
    //            if (item.actor == actor) am = item as ActorMethods;
    //        }
    //        if (am == null)
    //        {
    //            var newActor = new ActorMethods { actor = actor, objectMethod = method };
    //            actors.Add(newActor);
    //        }
    //    }
    //    else
    //    {
    //        var newActor = new ActorMethods { actor = actor, objectMethod = method };
    //        actorMethods.Add(code, new List<ActorMethods> { newActor });
    //    }
    //}

    public static void RegisterMethod<T>(PhotonView actor, EventCode eventCode, Action<T> method, bool allActros)
    {
        var actorMethods = Instance.actorMethods;
        byte code = (byte)eventCode;
        if (actorMethods.ContainsKey(code))
        {
            var actors = actorMethods[code];
            ParamMethods<T> am = null;
            foreach (var item in actors)
            {
                if (item.actor == actor) am = item as ParamMethods<T>;
            }
            if (am == null)
            {
                var newActor = new ParamMethods<T> { actor = actor, paramMethod = method, allActors = allActros };
                actors.Add(newActor);
            }
        }
        else
        {
            var newActor = new ParamMethods<T> { actor = actor, paramMethod = method, allActors = allActros };
            actorMethods.Add(code, new List<ActorMethods> { newActor });
        }
    }

    public static void RegisterMethod(PhotonView actor, EventCode eventCode, Action method)
    {
        var actorMethods = Instance.actorMethods;
        byte code = (byte)eventCode;
        if (actorMethods.ContainsKey(code))
        {
            var actors = actorMethods[code];
            ActorMethods am = null;
            foreach (var item in actors)
            {
                if (item.actor == actor) am = item;
            }
            if (am == null)
            {
                var newActor = new ActorMethods { actor = actor, method = method };
                actors.Add(newActor);
            }
        }
        else
        {
            var newActor = new ActorMethods { actor = actor, method = method };
            actorMethods.Add(code, new List<ActorMethods> { newActor });
        }
    }

    //public static void RaiseEvent(EventCode code, object content)
    //{
    //    RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
    //    SendOptions sendOptions = new SendOptions { Reliability = true };
    //    PhotonNetwork.RaiseEvent((byte)code, content, option, sendOptions);
    //}

    public static void RaiseEvent(EventCode code, object content, ReceiverGroup receiverGroup)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)code, content, option, sendOptions);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //Player player = GameManager.instance.players.First(p => p.GetPhoton().Owner == null);
        //GameManager.instance.players.Remove(player);
       
        //if (player.GetComponent<PhotonView>().IsMine == true && PhotonNetwork.IsConnected == true)//возможно проверка лишняя но все работает норм
        //{
        //    PhotonNetwork.Destroy(player.gameObject);
        //}

        //    EventSystem.TriggerEvent(EventKey.PlayerLeave);
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (!applicationIsQuit)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuit = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (actorMethods.ContainsKey(photonEvent.Code))
        {
            var actors = actorMethods[photonEvent.Code];

            foreach (var actor in actors)
            {
                if (actor.actor == null)
                {
                    continue;
                }

                if (actor.actor.Owner.ActorNumber == photonEvent.Sender || actor.allActors)
                {
                    actor.method?.Invoke();

                    var data = photonEvent.CustomData;

                    if (data is float)
                    {
                        (actor as ParamMethods<float>).paramMethod?.Invoke((float)photonEvent.CustomData);
                    }
                    if (data is int)
                    {
                        (actor as ParamMethods<int>).paramMethod?.Invoke((int)photonEvent.CustomData);
                    }
                    if (data is string)
                    {
                        (actor as ParamMethods<string>).paramMethod?.Invoke((string)photonEvent.CustomData);
                    }
                    if (data is Vector2)
                    {
                        (actor as ParamMethods<Vector2>).paramMethod?.Invoke((Vector2)photonEvent.CustomData);
                    }
                    if (data is EnemyHealthPoint)
                    {
                        (actor as ParamMethods<EnemyHealthPoint>).paramMethod?.Invoke((EnemyHealthPoint)photonEvent.CustomData);
                    }
                    if (data is CreateDiceParams)
                    {
                        (actor as ParamMethods<CreateDiceParams>).paramMethod?.Invoke((CreateDiceParams)photonEvent.CustomData);
                    }
                    if (data is FirePhotonData)
                    {
                        (actor as ParamMethods<FirePhotonData>).paramMethod?.Invoke((FirePhotonData)photonEvent.CustomData);
                    }
                }

            }
        }
      
    }


    #region --== Serialize / Deserialize Photon Data ==--
    public static object DeserializeEventHpContent(byte[] data)
    {
        EventHpContent result = new EventHpContent();
        result.viewId = BitConverter.ToInt32(data, 0);
        result.hp = BitConverter.ToInt32(data, 4);

        return result;
    }

    public static byte[] SerializeEventHpContent(object obj)
    {
        EventHpContent ehc = (EventHpContent)obj;
        byte[] result = new byte[8];

        BitConverter.GetBytes(ehc.viewId).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.hp).CopyTo(result, 4);

        return result;
    }

    public static object DeserializeEventTurnContent(byte[] data)
    {
        FirePhotonData result = new FirePhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            zAngle = BitConverter.ToSingle(data, 4)
        };
        return result;
    }

    public static byte[] SerializeEventTurnContent(object obj)
    {
        FirePhotonData ehc = (FirePhotonData)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.zAngle).CopyTo(result, 4);
        return result;
    }

    public static object DeserializeEnemyHealthPoint(byte[] data)
    {
        EnemyHealthPoint result = new EnemyHealthPoint
        {
            viewId = BitConverter.ToInt32(data, 0),
            healthPoint = BitConverter.ToInt32(data, 4)
        };
        return result;
    }

    public static byte[] SerializeHealthPoint(object obj)
    {
        EnemyHealthPoint ehc = (EnemyHealthPoint)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewId).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.healthPoint).CopyTo(result, 4);
        return result;
    }
    
    public static object DeserializeCreateDiceParams(byte[] data)
    {
        CreateDiceParams result = new CreateDiceParams
        {
            stage = BitConverter.ToInt32(data, 0),
            kind = BitConverter.ToInt32(data, 4),
            pos = new Vector2
            {
                x = BitConverter.ToSingle(data, 8),
                y = BitConverter.ToSingle(data, 12),
            }
        };
        return result;
    }

    public static byte[] SerializeCreateDiceParams(object obj)
    {
        CreateDiceParams cdp = (CreateDiceParams)obj;
        byte[] result = new byte[16];
        BitConverter.GetBytes(cdp.stage).CopyTo(result, 0);
        BitConverter.GetBytes(cdp.kind).CopyTo(result, 4);
        BitConverter.GetBytes(cdp.pos.x).CopyTo(result, 8);
        BitConverter.GetBytes(cdp.pos.y).CopyTo(result, 12);
        return result;
    }
    #endregion

}

public class FirePhotonData
{
    public float zAngle;
    public int viewID;
}

public class EventHpContent
{
    public int viewId;
    public int hp;
}

public enum EventCode
{
    PlayerStartAttack = 1,
    PlayerStopAttack = 2,
    WeaponSpellActivation = 17,
    WeaponSpellDeactivation = 18,
    EnemySetHealthPoint = 11,
    EnemyDestroyed = 69,
    
    // PERKS
    Perk1 = 30,
    Perk2 = 31,
    Perk3 = 32
}

public class EventAction
{
    public int actorNumber;
    public Action action;
}


