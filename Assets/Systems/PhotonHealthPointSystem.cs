using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class PhotonHealthPointSystem : IEcsRunSystem
{
    EcsFilter<PhotonHealthPointEvent> events;
    EcsFilter<PLayerComponent, HealthPointComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var e in events)
        {
            foreach (var p in players)
            {
                var view = players.Get1(p).view;
                if(view.photonView.ViewID == events.Get1(e).ViewID)
                {
                    players.Get2(p).Value = events.Get1(e).hp;
                }
            }
        }
    }
}
