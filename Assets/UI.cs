using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject perksHolder;
    [SerializeField] GameObject perkPrefab;

    public GameObject PerksHolder => perksHolder;
    public GameObject PerkPrefab => perkPrefab;


    private void Start()
    {
        perksHolder.SetActive(false);
    }
}
