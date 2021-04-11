using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class CooldownRateSystem : IEcsRunSystem
{
    EcsFilter<PLayerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            ref var plr = ref players.Get1(p);
            if(plr.cooldownRate < 0.18f)
            {
                plr.cooldownRate += Time.deltaTime;
            }
            
        }
    }
}
