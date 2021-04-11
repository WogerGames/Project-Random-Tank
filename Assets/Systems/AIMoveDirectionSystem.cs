using Leopotam.Ecs;
using UnityEngine;


sealed class AIMoveDirectionSystem : IEcsRunSystem
{
    readonly EcsFilter<PLayerComponent, MoveComponent, AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            ref var ai = ref players.Get3(p);

            if (ai.moveTarget == Vector2.zero)
                continue;

            Vector2 dir = ai.moveTarget - (Vector2)players.Get1(p).view.transform.position;

            dir.Normalize();

            ref var move = ref players.Get2(p);

            move.Value = dir * Time.deltaTime * move.Speed;
        }
    }
}
