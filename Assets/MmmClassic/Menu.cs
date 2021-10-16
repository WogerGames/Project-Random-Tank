using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

// Просто GOD, и не эпёт
public class Menu : MonoBehaviour
{
    [SerializeField] Button btnOpenLarek;
    [SerializeField] Transform availablePerksRoot;
    [SerializeField] Transform usedPerksRoot;
    [SerializeField] GameObject slotAvailablePrefab;
    [SerializeField] GameObject slotUsedPrefab;
    [SerializeField] PerkIcon perkIconPrefab;
    [SerializeField] UpgradeClasses upgradeClasses;

    [Space]

    [SerializeField] Player[] playerClasses;


    PlayerManager playerManager;
    

    const int cardNeeded = 10;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        btnOpenLarek.onClick.AddListener(Larek_Opened);

        upgradeClasses.Init(playerClasses);

        UpdateAvailablePerks();
        UpdateUsedPerks();
    }

    private void Update()
    {
        btnOpenLarek.interactable = playerManager.cards >= cardNeeded;
    }

    void Larek_Opened()
    {
        playerManager.cards -= cardNeeded;

        var perks = GameConfig.Perks;

        var randomId = Random.Range(0, perks.Length);
        var randomCountCard = Random.Range(3, 7);
        var perk = perks[randomId];

        var containsPerk = playerManager.perksAvailable.Find(p => p.id == randomId);
        
        if (containsPerk == null)
        {
            containsPerk = new PerkData { id = randomId };
            playerManager.perksAvailable.Add(containsPerk);
        }

        containsPerk.countCards += randomCountCard;

        UpdateAvailablePerks();

    }

    void UpdateUsedPerks()
    {
        foreach (Transform item in usedPerksRoot)
        {
            Destroy(item.gameObject);
        }

        var perks = playerManager.perksUsed;

        foreach (var perkData in perks)
        {
            var slot = Instantiate(slotUsedPrefab, usedPerksRoot).GetComponent<SlotPerkUsed>();

            slot.Init(perkData);
            
            var perk = GameConfig.Perks[perkData.id];

            var icon = Instantiate(perkIconPrefab, slot.transform);

            icon.Init(perkData, playerManager.GetIconByPerk(perk));
        }
    }

    void UpdateAvailablePerks()
    {
        foreach (Transform item in availablePerksRoot)
        {
            Destroy(item.gameObject);
        }

        var perks = playerManager.perksAvailable;

        foreach (var perkData in perks)
        {
            var slot = Instantiate(slotAvailablePrefab, availablePerksRoot);

            var perk = GameConfig.Perks[perkData.id];

            var icon = Instantiate(perkIconPrefab, slot.transform);

            icon.Init(perkData, playerManager.GetIconByPerk(perk));

            icon.Droped += PerkIcon_Droped;
        }
    }


    void PerkIcon_Droped(PerkData from, PerkData to)
    {
        var idx1 = playerManager.perksUsed.IndexOf(from);
        var idx2 = playerManager.perksUsed.IndexOf(to);

        if (playerManager.perksUsed.Contains(to))
        {
            playerManager.perksUsed.RemoveAt(idx2);
            playerManager.perksUsed.Insert(idx2, from);
        }

        playerManager.perksUsed.RemoveAt(idx1);
        playerManager.perksUsed.Insert(idx1, to);

        UpdateUsedPerks();
    }
}
