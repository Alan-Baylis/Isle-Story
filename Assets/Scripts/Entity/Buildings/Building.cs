﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;
public enum BuildingType {
    INN,COMPANY
}
public enum SizeType {
    SINGLE,HEX
}
public class Building : Entity {
    public bool UnderConstruct = true;
    public float ConstructTime=9999f;
    public BuildingType type;
    public PersonList personList;
    public List<Person> Workers;
    TriDirection entranceDirection;
    public TriCell EntranceLocation {
        get {
            return Location.GetNeighbor(EntranceDirection);
        }
    }
    public new TriCell Location {
        get {
            return location;
        }
        set {
            location = value;
            value.Entity = this;
            transform.localPosition = value.Position;
        }
    }

    public TriDirection EntranceDirection {
        get {
            return entranceDirection;
        }
        set {
            entranceDirection = value;
            Vector3 rot = new Vector3(0, (int)entranceDirection*120+(Location.inverted?0:180), 0);
            transform.localRotation = Quaternion.Euler(rot);
        }
    }


    
    public override void Save(BinaryWriter writer) {
        base.Save(writer);
        writer.Write((int)type);
        writer.Write((int)EntranceDirection);
        Location.coordinates.Save(writer);
    }
    public static Building Load(BinaryReader reader) {
        BuildingType type = (BuildingType)reader.ReadInt32();
        TriDirection entDir=(TriDirection)reader.ReadInt32();
        TriCoordinates coord = TriCoordinates.Load(reader);
        Building ret=null;
        switch (type) {
            case BuildingType.INN:
                ret = Inn.Load(reader);
                break;
        }
        
        ret.Location= TriIsleland.Instance.grid.GetCell(coord);
        ret.EntranceDirection = entDir;
        ret.type = type;
        return ret;
    }
    
    public override void BindOptions(CommandPanel menu) {
        base.BindOptions(menu);
        if (UnderConstruct&&Workers.Count > 0) { }
            menu.BindButton(1, "workers", BindWorkers);
    }
    public void BindWorkers() {
        personList.Bind(this, Workers);
    }
    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Property.Instance.Bind(this);
    }
    public void AddWorker(Person p) {
        Workers.Add(p);
    }
    public void RemoveWorker(Person p) {
        Workers.Remove(p);
    }
    public virtual void CheckConstruction() {
        if (ConstructTime < 0) {
            UnderConstruct = false;
            for(int i = 0; i < Workers.Count; i++) {
                Workers[i].AddCommand(new ChangeWorkCommand(null));
                if (Workers[i].company)
                    Workers[i].AddCommand(new GoJobCommand());
                else
                    Workers[i].AddCommand(new GoHomeCommand());
            }
            Workers.Clear();
        }
        else
            ConstructTime -= Time.deltaTime*Workers.Count;
    }
    public virtual void DailyCycle() {

    }
    public void LateUpdate() {
        if (UnderConstruct) {
            CheckConstruction();
        }
        else {
            DailyCycle();
        }
    }
}
