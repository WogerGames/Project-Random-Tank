using Leopotam.Ecs;
using UnityEngine;

sealed class PhotonIncreaseInitSystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerSpawnEvent> spawnEvent;
    readonly EcsFilter<PlayerComponent> players;

    public void Run()
    {
        foreach (var s in spawnEvent)
        {
            var player = spawnEvent.Get1(s).player;

            if (!player.photonView.IsMine)
            {
                spawnEvent.Get1(s).player.IncreaseEvent += IncreaseSetEvent;
            }
            
        }
    }

    void IncreaseSetEvent(IncreasePhotonData increaseData)
    {
        //Debug.Log(increaseData.viewID);
        foreach (var p in players)
        {
            if (players.Get1(p).view.photonView.ViewID == increaseData.viewID)
            {
                players.GetEntity(p).Get<IncreaseComponent>().Value = increaseData.Value;
            }
        }
    }
}
