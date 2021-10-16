using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeClasses : MonoBehaviour
{
    [SerializeField] Transform parentTabs;
    [SerializeField] Transform parentUpgradesParameters;

    [Space]

    [SerializeField] TabClass tabClassPrefab;
    [SerializeField] UpgradeParameter upgradeParameterPrefab;

    const int countParameters = 3;
   
    public void Init(Player[] playerClasses)
    {
        ClearAll();

        InitTabs(playerClasses);

        InitParameters(playerClasses.First());
    }

    void InitTabs(Player[] playerClasses)
    {
        foreach (var playerClass in playerClasses)
        {
           

            var tab = Instantiate(tabClassPrefab, parentTabs);

            var assault = playerClass.GetComponent<AssaultAuthoring>();
            if (assault)
            {
                tab.Init(TrooperClass.Assault, playerClass);
            }

            var engineer = playerClass.GetComponent<EngineerAuthoring>();
            if (engineer)
            {
                tab.Init(TrooperClass.Engineer, playerClass);
            }

            tab.tabClick += Tab_Clicked;
        }
    }

    private void Tab_Clicked(Player player)
    {
        InitParameters(player);
    }

    void InitParameters(Player player)
    {
        ClearParameters();

        var values = GetValuesParameters(player);

        for (int i = 0; i < countParameters; i++)
        {
            var parameter = Instantiate(upgradeParameterPrefab, parentUpgradesParameters);
            var name = GetNameParameter(i);
            var value = values[i];
            parameter.Init(name, value);
        }
    }

    private string GetNameParameter(int idxParameter)
    {
        switch (idxParameter)
        {
            case 0:
                return "Health";
            case 1:
                return "Damage";
            case 2:
                return "Firing Rate";
            default:
                break;
        }

        return "Не найдено блять";
    }

    float[] GetValuesParameters(Player player)
    {
        float[] values = new float[countParameters];

        for (int i = 0; i < countParameters; i++)
        {
            if(i == 0)
            {
                values[i] = player.GetComponent<HealthPointAuthoring>().MaxValue;
            }
            if (i == 1)
            {
                values[i] = player.GetComponent<PlayerAuthoring>().damage;
            }
            if (i == 2)
            {
                values[i] = player.GetComponent<FiringRateAuthoring>().Vaule;
            }
        }

        return values;
    }

    void ClearAll()
    {
        foreach (Transform item in parentTabs)
        {
            Destroy(item.gameObject);
        }

        ClearParameters();
    }

    void ClearParameters()
    {
        foreach (Transform item in parentUpgradesParameters)
        {
            Destroy(item.gameObject);
        }
    }
}

