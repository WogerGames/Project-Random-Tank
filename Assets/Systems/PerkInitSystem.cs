using Leopotam.Ecs;
using UnityEngine;

sealed class PerkInitSystem : IEcsRunSystem
{

    readonly EcsFilter<PlayerSpawnEvent> spawnEvent;
    readonly EcsFilter<PlayerComponent> players;

    public void Run()
    {
        foreach (var s in spawnEvent)
        {
            IPerk[] perks = new IPerk[] { new Perk1(), new Perk2(), new Perk3() };

            var player = spawnEvent.Get1(s).player;

            if (player.perks == null)
            {
                player.perks = perks;

                if (!player.photonView.IsMine)
                {
                    spawnEvent.Get1(s).player.PerkEvent += PerkEvent;
                }
            }    
        }
    }

    void PerkEvent(EventCode eventCode, int viewID)
    {
        Debug.Log(viewID);
        foreach (var p in players)
        {
            if(players.Get1(p).view.photonView.ViewID == viewID)
            {
                var perk = eventCode.GetPerk();
                perk.AddPerkToEntity(ref players.GetEntity(p));
            }
        }
    }
}
