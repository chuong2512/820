using UnityEngine;
using UnityEngine.UI;

public class DynamicImage : RawImage
{
    public float rectWidth;
    public float rectHeight;

    public void SetImage(Texture2D texture)
    {
        var imgWidth = texture.width;
        var imgHeight = texture.height;

        var imgAspect = imgWidth / imgHeight;
        var rectAspect = rectWidth / rectHeight;

        var currentWidth = 0f;
        var currentHeight = 0f;

        if (rectAspect > imgAspect)
        {
            currentWidth = (rectHeight / imgHeight) * imgWidth;
            currentHeight = rectHeight;
        }
        else
        {
            currentWidth = rectWidth;
            currentHeight = (rectWidth / imgWidth) * imgHeight;
        }

        rectTransform.sizeDelta = new Vector2(currentWidth, currentHeight);
        this.texture = texture;
    }

    public void RemoveImage()
    {
        texture = Texture2D.whiteTexture;
        rectTransform.sizeDelta = new Vector2(rectWidth, rectHeight);
    }

    public Texture2D GetImage()
    {
        return (Texture2D) texture;
    }
}