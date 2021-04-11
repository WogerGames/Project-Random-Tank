using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class DamageSystem : IEcsRunSystem
{
    
    readonly EcsFilter<ProjectileCollisionEvent> events;
    readonly EcsFilter<ProjectileComponent> projectiles;
    readonly EcsFilter<PLayerComponent, HealthPointComponent> players;

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

                    hp.Value--;
                    events.Get1(p).player.OnDamage(hp.Value);

                    if(hp.Value <= 0)
                    {
                        players.GetEntity(idx).Get<DestroyEvent>();
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
