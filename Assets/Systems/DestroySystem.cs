using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class DestroySystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, DestroyEvent> players;
    readonly EcsFilter<PlayerComponent> ebachi;
    readonly EcsFilter<ObjectComponent> allObjects;


    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            players.Get1(p).view.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
            players.Get1(p).view.collider.enabled = false;
            var playerID = players.GetEntity(p).GetInternalId();

            var player = players.Get1(p);

            players.GetEntity(p).Del<PlayerComponent>();

            LeanTween.delayedCall(3, Respawn);

            void Respawn()
            {
                foreach (var o in allObjects)
                {
                    if(allObjects.Get1(o).go == player.view.gameObject)
                    {
                        ref var p = ref allObjects.GetEntity(o).Get<PlayerComponent>();
                        //p.view = player.view;
                        //p.teamNum = player.teamNum;
                        //p.currentProgress = player.currentProgress;
                        allObjects.GetEntity(o).Replace(player);

                        allObjects.GetEntity(o).Get<PlayerSpawnEvent>().player = player.view;
                    }
                }
            }
        }
    }

    

    
}
