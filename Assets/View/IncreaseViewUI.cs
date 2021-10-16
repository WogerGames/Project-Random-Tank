using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IncreaseViewUI : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text increaseValue;
    [SerializeField] TMP_Text chanceInrease;


    public Button Button => button;
    public TMP_Text IncreaseValue => increaseValue;
    public TMP_Text ChanceInrease => chanceInrease;
}
