﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EntityMenu : WorldSpaceCanvas{
    public Entity nowEntity;
    public Text tooltip;
    public List<Button> buttons;
    public new bool enabled {
        get {
            return base.enabled;
        }
        set {
            base.enabled = value;
            Clear();
        }
    }
    private void Start() {
        canvas = GetComponent<Canvas>();
    }
    public void Bind(Entity entity) {
        nowEntity = entity;
        tooltip.text = nowEntity.UIName;
        Vector3 t=nowEntity.Location.transform.localPosition;
        t.y += 20;
        transform.localPosition = t;
        nowEntity.BindOptions(this);
    }
    public void Clear() {
        for(int i = 0; i < buttons.Count; i++) {
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
        }
    }
    public void BindButton(int index,string tooltip,UnityEngine.Events.UnityAction action) {
        buttons[index].gameObject.SetActive(true);
        buttons[index].GetComponentInChildren<Text>().text = tooltip;
        buttons[index].onClick.AddListener(action);
    }
    public void ToBuildingOption() {
        cameraManager.SwitchCamera(CamType.BUILDINGVIEW);
        ((IndivViewCam)cameraManager.GetNowActive()).setAnchor(nowEntity);
        ((IndivViewCam)cameraManager.GetNowActive()).SetOffset(nowEntity);
    }
    private void Update() {
        if (enabled)
            transform.rotation = cameraManager.GetNowActive().cam.transform.rotation;
        
    }
}
