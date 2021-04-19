using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class AIRotationSystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        if (!MultiplayerManager.IsMaster)
            return;

        foreach (var p in players)
        {
            var view = players.Get1(p).view;

            if (!view.GetComponent<PhotonView>()) continue;

            if (players.Get2(p).enemyTarget)
            {
                var playerPos = view.transform.position;
                var enemyPos = players.Get2(p).enemyTarget.position;

                var direction = enemyPos - playerPos;
                direction.x *= -1f;

                float angleTarget = Vector2.SignedAngle(direction, Vector2.up);
                float angleCurrent = view.transform.rotation.eulerAngles.z;
                float angle = Mathf.LerpAngle(angleCurrent, angleTarget, .1f);

                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                view.transform.rotation = rotation;
            }
        }
    }
}
