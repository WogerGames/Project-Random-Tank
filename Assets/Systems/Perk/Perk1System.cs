using Leopotam.Ecs;
using UnityEngine;

sealed class Perk1System : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, Perk1> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            players.Get1(p).cooldownValue /= 2f;
            Debug.Log("SKOROSTOOOS");
        }
    }
}
