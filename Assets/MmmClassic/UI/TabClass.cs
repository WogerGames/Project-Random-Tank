using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class TabClass : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameClass;
    [SerializeField] Image progressbar;
    [SerializeField] Button button;

    public Action<Player> tabClick;

    //Player player;

    public void Init(TrooperClass trooperClass, Player player)
    {
        nameClass.text = trooperClass.ToString();

        //this.player = player;

        button.onClick.AddListener(() => tabClick?.Invoke(player));
    }
}
