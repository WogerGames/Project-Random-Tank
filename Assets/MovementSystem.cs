using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class MovementSystem : IEcsRunSystem
{
    

    EcsFilter<PlayerComponent, MoveComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            var view = players.Get1(p).view;

            ref var move = ref players.Get2(p);

            if (move.Value != Vector2.zero)
            {
                view.transform.position += (Vector3)move.Value;
            }
        }
    }
}
