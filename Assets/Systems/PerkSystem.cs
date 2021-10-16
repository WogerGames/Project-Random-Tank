using UnityEngine.UI;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;

sealed class PerkSystem : IEcsRunSystem
{
    readonly GameManager gameManager;
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

            foreach (Transform item in perkHolder.transform)
            {
                Object.Destroy(item.gameObject);
            }
            // Перки
            foreach (var perk in player.perks)
            {
                var perkView = Object.Instantiate(gameManager.UI.PerkPrefab, perkHolder.transform);
                perkView.GetComponentInChildren<TextMeshProUGUI>().text = perk.ToString();
                perkView.GetComponent<Button>().onClick.AddListener
                (
                    () => {
                        var eventCode = perk.AddPerkToEntity(ref players.GetEntity(p));
                        
                        perkHolder.SetActive(false);
                        player.PerkChoosed(eventCode);
                        player.usedPerks.Add(perk);
                    }
                );
                var found = player.usedPerks.Find(p => p == perk);
                if (found != null)
                {
                    perkView.GetComponent<Button>().interactable = false;
                }
            }
            // Усиление
            var increase = Object.Instantiate(gameManager.UI.IncreasePrefab, perkHolder.transform);

            var entity = players.GetEntity(p);

            var nextIncrease = new IncreaseComponent();
            if (entity.Has<IncreaseComponent>())
            {
                nextIncrease = entity.Get<IncreaseComponent>();
            }

            increase.ChanceInrease.text = $"{nextIncrease.GetChance()}%";

            nextIncrease.Value++;
            increase.IncreaseValue.text = $"{nextIncrease}";

            increase.Button.onClick.AddListener(() => 
            {
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
                perkHolder.SetActive(false);
            });

        }
    }
}
