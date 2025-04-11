using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ReplayClip
{
    public List<byte[]> rawFrames = new List<byte[]>();
    public int width;
    public int height;
    public TextureFormat format;

    public ReplayClip(int width, int height, TextureFormat format)
    {
        this.width = width;
        this.height = height;
        this.format = format;
    }

    public void Dispose()
    {
        // memory allocaitn is hard
        rawFrames.Clear();
    }
}