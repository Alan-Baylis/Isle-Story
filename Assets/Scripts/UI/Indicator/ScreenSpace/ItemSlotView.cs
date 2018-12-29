﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemSlotView : MonoBehaviour {
    static int colSize = 5;
    public ItemSlot Target;
    public Image icon;
    public Text numIndicator;
    

    public void Bind(ItemSlot target,int count) {
        Target = target;
        if (Target) {
            numIndicator.text = Target.Quantity.ToString();
            
        }
        else {
            numIndicator.text = "";
        }
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(count%colSize*40+20,count/colSize*-40-20, 0);
    }
}
