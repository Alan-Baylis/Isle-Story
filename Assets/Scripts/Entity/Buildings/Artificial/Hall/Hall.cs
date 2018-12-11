﻿using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;


public enum HallType {
    CAMP
}
public class Hall : Building {
    List<Person> livers;
    public HallType subType;
    protected int capacity;
    void setLiver(Person person) {
        if (livers.Count < 4) {
            livers.Add(person);
        }
    }
    public new void Save(BinaryWriter writer) {
        base.Save(writer);
        writer.Write((int)subType);
        writer.Write(capacity);
    }
    public void addPerson(Person p) {
        livers.Add(p);
    }
    public void removePerson(Person p) {
        livers.Remove(p);
    }
    public static Hall Load(BinaryReader reader) {
        HallType subType = (HallType)reader.ReadInt32();
        int capacity = reader.ReadInt32();
        Hall ret = null;
        switch (subType) {
            case HallType.CAMP:
                ret = Hall.Load(reader);
                break;
        }
        ret.subType = subType;
        ret.capacity = capacity;
        return ret;
    }
    public override void BindOptions(List<Button> buttons, Selector selector) {
        base.BindOptions(buttons, selector);
        buttons[5].gameObject.SetActive(true);
        buttons[5].GetComponentInChildren<Text>().text = "inn option";

    }
}
