using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class ColorCameraView : MonoBehaviour
{
    public UnityEngine.UI.Image ImageComponent;

    bool imagePending = false;
    int width, height;
    byte[] pixels;

    // Update is called once per frame
    void Update()
    {
        if (imagePending)
        {
            // Convert BGRA32 image bytes from Azure Kinect to a texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.BGRA32, false);
            tex.LoadRawTextureData(pixels);
            tex.Apply();

            // Set image component sprite to new sprite created from texture
            ImageComponent.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(1.0f, 1.0f));

            // Wait for another image to be received
            imagePending = false;
        }
    }

    public void setImage(Image newImage)
    {
        // Store image width, height, and byte array
        width = newImage.WidthPixels;
        height = newImage.HeightPixels;
        pixels = newImage.Memory.ToArray();

        // New image can be processed in the main thread
        imagePending = true;
    }
}
