using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Leopotam.Ecs;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;

    Vector3 prevPoint, curPoint;

    public bool ishit;
    public int ViewID;

    public PhotonView hitPhotonView;

    public int OwnerId { get; set; }

    public float lifetime = 0;

    IEnumerator Start()
    {
        prevPoint = transform.position;
        curPoint = transform.position;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        

    }


    void Update()
    {
        CheckCollision();
        lifetime += Time.deltaTime;
    }

    void CheckCollision()
    {
        Vector3 dir = curPoint - prevPoint;

        var hit = Physics2D.RaycastAll(prevPoint, dir, dir.magnitude, collisionMask);

        if (hit.Length > 0 && lifetime > 0.1f)
        {
            //Debug.Log(hit.First().transform);

            var raycastHit = hit.ToList().Find(h => h.transform.GetComponent<Player>() != null);
            
            if (!raycastHit.transform)
                return;

            var player = raycastHit.transform.GetComponent<Player>();

            if (player)
            {
                var e = GameManager.Instance.EcsWorld.NewEntity();
                e.Get<ProjectileCollisionEvent>().player = player;
                e.Get<ProjectileCollisionEvent>().projectile = this;

                if (player.photonView)
                {
                    hitPhotonView = player.photonView;
                }
            }
            
        }
        
        prevPoint = curPoint;
        curPoint = transform.position;
    }
}
