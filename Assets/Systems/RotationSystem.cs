using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class RotationSystem : IEcsRunSystem
{
    EcsFilter<PLayerComponent>.Exclude<AIControllerComponent> players;
    GameManager gameManager;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            var view = players.Get1(p).view;

            if (!view.GetComponent<PhotonView>()) continue;

            if (view.GetComponent<PhotonView>().IsMine)
            {

                var direction = gameManager.rotationJoystick.Direction;
                direction.x *= -1f;
                float angle = Vector2.SignedAngle(direction, Vector2.up);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                view.transform.rotation = rotation;
            }
        }
    }
}
