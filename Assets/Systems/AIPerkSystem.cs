using Leopotam.Ecs;
using UnityEngine;
using System.Linq;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class AIPerkSystem : IEcsRunSystem
{
 
    readonly EcsFilter<PlayerComponent, ProgressEvent, AIControllerComponent> players;

    // Рандомный выбор перка на хосте
    void IEcsRunSystem.Run()
    {
        if (MultiplayerManager.IsMaster)
        {
            foreach (var p in players)
            {
                var perks = players.Get1(p).view.perks;

                var randomPerk = perks[Random.Range(0, perks.Length)];

                var leftPerks = perks.ToList();
                leftPerks.Remove(randomPerk);
                players.Get1(p).view.perks = leftPerks.ToArray();
                
                var eventCode = randomPerk.AddPerkToEntity(ref players.GetEntity(p));
                players.Get1(p).view.ChoosedPerk(eventCode);
                players.Get1(p).view.usedPerks.Add(randomPerk);
            }
        }
    }

    
}
