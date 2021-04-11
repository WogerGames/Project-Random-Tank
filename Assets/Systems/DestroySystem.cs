using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class DestroySystem : IEcsRunSystem
{
    readonly EcsFilter<PLayerComponent, DestroyEvent> players;
    readonly EcsFilter<PLayerComponent> ebachi;


    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            players.Get1(p).view.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
            players.Get1(p).view.collider.enabled = false;
            var playerID = players.GetEntity(p).GetInternalId();
            

            LeanTween.delayedCall(1, Respawn);

            void Respawn()
            {
                foreach (var i in ebachi)
                {
                    if(ebachi.GetEntity(i).GetInternalId() == playerID)
                    {
                        ebachi.GetEntity(i).Get<PlayerSpawnEvent>().player = ebachi.Get1(i).view;
                    }
                }
            }
        }
    }

    

    
}
