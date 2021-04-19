using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class DamageSystem : IEcsRunSystem
{
    
    readonly EcsFilter<ProjectileCollisionEvent> events;
    readonly EcsFilter<ProjectileComponent> projectiles;
    readonly EcsFilter<PlayerComponent, HealthPointComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in events)
        {
            foreach (var idx in players)
            {
                ref var player = ref players.Get1(idx);

                if (player.view == events.Get1(p).player)
                {
                    ref var hp = ref players.Get2(idx);

                    if (player.view.IsMine())
                    {
                        //Debug.Log(events.Get1(p).damageData.Damage);
                        hp.Value -= events.Get1(p).damageData.Damage;
                        hp.Value = Mathf.Clamp(hp.Value, 0, int.MaxValue);
                        events.Get1(p).player.OnDamage(hp.Value);
                    }

                    if(hp.Value <= 0)
                    {
                        players.GetEntity(idx).Get<DestroyEvent>().DestroyerID = events.Get1(p).damageData.OwnerId;
                    }
                }
            }
            

            foreach (var i in projectiles)
            {
                if(projectiles.Get1(i).view == events.Get1(p).projectile)
                {
                    Object.Destroy(projectiles.Get1(i).view.gameObject);
                    projectiles.GetEntity(i).Destroy();
                    
                }
            }
        }
    }
}

public class EnemyHealthPoint
{
    public int viewId;
    public int healthPoint;
}
