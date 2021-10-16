using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UpgradeParameter : MonoBehaviour
{
    [SerializeField] Button btnAdd;
    [SerializeField] Button btnSub;
    [SerializeField] TextMeshProUGUI nameParameter;
    [SerializeField] TextMeshProUGUI labelValue;

    public void Init(string name, float value)
    {
        nameParameter.text = name;
        labelValue.text = value.ToString("F0");
    }

    
}
