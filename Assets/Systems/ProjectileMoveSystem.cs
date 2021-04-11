using Leopotam.Ecs;
using UnityEngine;

sealed class ProjectileMoveSystem : IEcsRunSystem
{
    EcsFilter<ProjectileComponent, ObjectComponent> bullets;

    void IEcsRunSystem.Run()
    {
        foreach (var b in bullets)
        {

            var go = bullets.Get2(b).go;
            go.transform.position += go.transform.up * Time.deltaTime * 15;
        }
    }
}
