using UnityEngine;

public static class PerlinNoise
{
    static int width = 256;
    static int height = 256;

    static float scale = 20f;

    static float offsetX = 100f;
    static float offsetY = 100f;

    static public Texture2D GenerateTexture(float scale1, int width1, int height1)
    {
        scale = scale1;
        width = width1;
        height = height1;

        Texture2D texture = new Texture2D(width, height);

        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = GenerateColor(x,y);
                texture.SetPixel(x,y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    static Color GenerateColor(int x, int y)
    {
        float XCoord = (float) x / width * scale + offsetX;
        float YCoord = (float) y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(XCoord, YCoord);

        Color color = new Color(sample,sample,sample);

        return color;
    }

}
