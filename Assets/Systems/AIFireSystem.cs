using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class AIFireSystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        if (!MultiplayerManager.IsMaster)
            return;

        foreach (var p in players)
        {
            ref var player = ref players.Get1(p);

            //if (!player.view.GetComponent<PhotonView>()) continue;

            if (players.Get2(p).enemyTarget && player.cooldownRate >= player.cooldownValue)
            {
                player.view.Fire(player.damage);
                player.cooldownRate = 0;
                //Debug.Log($"{player.cooldownValue} -+-+- {player.view.photonView.ViewID}");
            }
        }
    }
}
