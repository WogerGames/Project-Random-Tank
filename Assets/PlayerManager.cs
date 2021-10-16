using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    public TeamNum team;
    public int countDestroy;

    public int cards;

    public List<PerkData> perksAvailable = new List<PerkData>()
    {
        new PerkData { id = 0 },
        new PerkData { id = 1 },
        new PerkData { id = 2 }
    };

    [HideInInspector]
    public List<PerkData> perksUsed = new List<PerkData>();

    [SerializeField] Sprite[] iconsPerks;
    [SerializeField] Sprite uporotost;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PerkData[] array = new PerkData[perksAvailable.Count];
        perksAvailable.CopyTo(array);

        perksUsed = array.ToList();
    }

    public Sprite GetIconByPerk(IPerk perk)
    {
        if(perk is Perk1)
        {
            return iconsPerks[0];
        }
        else if (perk is Perk2)
        {
            return iconsPerks[1];
        }

        return uporotost;
    }
    
}

[System.Serializable]
public class PerkData
{
    public int id;
    public int countCards;
    public int level;
}
