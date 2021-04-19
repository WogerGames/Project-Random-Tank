using Leopotam.Ecs;
using UnityEngine;

sealed class ProgressSystem : IEcsRunSystem
{
    readonly EcsFilter<DestroyEvent> events;
    readonly EcsFilter<PlayerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var e in events)
        {
            foreach (var p in players)
            {
                ref var player = ref players.Get1(p);

                if (player.view.photonView.ViewID == events.Get1(e).DestroyerID)
                {
                    var progress = player.view.ProgressCurve.Evaluate(player.countDestroyed);
                    
                    if(player.currentProgress < (int)progress)
                    {
                        player.currentProgress = (int)progress;
                        players.GetEntity(p).Get<ProgressEvent>();
                        //Debug.Log(player.currentProgress + " &&&&&&&&&&&&&&&&&&&&&&&&&&&&& " + player.view.photonView.ViewID);
                    }
                }
            }
        }
    }
}
