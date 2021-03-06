﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class Tent : Inn {
    public override void Start() {
        base.Start();
        camAnchorOffset = Vector3.zero;
    }
    public override void Save(BinaryWriter writer) {
        base.Save(writer);
    }
    public static new Tent Load(BinaryReader reader) {
        return Instantiate((Tent)TriIsleland.GetBuildingPrefabs((int)BuildingType.INN, (int)InnType.TENT, 0));
    }
}
