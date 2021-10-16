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
                // +1 нужно для усиления, то есть значение рандома выходящее
                // за пределы длины массива перков, будет означать выбор усиления
                int rangeRandom = perks.Length + 1;
                int randomValue = Random.Range(0, rangeRandom);

                if (randomValue < perks.Length)
                {

                    var randomPerk = perks[randomValue];

                    var leftPerks = perks.ToList();
                    leftPerks.Remove(randomPerk);
                    players.Get1(p).view.perks = leftPerks.ToArray();

                    var eventCode = randomPerk.AddPerkToEntity(ref players.GetEntity(p));
                    players.Get1(p).view.PerkChoosed(eventCode);
                    players.Get1(p).view.usedPerks.Add(randomPerk);
                }
                else
                {
                    Debug.Log("Выбираем Казино!!!!!!!!!!!!!!!!!!!!!");

                    players.GetEntity(p).Get<IncreaseComponent>();

                    int chance = Random.Range(0, 100);
                    if (chance < players.GetEntity(p).Get<IncreaseComponent>().GetChance())
                    {
                        players.GetEntity(p).Get<IncreaseComponent>().Value++;
                        players.GetEntity(p).Get<IncreasedEvent>();
                        Debug.Log("Успээээээхх!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        players.Get1(p).view.IncreaseValue = players.GetEntity(p).Get<IncreaseComponent>().Value;
                    }
                    else Debug.Log("failishe--------_______________.......................");
                }
            }
        }
    }

    
}
