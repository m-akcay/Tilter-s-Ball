using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Util
{
    public static float avg(params float[] vals)
    {
        float total = 0.0f;

        foreach (var f in vals)
        {
            total += f;
        }

        return total / vals.Length;
    }
   
    public static Color randColor()
    {
        float r = Random.Range(0.1f, 0.99f);
        float g = Random.Range(0.1f, 0.99f);
        float b = Random.Range(0.1f, 0.99f);

        return new Color(r, g, b);
    }

    public static void createPointTexture()
    {
        Color PURPLE = new Color(0.719f, 0, 1.0f);
        Color BLUE = new Color(0, 0, 1.0f);
        Color GREEN = new Color(0, 1.0f, 0);
        Color WHITE = new Color(1.0f, 1.0f, 1.0f);
        Color BLUE_GREEN = (BLUE + GREEN) / 2;

        // size is determined relative to the normal map
        int size = 256;
        Vector2 center = new Vector2(0.5f, 0.5f);
        Color[] colorArr = new Color[size * size];

        const float purpleLimit = 0.2f;
        const float blueLimit = 0.3f;
        const float greenLimit  = 0.8f;

        float maxDist = 0;
        Vector2 maxPt = new Vector2(-1, -1);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector2 thisPt = new Vector2((float)i / size, (float)j / size);
                float dist = Vector2.Distance(thisPt, center);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    maxPt = thisPt;
                }

                Color color;

                if (dist < purpleLimit)
                {
                    float purpleToBlue = smoothstep(0.0f, blueLimit, dist);
                    color = lerp(PURPLE, BLUE_GREEN, purpleToBlue);
                }
                else if (dist < blueLimit)
                {
                    float purpleToBlue = smoothstep(purpleLimit, blueLimit, dist);
                    color = lerp(BLUE_GREEN, GREEN, purpleToBlue);
                }
                else
                {
                    float greenToWhite = smoothstep(blueLimit, greenLimit, dist);
                    color = lerp(GREEN, WHITE, greenToWhite);
                }
                color.a = 1 - smoothstep(0.0f, greenLimit * 1.5f, dist);

                colorArr[i * size + j] = color;
            }
        }
        Debug.LogWarning(maxDist + "   " + maxPt);
        TextureRenderer.pointTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        TextureRenderer.pointTexture.wrapMode = TextureWrapMode.Clamp;
        TextureRenderer.pointTexture.filterMode = FilterMode.Trilinear;
        TextureRenderer.pointTexture.SetPixels(colorArr);
        TextureRenderer.pointTexture.Apply();
        Shader.SetGlobalTexture("Texture2D_70E33B5E", TextureRenderer.pointTexture);
        //AssetDatabase.CreateAsset(TextureRenderer.pointTexture, "Assets/PointTexture.png");
    }

    private static Color lerp(Color c0, Color c1, float t)
    {
        return c0 + t * (c1 - c0);
    }

    float lerp(float v0, float v1, float t)
    {
        return v0 + t * (v1 - v0);
    }

    private static float smoothstep(float edge0, float edge1, float x)
    {
        x = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        return x * x * (3 - 2 * x);
    }
}

public static class Vector3Extensions
{
    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector4 toVec4(this Vector3 v, float w)
    {
        return new Vector4(v.x, v.y, v.z, w);
    }

    public static Color toColor(this Vector3 v, float a)
    {
        return new Color(v.x, v.y, v.z, a);
    }
}

public static class TransformExtensions
{
    public static Vector2 transformPoint(this Transform transform, Vector2 point)
    {
        return new Vector2();
    }
}
