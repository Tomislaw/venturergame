﻿using UnityEngine;

public static class UnityEngineExtensions
{
    public static Vector2 Abs(this Vector2 vector)
    {
        for (int i = 0; i < 2; ++i) vector[i] = Mathf.Abs(vector[i]);
        return vector;
    }

    public static Vector2 DividedBy(this Vector2 vector, Vector2 divisor)
    {
        for (int i = 0; i < 2; ++i) vector[i] /= divisor[i];
        return vector;
    }

    public static Vector2 Max(this RectInt rect)
    {
        return new Vector2(rect.xMax, rect.yMax);
    }

    public static Vector2 IntersectionWithRayFromCenter(this RectInt rect, Vector2Int pointOnRay)
    {
        Vector2 pointOnRay_local = pointOnRay - rect.center;
        Vector2 edgeToRayRatios = (rect.Max() - rect.center).DividedBy(pointOnRay_local.Abs());
        return (edgeToRayRatios.x < edgeToRayRatios.y) ?
            new Vector2(pointOnRay_local.x > 0 ? rect.xMax : rect.xMin,
                pointOnRay_local.y * edgeToRayRatios.x + rect.center.y) :
            new Vector2(pointOnRay_local.x * edgeToRayRatios.y + rect.center.x,
                pointOnRay_local.y > 0 ? rect.yMax : rect.yMin);
    }
}