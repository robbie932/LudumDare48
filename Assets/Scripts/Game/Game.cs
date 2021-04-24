using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Game : MonoBehaviour
{
    public static Game game;

    public float[] Lengths = new float[3];
    public float[] Offsets = new float[3];
    public float[] SideOffsets = new float[3];

    private void OnValidate()
    {
        game = this;
    }
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    public float GetLengths(PlatformData.SizeType type)
    {
        return (GetSetting(Lengths, type));
    }
    public float GetOffset(PlatformData.SizeType type)
    {
        return (GetSetting(Offsets, type));
    }
    public float GetSideOffset(PlatformData.SizeType type)
    {
        return (GetSetting(SideOffsets, type));
    }

    public float GetSetting(float[] from, PlatformData.SizeType type)
    {
        if (from.Length <= 0)
        {
            return 0;
        }
        return from[Mathf.Clamp((int)type, 0, from.Length - 1)];
    }

}
