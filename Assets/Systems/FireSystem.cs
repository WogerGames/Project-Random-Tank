using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class FireSystem : IEcsRunSystem
{
    
    GameManager gameManager;
    EcsFilter<PLayerComponent>.Exclude<AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            ref var plr = ref players.Get1(p);

            if (!plr.view.GetComponent<PhotonView>()) continue;

            if (plr.view.GetComponent<PhotonView>().IsMine)
            {

                if (plr.cooldownRate >= 0.18f && gameManager.rotationJoystick.Direction.magnitude > 0)
                {
                    plr.view.Fire();
                    plr.cooldownRate = 0;
                }
            }
        }
    }
}
