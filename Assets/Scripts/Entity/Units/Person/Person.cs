﻿using UnityEngine;
using System.Collections;
using System.IO;
public class Person : Unit {
    public Inn home;
    public Building company;
    public Entity work;
    public void Start() {
        type = UnitType.PERSON;
        UIName = "person";

    }
    public override void Migrate (){
        ((ChangeHomeCommand)nowWork).target.addPerson(this);
        home = ((ChangeHomeCommand)nowWork).target;
    }
    public void GoHome() {
        CancelAllAct();
        AddCommand(new MoveCommand(home.Location));
        AddCommand(new GetInCommand(home));
    }
    public new void Save(BinaryWriter writer) {
        base.Save(writer);
        if (home)
            home.Location.coordinates.Save(writer);
        else new TriCoordinates(-1, -1).Save(writer);
        if (company)
            company.Location.coordinates.Save(writer);
        else new TriCoordinates(-1, -1).Save(writer);
        if (work)
            work.Location.coordinates.Save(writer);
        else new TriCoordinates(-1, -1).Save(writer);
    }
    
    public static new Person Load(BinaryReader reader) {
        Person ret= Instantiate((Person)TriIsleland.Instance.unitPrefabs[(int)UnitType.PERSON]);
        TriCell tLoc = TriGrid.Instance.GetCell(TriCoordinates.Load(reader));
        if (tLoc) {
            ret.home = (Inn)tLoc.Entity;
        }
        tLoc = TriGrid.Instance.GetCell(TriCoordinates.Load(reader));
        if (tLoc) {
            ret.company= (Company)tLoc.Entity;
        }
        tLoc = TriGrid.Instance.GetCell(TriCoordinates.Load(reader));
        if (tLoc) {
            ret.work= tLoc.Entity;
        }
        return ret;
    }
    public override void Build(){
        Building t=new Building();// = GameUI.Instance.mapEditor.CreateBuilding(nowWork.dir,nowWork.targetLocation,(Building)nowWork.target);
        t.Location = ((BuildCommand)nowWork).location;
        t.EntranceDirection= ((BuildCommand)nowWork).dir;
        TriIsleland.Instance.entities.AddBuilding(t);
        AddCommand(new ChangeWorkCommand(t));
    }
    public override void ChangeJob() {
        company = ((ChangeJobCommand)nowWork).target;
    }
    public override void ChangeWork() {
        work = ((ChangeWorkCommand)nowWork).target;
    }
    public override void BindOptions(EntityMenu menu) {
        base.BindOptions(menu);
        menu.BindButton(5, "build", menu.switchBuildMenu);
    }
}
