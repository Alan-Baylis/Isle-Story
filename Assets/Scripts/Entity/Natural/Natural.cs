﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public enum NaturalType {
    TREE
}
public class Natural : Entity {
    public NaturalType type;
    TriDirection entranceDirection;

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
            Vector3 rot = new Vector3(0, (int)entranceDirection * 120*(Location.inverted?-1:1), 0);
            transform.localRotation = Quaternion.Euler(rot);
        }
    }
    public void validateRotation() {
        Vector3 rot = new Vector3(0, (int)entranceDirection * 120 * (Location.inverted ? -1 : 1), 0);
        transform.localRotation = Quaternion.Euler(rot);
    }
    public new void Save(BinaryWriter writer) {
        base.Save(writer);
        writer.Write((int)type);
        writer.Write((int)EntranceDirection);
    }
    public static Natural Load(BinaryReader reader) {
        NaturalType type = (NaturalType)reader.ReadInt32();
        TriDirection entDir=(TriDirection)reader.ReadInt32();
        Natural ret=null;
        switch (type) {
            case NaturalType.TREE:
                ret = Tree.Load(reader);
                break;
        }
        ret.entranceDirection = entDir;
        ret.type = type;
        return ret;
    }
}
