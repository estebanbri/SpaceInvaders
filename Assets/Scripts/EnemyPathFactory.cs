using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyPathFactory
{
    public static List<Vector3> CreatePath(
        FormationType type,
        float spawnY,
        float spawnWidth,
        float vVertexDepth,
        float randomSpawnY,
        float arcX = 0f,
        bool fromLeft = true)
    {
        switch (type)
        {
            case FormationType.VPath:
                float vertexY = spawnY - vVertexDepth;
                return CreateVPath(spawnY, spawnWidth, vertexY);

            case FormationType.SineDive:
                return CreateSineDive(spawnY, spawnWidth);

            case FormationType.Hover:
                float hoverX = UnityEngine.Random.Range(-spawnWidth, spawnWidth);
                return CreateHover(spawnY, hoverX);

            case FormationType.SideArc:
                return CreateSideArc(spawnY, arcX);

            case FormationType.HorizontalPass:
                return CreateHorizontalPass(randomSpawnY, spawnWidth, fromLeft);

            default:
                return null;
        }
    }

    static List<Vector3> CreateVPath(
    float spawnY,
    float spawnWidth,
    float vertexY)
    {
        List<Vector3> points = new List<Vector3>();

        float sideOffset = spawnWidth * 0.9f;

        // 🔴 START (arriba izquierda)
        Vector3 p0 = new Vector3(-sideOffset, spawnY, 0f);

        // 🔻 VÉRTICE (centro exacto X = 0)
        Vector3 p1 = new Vector3(0f, vertexY, 0f);

        // 🔵 END (arriba derecha)
        Vector3 p2 = new Vector3(sideOffset, spawnY, 0f);

        points.Add(p0);
        points.Add(p1);
        points.Add(p2);

        return points;
    }

    static List<Vector3> CreateSineDive(float spawnY, float spawnWidth)
    {
        List<Vector3> points = new List<Vector3>();

        int resolution = 120;         // más puntos = más largo
        float amplitude = spawnWidth * 0.6f;
        float verticalSpacing = 0.3f; // separación vertical entre puntos
        float frequency = 2f;         // qué tan cerrada la onda

        for (int i = 0; i < resolution; i++)
        {
            float y = spawnY - i * verticalSpacing;

            // 👇 El truco: el seno depende de la altura
            float x = Mathf.Sin(y * frequency) * amplitude;

            points.Add(new Vector3(x, y, 0f));
        }

        return points;
    }

    static List<Vector3> CreateHover(float spawnY, float x)
    {
        List<Vector3> points = new List<Vector3>();

        points.Add(new Vector3(x, spawnY, 0));
        points.Add(new Vector3(x, spawnY - 3f, 0));

        return points;
    }

    static List<Vector3> CreateSideArc(float spawnY, float x)
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 p0 = new Vector3(x, spawnY, 0);
        Vector3 p1 = p0 + new Vector3(3f, -3f, 0);
        Vector3 p2 = p0 + new Vector3(0f, -8f, 0);

        int resolution = 25;

        // 🔵 Parte curva
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            points.Add(Bezier(p0, p1, p2, t));
        }

        // 🔴 Continuación descendente
        Vector3 last = points[points.Count - 1];

        float extraDepth = 12f;
        int extraSteps = 30;

        for (int i = 1; i <= extraSteps; i++)
        {
            float t = i / (float)extraSteps;
            float y = last.y - t * extraDepth;

            points.Add(new Vector3(last.x, y, 0));
        }

        return points;
    }

    static List<Vector3> CreateHorizontalPass(
    float spawnY,
    float spawnWidth,
    bool fromLeft)
    {
        List<Vector3> points = new List<Vector3>();

        float extraDistance = spawnWidth * 1.5f;

        float startX = fromLeft
            ? -spawnWidth - extraDistance
            : spawnWidth + extraDistance;

        float endX = fromLeft
            ? spawnWidth + extraDistance
            : -spawnWidth - extraDistance;

        int resolution = 80;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            float x = Mathf.Lerp(startX, endX, t);

            points.Add(new Vector3(x, spawnY, 0));
        }

        return points;
    }

    static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return Mathf.Pow(1 - t, 2) * a +
               2 * (1 - t) * t * b +
               Mathf.Pow(t, 2) * c;
    }
}