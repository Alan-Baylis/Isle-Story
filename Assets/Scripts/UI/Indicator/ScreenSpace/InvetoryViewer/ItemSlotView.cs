﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ItemSlotView : MonoBehaviour {
    public static int ColSize = 5;
    public int Index;
    public ItemSlot Target;
    public Image icon;
    public Text numIndicator;
    

    public void Bind(ItemSlot target,int count) {
        Target = target;
        Index = count;
        if (Target.Content) {
            numIndicator.text = Target.Quantity.ToString();
            icon.sprite = InventoryViewer.Instance.Sprites[Target.Content.Id];
        }
        else {
            numIndicator.text = "";
        }
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(count%ColSize*40+20,count/ColSize*-40-20, 0);
    }
    public void Refresh() {
        if (Target.Content) {
            numIndicator.text = Target.Quantity.ToString();
            icon.sprite = InventoryViewer.Instance.Sprites[Target.Content.Id];
        }
        else {
            icon.sprite = null;
            numIndicator.text = "";
        }
        Target.refreshed = false;
    }
    private void LateUpdate() {
        if (Target!=null&&Target.refreshed) {
            Refresh();
        }
    }
    public void Clear() {
        Target = null;
        Index = -1;
    }
}
