﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TriGridChunk : MonoBehaviour {
    TriCell[] cells;
    public TriMesh terrain;
    Canvas gridCanvas;
    public void setLabels(bool val) {
        gridCanvas.enabled = val;
    }
    void Awake() {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new TriCell[TriMetrics.chunkSizeX * TriMetrics.chunkSizeZ];
        ShowUI(false);
    }
    public void ShowUI(bool visible) {
        gridCanvas.gameObject.SetActive(visible);
    }
    public void AddCell(int index, TriCell cell) {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }
    public void Refresh() {
        enabled = true;
    }
    private void LateUpdate() {
        if (enabled) {
            Triangulate();
            enabled = false;
        }
    }
    public void Triangulate() {
        terrain.Clear();
        for (int i = 0; i < cells.Length; i++) {
            Triangulate(cells[i]);
        }
        terrain.Apply();
    }
    void Triangulate(TriCell cell) {
        for (TriDirection d = TriDirection.VERT; d <= TriDirection.RIGHT; d++) {
            Triangulate(d, cell);
        }
    }
    void Triangulate(TriDirection direction, TriCell cell) {
        int inverter = 0;
        if (cell.inverted) inverter = -1;
        else inverter = 1;

        Vector3 center = cell.Position, v1, v2;

        v1 = center + inverter * TriMetrics.GetFirstSolidCorner(direction);
        v2 = center + inverter * TriMetrics.GetSecondSolidCorner(direction);
        EdgeVertices e = new EdgeVertices(v1, v2);
        if (cell.HasRiverThroughEdge(direction)) {
            TriangulateWithRiver(cell, direction, center, e, cell.Color);
        }
        else {
            TriangulateEdgeFan(center, e, cell.Color);
        }
        if (cell.inverted) {
            if (direction != TriDirection.LEFT)
                TriangulateConnection(direction, cell, e, inverter);
        }
        else {
            if (direction == TriDirection.LEFT)
                TriangulateConnection(direction, cell, e, inverter);
        }

    }
    void TriangulateConnection(TriDirection direction, TriCell cell, EdgeVertices e1, int inverter) {
        Vector3 center = cell.transform.localPosition;
        TriCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null) return;
        Vector3 bridge = TriMetrics.GetBridge(direction);
        bridge.y = neighbor.Position.y - cell.Position.y;
        EdgeVertices e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v5 + bridge
            );
        TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color,cell.HasRiverThroughEdge(direction));

    }
    void TriangulateWithRiver(TriCell cell, TriDirection direction, Vector3 center, EdgeVertices edge, Color color) {

        Vector3 nextCorner, prevCorner;
        nextCorner = (center + edge.v1) / 2f;
        prevCorner = (center + edge.v5) / 2f;
        Vector3 underCenter = center, underNextCorner = nextCorner, underPrevCorner = prevCorner;
        underCenter.y += TriMetrics.streamBedElevationOffset;
        underNextCorner.y += TriMetrics.streamBedElevationOffset;
        underPrevCorner.y += TriMetrics.streamBedElevationOffset;

        EdgeVertices underEdge = edge;
        underEdge.v1.y += TriMetrics.streamBedElevationOffset;
        underEdge.v2.y += TriMetrics.streamBedElevationOffset;
        underEdge.v3.y += TriMetrics.streamBedElevationOffset;
        underEdge.v4.y += TriMetrics.streamBedElevationOffset;
        underEdge.v5.y += TriMetrics.streamBedElevationOffset;

        terrain.AddTriangle(nextCorner, edge.v1, edge.v2);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(prevCorner, edge.v4, edge.v5);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(underCenter, underEdge.v2, underEdge.v4);
        terrain.AddTriangleColor(color);
        if (cell.HasRiverThroughEdge(direction.Next())) {
            terrain.AddTriangle(underEdge.v4, underPrevCorner, underCenter);
            terrain.AddTriangleColor(color);
            terrain.AddQuad(underPrevCorner, prevCorner, underEdge.v4, edge.v4);
            terrain.AddQuadColor(color, color);
        }
        else {
            terrain.AddTriangle(center, edge.v4, prevCorner);
            terrain.AddTriangleColor(color);
            terrain.AddQuad(underCenter, center, underEdge.v4, edge.v4);
            terrain.AddQuadColor(color, color);
        }
        if (cell.HasRiverThroughEdge(direction.Previous())) {
            terrain.AddTriangle(underEdge.v2, underCenter, underNextCorner);
            terrain.AddTriangleColor(color);
            terrain.AddQuad(underEdge.v2, edge.v2, underNextCorner, nextCorner);
            terrain.AddQuadColor(color, color);
        }
        else {
            terrain.AddTriangle(center, nextCorner, edge.v2);
            terrain.AddTriangleColor(color);
            terrain.AddQuad(underEdge.v2, edge.v2, underCenter, center);
            terrain.AddQuadColor(color, color);
        }
    }
    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color) {
        Vector3 nextCorner, prevCorner;
        nextCorner = (center + edge.v1) / 2f;
        prevCorner = (center + edge.v5) / 2f;
        terrain.AddTriangle(nextCorner, edge.v1, edge.v2);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(prevCorner, edge.v4, edge.v5);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v2, edge.v4);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v4, prevCorner);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, nextCorner, edge.v2);
        terrain.AddTriangleColor(color);
    }
    void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2,bool isRiverInDir) {
        
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);
        //terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        //terrain.AddQuadColor(c1, c2);
        if (isRiverInDir) {
            Vector3 t = new Vector3(0, TriMetrics.streamBedElevationOffset, 0);
            terrain.AddQuad(e1.v2 + t, e1.v4 + t, e2.v2 + t, e2.v4 + t);
        }
        else terrain.AddQuad(e1.v2, e1.v4, e2.v2, e2.v4);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);
    }
}
