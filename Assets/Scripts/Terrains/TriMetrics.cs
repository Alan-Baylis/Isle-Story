﻿using UnityEngine;

public class TriMetrics {
    public static Color[] colors;
    public const float root = 1.732050807568877f;
    public const float outerRadius = 2f * root * innerRadius / 3f;
    public const float innerRadius = 10f;
    public const float waterElevationOffset = -0.5f;
    public const float streamBedElevationOffset = -3f;

    public const float solidFactor = 1f;
    public const float blendFactor = 1f - solidFactor;

    public const float elevationStep = 5f;

    public const float cellPerturbStrength = 4f;
    public const float noiseScale = 0.003f;
    public const float elevationPerturbStrength = 1.5f;

    public static Texture2D noiseSource;

    public const int chunkSizeX = 5, chunkSizeZ = 5;


    public static Vector3 Perturb(Vector3 position) {
        Vector4 sample = SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
        return position;
    }
    public static Vector4 SampleNoise(Vector3 position) {
        return noiseSource.GetPixelBilinear(
            position.x*noiseScale,
            position.z * noiseScale
            );
    }

    static Vector3[] corners = {
        new Vector3(0f,0f,outerRadius),
        new Vector3(innerRadius,0f,-0.5f*outerRadius),
        new Vector3(-innerRadius,0f,-0.5f*outerRadius),
        new Vector3(0f,0f,outerRadius)
    };
    public static Vector3 GetFirstCorner(TriDirection direction) {
        return corners[((int)direction+1)];
    }

    public static Vector3 GetSecondCorner(TriDirection direction) {
        return corners[((int) direction+2)%3];
    }

    public static Vector3 GetFirstSolidCorner(TriDirection direction) {
        return GetFirstCorner(direction) * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(TriDirection direction) {
        return GetSecondCorner(direction) * solidFactor;
    }
    public static Vector3 GetBridge(TriDirection direction) {
        return (GetFirstCorner(direction) + GetSecondCorner(direction))*blendFactor;
    }

    public static Vector3 GetSolidEdgeMiddle(TriDirection direction) {
        return
            (corners[(int)direction] + corners[(int)direction + 1]) *
            (0.5f * solidFactor);
    }
}
