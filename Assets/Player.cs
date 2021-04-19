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
    [SerializeField] new public Collider2D collider;
    [SerializeField] AnimationCurve progressCurve;
    public IPerk[] perks;

    [Space]
    public TeamNum team;

    public List<IPerk> usedPerks = new List<IPerk>();

    public AnimationCurve ProgressCurve => progressCurve;
    

    void Awake()
    {
        collider.enabled = false;
    }

    void Start()
    {
        if (!photonView) return;
        photonView.RegisterMethod<FirePhotonData>(EventCode.PlayerStartAttack, NotMinePlayerFire);
        photonView.RegisterMethod<EnemyHealthPoint>(EventCode.EnemySetHealthPoint, SetHP);

        GameManager.Instance.EcsWorld.NewEntity().Get<PlayerSpawnEvent>().player = this;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        LeanTween.delayedCall(0.58f, SendPerkState);

        void SendPerkState() => StartCoroutine(Send());
        

        IEnumerator Send()
        {
            foreach (var perk in usedPerks)
            {
                var e = GameManager.Instance.EcsWorld.NewEntity();
                photonView.RaiseEvent(perk.AddPerkToEntity(ref e), photonView.ViewID, newPlayer.ActorNumber);

                yield return new WaitForSeconds(0.3f);
            }
        }
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation.eulerAngles.z);
        }

        if (stream.IsReading)
        {
            var rotZ = (float)stream.ReceiveNext();
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }

    public Vector2 Fire(int damage)
    {
        if (!collider.enabled)
            return default;

        var dd = new DamageData { Damage = damage, OwnerId = photonView.ViewID };

        var p = Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
        p.GetComponent<Projectile>().Init(dd);

        var fireData = new FirePhotonData
        {
            zAngle = transform.rotation.eulerAngles.z,
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

            var p = Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, fireData.zAngle));
            p.GetComponent<SpriteRenderer>().color = Color.cyan;
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

    public void ChoosedPerk(EventCode perkEventCode)
    {
        photonView.RaiseEvent(perkEventCode, photonView.ViewID);
    }

    void Update()
    {
        hpText.transform.rotation = Quaternion.identity;
    }

    //public Func<EventCode> notMinePlayerChoosedPerk;
    public Action<EventCode, int> PerkEvent;

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
