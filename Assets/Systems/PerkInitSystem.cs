using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

sealed class PerkInitSystem : IEcsRunSystem
{
    readonly PlayerManager playerManager;
    readonly EcsFilter<PlayerSpawnEvent> spawnEvent;
    readonly EcsFilter<PlayerComponent> players;

    public void Run()
    {
        foreach (var s in spawnEvent)
        {
            IPerk[] perks = ConvertToPerks(playerManager.perksUsed);

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

    IPerk[] ConvertToPerks(List<PerkData> perksData)
    {
        IPerk[] result = new IPerk[perksData.Count];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = GameConfig.Perks[perksData[i].id];
        }

        return result;
    }
}
