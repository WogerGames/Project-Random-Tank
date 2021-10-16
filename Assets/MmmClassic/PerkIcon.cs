using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class PerkIcon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] Image icon;

    public Action<PerkData, PerkData> Droped;

    bool isDrag;

    PerkData perkData;

    public void Init(PerkData perkData, Sprite icon)
    {
        label.text = GameConfig.Perks[perkData.id].GetType().ToString();
        this.icon.sprite = icon;

        this.perkData = perkData;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;

        List<RaycastResult> raycastHits = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastHits);

        SlotPerkUsed slot = null;

        foreach (RaycastResult raycastHit in raycastHits)
        {
            slot = raycastHit.gameObject.GetComponent<SlotPerkUsed>();
            if (slot)
            {
                Droped?.Invoke(slot.PerkData, perkData);
                break;
            }
        }

        transform.GetComponent<RectTransform>().localPosition = Vector3.zero;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true; 
    }

    void Update()
    {
        if (isDrag)
        {
            var mousePos = Input.mousePosition;

            transform.GetComponent<RectTransform>().position = mousePos;
        }
    }
}
