﻿using UnityEngine;
using System.Collections.Generic;

public class TriMapGenerator : MonoBehaviour {
    int cellCount;
    public TriGrid grid;
    Queue<TriCell> searchFrontier;
    HashSet<TriCoordinates> checker;
    [Range(0f, 1f)]
    public float jitterProbability = 0.25f;
    [Range(20, 200)]
    public int chunkSizeMin = 30;
    [Range(20, 200)]
    public int chunkSizeMax = 100;
    [Range(5, 95)]
    public int landPercentage = 50;
    int searchFrontierPhase;
    public void GenerateMap(int x, int z) {
        cellCount = x * z;
        grid.CreateMap(x, z);
        if (searchFrontier == null) {
            searchFrontier = new Queue<TriCell>();
            checker = new HashSet<TriCoordinates>();
        }
        CreateLand();
        SetTerrainType();
    }

    void CreateLand() {
        int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        bool initiated = false;
        while (landBudget > 0) {
            landBudget = RaiseTerrain(
                Random.Range(chunkSizeMin, chunkSizeMax + 1), landBudget, initiated
            );
            initiated = true;
        }
    }

    int RaiseTerrain(int chunkSize, int budget, bool initiated) {
        searchFrontierPhase += 1;
        TriCell firstCell = grid.GetCell(grid.cellCountX*grid.cellCountZ/2+grid.cellCountX/2);
        if (initiated) { 
            do {
                firstCell = GetRandomCell();
            } while (firstCell.Elevation < 1); }
        searchFrontier.Enqueue(firstCell);
        TriCoordinates center = firstCell.coordinates;
        checker.Add(center);
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0) {
            TriCell current = searchFrontier.Dequeue();
            if(current.Elevation<16)
            current.Elevation += 1;
            if (current.Elevation == 1 && --budget == 0) {
                break;
            }
            size += 1;

            for (TriDirection d = TriDirection.VERT; d <= TriDirection.RIGHT; d++) {
                TriCell neighbor = current.GetNeighbor(d);
                if (neighbor && !checker.Contains(neighbor.coordinates) && (d == TriDirection.RIGHT || Random.value < jitterProbability ? true : false)) {
                    searchFrontier.Enqueue(neighbor);
                    checker.Add(neighbor.coordinates);
                }
            }
        }
        searchFrontier.Clear();
        checker.Clear();
        return budget;
    }
    void SetTerrainType() {
        for (int i = 0; i < cellCount; i++) {
            TriCell cell = grid.GetCell(i);
            cell.TerrainTypeIndex = (int)Mathf.Pow(cell.Elevation,0.5f);
        }
    }
    TriCell GetRandomCell() {
        return grid.GetCell(Random.Range(0, cellCount));
    }
}
