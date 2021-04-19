using Leopotam.Ecs;
using UnityEngine;

sealed class ProjectileLifetimeSystem : IEcsRunSystem
{
    readonly EcsFilter<ProjectileComponent> projectiles;

    void IEcsRunSystem.Run()
    {
        foreach (var p in projectiles)
        {
            if(projectiles.Get1(p).view.lifetime > 3.5f)
            {
                Object.Destroy(projectiles.Get1(p).view.gameObject);
                projectiles.GetEntity(p).Destroy();
            }
        }
    }
}
