using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Leopotam.Ecs;
using System;
[DefaultExecutionOrder(500)]
public class Player : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
{
    [SerializeField] GameObject projectile;
    [SerializeField] public TextMeshPro hpText;
    [SerializeField] public TextMeshPro increaseLabel;
    [SerializeField] new public Collider collider;
    [Tooltip("Количество уничтоженных врагов для получения перка")]
    [SerializeField] AnimationCurve progressCurve;
    [SerializeField] Vector2[] curvePoints = new Vector2[] { new Vector2(0, 0), new Vector2(5, 2), new Vector2(3, 12) };
    public IPerk[] perks;

    [Space]
    public TeamNum team;

    public List<IPerk> usedPerks = new List<IPerk>();

    public Action<EventCode, int> PerkEvent { get; set; }
    public Action<IncreasePhotonData> IncreaseEvent { get; set; }
    public AnimationCurve ProgressCurve => progressCurve;

    public byte IncreaseValue { get; set; }

    void Awake()
    {
        collider.enabled = false;
    }

    void Start()
    {
        if (!photonView) return;
        photonView.RegisterMethod<FirePhotonData>(EventCode.PlayerStartAttack, NotMinePlayerFire);
        photonView.RegisterMethod<EnemyHealthPoint>(EventCode.EnemySetHealthPoint, SetHP);
        photonView.RegisterMethod<IncreasePhotonData>(EventCode.Increase, NotMineIncreaseSet);

        GameManager.Instance.EcsWorld.NewEntity().Get<PlayerSpawnEvent>().player = this;

        if (curvePoints.Length > 0) {
            Keyframe[] keyframes = new Keyframe[curvePoints.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                var point = curvePoints[i];
                keyframes[i] = new Keyframe(point.x, point.y);
            }

            progressCurve.keys = keyframes;
        }

        var navMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        if (MultiplayerManager.IsMaster) 
        {
            if (navMesh) navMesh.enabled = false;
        }
        else
        {
            Destroy(navMesh);
        }
            
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        LeanTween.delayedCall(0.58f, SendPerkState);
        LeanTween.delayedCall(0.93f, SendInreaseState);

        void SendPerkState() => StartCoroutine(SendPerk());
        void SendInreaseState() => StartCoroutine(SendIncrease());
        

        IEnumerator SendPerk()
        {
            foreach (var perk in usedPerks)
            {
                var e = GameManager.Instance.EcsWorld.NewEntity();
                photonView.RaiseEvent(perk.AddPerkToEntity(ref e), photonView.ViewID, newPlayer.ActorNumber);

                yield return new WaitForSeconds(0.3f);
            }
        }

        IEnumerator SendIncrease()
        {
            if(IncreaseValue > 0)
            {
                var data = new IncreasePhotonData { viewID = photonView.ViewID, Value = IncreaseValue };
                photonView.RaiseEvent(EventCode.Increase, data, newPlayer.ActorNumber);
            }
            yield return new WaitForSeconds(0.3f);
        }
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation.eulerAngles.y);
        }

        if (stream.IsReading)
        {
            var rotY = (float)stream.ReceiveNext();
            transform.rotation = Quaternion.Euler(0, rotY, 0);
        }
    }

    public Vector2 Fire(int damage)
    {
        if (!collider.enabled)
            return default;

        var dd = new DamageData { Damage = damage, OwnerId = photonView.ViewID };

        var p = Instantiate(projectile, transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
        p.GetComponent<Projectile>().Init(dd);

        var fireData = new FirePhotonData
        {
            yAngle = transform.rotation.eulerAngles.y,
            viewID = photonView.ViewID
        };

        photonView.RaiseEvent(EventCode.PlayerStartAttack, fireData);

        return transform.forward;
    }

    void NotMinePlayerFire(FirePhotonData fireData)
    {
        if (!collider.enabled)
            return;

        int damage = 1;
        var filter = GameManager.Instance.EcsWorld.GetFilter(typeof(EcsFilter<PlayerComponent>));
        foreach (var i in filter)
        {
            var plr = filter.GetEntity(i).Get<PlayerComponent>();
            if (this == plr.view) damage = plr.damage;
        }

        if (photonView.ViewID == fireData.viewID)
        {
            var dd = new DamageData { Damage = damage, OwnerId = fireData.viewID };

            var p = Instantiate(projectile, transform.position, Quaternion.Euler(0, fireData.yAngle, 0));
            p.GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
            p.GetComponent<Projectile>().Init(dd);
        }
    }

    public void OnDamage(int damage)
    {
        var hpData = new EnemyHealthPoint { viewId = photonView.ViewID, healthPoint = damage };
        photonView.RaiseEvent(EventCode.EnemySetHealthPoint, hpData);
    }

    void SetHP(EnemyHealthPoint hp)
    {
        if (hp.viewId == photonView.ViewID)
        {
            var entity = GameManager.Instance.EcsWorld.NewEntity();
            ref var hpEvent = ref entity.Get<PhotonHealthPointEvent>();
            hpEvent.hp = hp.healthPoint;
            hpEvent.ViewID = hp.viewId;
        }
    }

    public void IncreaseChoosed(byte value)
    {
        var data = new IncreasePhotonData { viewID = photonView.ViewID, Value = value };
        photonView.RaiseEvent(EventCode.Increase, data);
    }

    public void PerkChoosed(EventCode perkEventCode)
    {
        photonView.RaiseEvent(perkEventCode, photonView.ViewID);
    }

    
    void FixedUpdate()
    {  
        hpText.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void LateUpdate()
    {
        hpText.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void NotMineIncreaseSet(IncreasePhotonData increaseData)
    {
        IncreaseEvent?.Invoke(increaseData);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)EventCode.Perk1)
        {
            PerkEvent?.Invoke(EventCode.Perk1, (int)photonEvent.CustomData);
        }
        if (photonEvent.Code == (byte)EventCode.Perk2)
        {
            PerkEvent?.Invoke(EventCode.Perk2, (int)photonEvent.CustomData);
        }
        if (photonEvent.Code == (byte)EventCode.Perk3)
        {
            PerkEvent?.Invoke(EventCode.Perk3, (int)photonEvent.CustomData);
        }
        
    }


}
