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

            var enemyTarget = players.Get2(p).enemyTarget;

            if (enemyTarget)
            {
                var playerPos = new Vector2(view.transform.position.x, view.transform.position.z);
                var enemyPos = new Vector2 (enemyTarget.position.x, enemyTarget.position.z);

                var direction = enemyPos - playerPos;
                //direction.x *= -1f;

                float angleTarget = Vector2.SignedAngle(direction, Vector2.up);
                float angleCurrent = view.transform.rotation.eulerAngles.y;
                float angle = Mathf.LerpAngle(angleCurrent, angleTarget, .1f);

                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                view.transform.rotation = rotation;
            }
        }
    }
}
