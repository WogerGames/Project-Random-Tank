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
            PlayerViewChanges(players.Get1(p).view);
            
            var playerID = players.GetEntity(p).GetInternalId();

            var player = players.Get1(p);

            var navMesh = player.view.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navMesh) navMesh.enabled = false;

            players.GetEntity(p).Del<PlayerComponent>();

            LeanTween.delayedCall(8, Respawn);

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

    void PlayerViewChanges(Player view)
    {
        
        view.collider.enabled = false;

        var renderers = view.transform.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            var color = renderer.color;
            color.a = 0.38f;
            renderer.color = color;
        }
        
    }

    
}
