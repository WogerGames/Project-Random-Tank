using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class RotationSystem : IEcsRunSystem
{
    EcsFilter<PlayerComponent>.Exclude<AIControllerComponent> players;
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
                //direction.y *= -1f;
                float angleTarget = Vector2.SignedAngle(direction, Vector2.up);
                float angleCurrent = view.transform.rotation.eulerAngles.y;

                
                float angle = Mathf.LerpAngle(angleCurrent, angleTarget, .3f);
                
                Quaternion rotation = Quaternion.Euler(0, angle, 0);

                view.transform.rotation = rotation;
            }
        }
    }
}
