using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPerkUsed : MonoBehaviour
{
    public PerkData PerkData { get; private set; }

    public void Init(PerkData perkData)
    {
        PerkData = perkData;
    }
}
