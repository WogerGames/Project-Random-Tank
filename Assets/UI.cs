using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject perksHolder;
    [SerializeField] GameObject perkPrefab;
    [SerializeField] IncreaseViewUI increasePrefab;
    [SerializeField] GameObject panelComplete;

    [SerializeField] Button leave;

    public GameObject PanelComplete => panelComplete;
    public GameObject PerksHolder => perksHolder;
    public GameObject PerkPrefab => perkPrefab;
    public IncreaseViewUI IncreasePrefab => increasePrefab;
    public Button Leave => leave;


    private void Start()
    {
        perksHolder.SetActive(false);
        panelComplete.SetActive(false);
    }
}
