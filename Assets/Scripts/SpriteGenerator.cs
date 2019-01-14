using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGenerator : ScriptableObject {
    
    
    // Encodes the tex2d to a png and returns as an array of bytes
    public byte[] tex2dtobytes(Texture2D texture)
    {
        return ImageConversion.EncodeToPNG(texture);//texture.GetRawTextureData();
    }

    // Encodes the tex2d to a png and returns as an array of bytes
    public Texture2D bytestotex2d(byte[] bytes)
    {
        Texture2D tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        ImageConversion.LoadImage(tex, bytes, false);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }

    public Sprite bytesToSprite(byte[] bytes)
    {
        Texture2D tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        ImageConversion.LoadImage(tex, bytes, false);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite mirror = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);

        return mirror;
    }
    

    /// <summary>
    /// Take in 2 textured and return a merged texture made up of parts of both source
    /// </summary>
    /// <param name="tex1"></param>
    /// <param name="tex2"></param>
    /// <returns></returns>
    public Texture2D mergeTextures(Texture2D tex1, Texture2D tex2)
    {
        // Our merged texture we will return, default to the first texture to have something to compare it against
        Texture2D merged = Instantiate(tex1) as Texture2D;

        for (int i = 0; i < tex1.width / 2; i++)
        {
            for (int j = 0; j < tex1.height; j++)
            {
                Color tex1Pixel = tex1.GetPixel(i, j);
                Color tex2Pixel = tex2.GetPixel(i, j);
                Color pixel = tex1Pixel;

                // we already set it to the first parent, should we grab this pixel from the second parent
                if (tex1Pixel != tex2Pixel) {
                    int range = 0;

                    // we want the black squares to be more domiant and likley to come accross then white
                    if (tex2Pixel == Color.black) {
                        range = GameController.game.random.range(1, 3);
                    } else if(tex2Pixel == Color.white)
                    {
                        range = GameController.game.random.range(1, 4);
                    }

                    if (range == 2)
                    {
                        pixel = tex2Pixel;
                    }
                    
                }
               

                /*
                if (pixel == Color.black || (pixel == Color.white && pixel == Color.clear))
                {
                    Debug.Log("Black" + i + "_" + j + "Pixel: " + pixel.ToString());
                    pixel.r = 0.5f;
                }
                if (pixel == Color.white)
                {
                    Debug.Log("White" + i + "_" + j + "Pixel: " + pixel.ToString());
                    pixel.b = 0.5f;
                }
                */
                merged.SetPixel(i, j, pixel);
            }
        }
        merged.Apply();
        return merged;
    }

    /// <summary>
    /// Mirror the current passed in image to assuming the image passed in is only the left side of the image and needs to be mirrored.
    /// </summary>
    /// <param name="sprite">The sprite to mirror the other side.</param>
    /// <returns> the resulting sprite after mirroring.</returns>
    public Sprite addXMirror(Sprite source)
    {
        Texture2D texture = Instantiate(source.texture) as Texture2D;

        for (int i = 0; i < texture.width/2; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                Color pixel = texture.GetPixel(i, j);

                texture.SetPixel((texture.width-i)-1, j, pixel);
            }
        }
        texture.Apply();
        Sprite mirror = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);

        return mirror;
    }

}
