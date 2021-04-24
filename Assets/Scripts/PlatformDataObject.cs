using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlatformData
{
    public float length, offset;
    public int sideCount;
    public float sideOffset;
    public enum Lengths
    { 
        Small = 5,
        Medium = 10,
        Big = 20
    }

    public enum Offsets
    {
        Small = 5,
        Medium = 7,
        Big = 10
    }

    public enum SideOffsets
    {
        Small = 5,
        Medium = 7,
        Big = 10
    }
}


[CreateAssetMenu(fileName = "PlatformData", menuName = "PlatformData")]
public class PlatformDataObject : ScriptableObject
{
    public PlatformData[] data;
    public float offsetAfter;
    public bool globalDisplaySettingsFoldout;
}
