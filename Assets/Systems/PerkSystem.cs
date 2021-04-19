using UnityEngine.UI;
using Leopotam.Ecs;
using UnityEngine;

sealed class PerkSystem : IEcsRunSystem
{
    readonly GameManager gameManager;
    // auto-injected fields.
    readonly EcsWorld _world = null;
    readonly EcsFilter<PlayerComponent, ProgressEvent>.Exclude<AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            var player = players.Get1(p).view;

            if (!player.photonView.IsMine)
                return;

            var perkHolder = gameManager.UI.PerksHolder;
            perkHolder.SetActive(true);
            foreach (var perk in player.perks)
            {
                var perkView = Object.Instantiate(gameManager.UI.PerkPrefab, perkHolder.transform);
                perkView.GetComponent<Button>().onClick.AddListener
                (
                    () => {
                        var eventCode = perk.AddPerkToEntity(ref players.GetEntity(p));
                        foreach (Transform item in gameManager.UI.PerksHolder.transform)
                        {
                            Object.Destroy(item.gameObject);
                        }
                        gameManager.UI.PerksHolder.SetActive(false);
                        player.ChoosedPerk(eventCode);
                        player.usedPerks.Add(perk);
                    }
                );
            
            }

            
        }
    }
}
