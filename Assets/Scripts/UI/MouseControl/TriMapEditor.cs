﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class TriMapEditor : MonoBehaviour {
    public PersonList personList;
    public TriGrid triGrid;
    public TriIsleland isleland;
    public EntityManager entities;
    public TriDirection buildDirection;
    public int x, z;
    public TriMapGenerator mapGenerator;
    bool applyElevation = false;
    bool isDrag;
    TriDirection dragDirection;
    TriCell previousCell;
    int activeTerrainTypeIndex = -1;
    enum OptionalToggle {
        Ignore, Yes, No
    }
    int activeElevation;

    public void SetElevation(float elevation) {
        activeElevation = (int)elevation;
    }

    private void Awake() {
        SetEditMode(false);
    }

    void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButton(0)) {
                HandleInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U)) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    DestroyUnit();
                }
                else {
                    CreateUnit();
                }
            }
            if (Input.GetKeyDown(KeyCode.T)) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    DestroyUnit();
                }
                else {
                    CreateTent(buildDirection);
                }
            }
            if (Input.GetKeyDown(KeyCode.K)) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    DestroyUnit();
                }
                else {
                    CreateHall(buildDirection);
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                buildDirection = (buildDirection == TriDirection.RIGHT) ? TriDirection.VERT : buildDirection += 1;
            }
        }
        previousCell = null;
    }

    TriCell GetCellUnderCursor() {
        return
            triGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    void HandleInput() {
        TriCell currentCell = GetCellUnderCursor();
        if (currentCell) {
            if (previousCell && previousCell != currentCell) {
                ValidateDrag(currentCell);
            }
            else {
                isDrag = false;
            }
            EditCell(currentCell);
            previousCell = currentCell;
        }
        else {
            previousCell = null;
        }
    }

    public void SetEditMode(bool toggle) {
        enabled = toggle;
    }

    void ValidateDrag(TriCell currentCell) {
        for (
            dragDirection = TriDirection.VERT;
            dragDirection <= TriDirection.RIGHT;
            dragDirection++
        ) {
            if (previousCell.GetNeighbor(dragDirection) == currentCell) {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    OptionalToggle riverMode = OptionalToggle.Yes;

    public void SetRiverMode(int mode) {
        riverMode = (OptionalToggle)mode;
    }

    public void SetTerrainTypeIndex(int index) {
        activeTerrainTypeIndex = index;
    }

    public void SetApplyElevation(bool toggle) {
        applyElevation = toggle;
    }

    void EditCell(TriCell cell) {
        if (activeTerrainTypeIndex >= 0) {
            cell.TerrainTypeIndex = activeTerrainTypeIndex;
        }
        if (applyElevation) {
            cell.Elevation = activeElevation;
        }
        if (riverMode == OptionalToggle.No) {
            cell.RemoveRiver(dragDirection);
        }
        else if (isDrag && riverMode == OptionalToggle.Yes) {
            TriCell otherCell = cell.GetNeighbor(dragDirection);
            if (otherCell) {
                otherCell.SetRiver(dragDirection);
            }
        }
    }

    void EditHex(TriCell cell) {
        TriCell k = cell;
        bool inverted = cell.inverted;
        TriDirection d = TriDirection.VERT;
        for (int i = 0; i < 6; i++) {
            if (!k) break;
            EditCell(k);
            k = k.GetNeighbor(d);
            if (inverted)
                d = d.Next();
            else
                d = d.Previous();
        }
    }

    public void Save() {
        isleland.Save();
    }

    public void Load() {
        isleland.Load();
    }

    public void NewMap() {
        entities.ClearEntities();
        triGrid.CreateMap(x, z);
        mapGenerator.GenerateMap(x, z);
        isleland.topCam.ValidatePosition();
    }
    void CreateHall(TriDirection dir) {
        TriCell cell = GetCellUnderCursor();
        if (cell && Camp.IsBuildable(dir,cell.coordinates, SizeType.HEX)) {
            Camp ret = (Camp)Instantiate(isleland.hallPrefabs[0]);
            ret.ID = entities.UnitCount;
            ret.Location = cell;
            cell.Building = ret;
            ret.EntranceDirection = dir;
            ret.liverList = personList;
            entities.AddBuilding(ret);
            Debug.Log("camp built");
        }
        else {
            Debug.Log("building failed");
        }
    }
    void CreateTent(TriDirection dir) {
        TriCell cell = GetCellUnderCursor();
        if (cell && Tent.IsBuildable(dir, cell.coordinates,SizeType.SINGLE)) {

            Tent ret = (Tent)Instantiate(isleland.innPrefabs[0]);
            ret.ID = entities.UnitCount;
            ret.Location = cell;
            cell.Building = ret;
            ret.EntranceDirection = dir;
            ret.liverList = personList;
            entities.AddBuilding(ret);
            Debug.Log("tent built");
        }
        else {
            Debug.Log("building failed");
        }
    }
    void CreateUnit() {
        TriCell cell = GetCellUnderCursor();
        if (cell && !cell.Entity) {
            Unit ret=Instantiate(isleland.unitPrefabs[0]);
            ret.ID = entities.UnitCount;
            ret.Location = cell;
            ret.Orientation = Random.Range(0f, 360f);
            entities.AddUnit(ret);
        }
    }

    void DestroyUnit() {
        TriCell cell = GetCellUnderCursor();
        if (cell && cell.Entity) {
            entities.RemoveUnit(cell.Entity.ID);
        }
    }
    void DestroyBuilding() {
        TriCell cell = GetCellUnderCursor();
        if (cell && cell.Building) {
            entities.RemoveBuilding(cell.Building.ID);
        }
    }

}
