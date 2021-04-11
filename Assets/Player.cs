using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Leopotam.Ecs;
[DefaultExecutionOrder(500)]
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject projectile;
    [SerializeField] public TextMeshPro hpText;
    [SerializeField] new public Collider2D collider;

    [Space]
    public TeamNum team;

    void Awake()
    {
        collider.enabled = false;
    }

    void Start()
    {
        if (!photonView) return;
        photonView.RegisterMethod<float>(EventCode.PlayerStartAttack, NotMinePlayerFire);
        photonView.RegisterMethod<EnemyHealthPoint>(EventCode.EnemySetHealthPoint, SetHP);

        GameManager.Instance.EcsWorld.NewEntity().Get<PlayerSpawnEvent>().player = this;
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

    public Vector2 Fire()
    {
        var p = Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));

        photonView.RaiseEvent(EventCode.PlayerStartAttack, transform.rotation.eulerAngles.z);

        return transform.forward;
    }

    void NotMinePlayerFire(float angleZ)
    {
        Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, angleZ));   
    }

    public void OnDamage(int damage)
    {
        var hpData = new EnemyHealthPoint { viewId = photonView.ViewID, healthPoint = damage };
        photonView.RaiseEvent(EventCode.EnemySetHealthPoint, hpData);
    }

    void SetHP(EnemyHealthPoint hp)
    {
        //print(hp.viewId + " ----- " + photonView.ViewID);

        if (hp.viewId == photonView.ViewID)
        {
            var entity = GameManager.Instance.EcsWorld.NewEntity();
            ref var hpEvent = ref entity.Get<PhotonHealthPointEvent>();
            hpEvent.hp = hp.healthPoint;
            hpEvent.ViewID = hp.viewId;

        }
    }

    void Update()
    {
        hpText.transform.rotation = Quaternion.identity;
    }
}
