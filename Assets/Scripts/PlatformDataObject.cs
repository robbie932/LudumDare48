using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlatformData
{
    public SizeType length;
    public SizeType offset;
    public SizeType sideOffset;
    public int sideCount;

    public float Length => PlatformCreator.instance.GetLengths(length);
    public float Offset => PlatformCreator.instance.GetOffset(offset);
    public float SideOffset => PlatformCreator.instance.GetSideOffset(sideOffset);

    public enum SizeType
    {
        S = 0,
        MS = 1,
        M = 2,
        ML = 3,
        L = 4
    }
}


[CreateAssetMenu(fileName = "PlatformData", menuName = "PlatformData")]
public class PlatformDataObject : ScriptableObject
{
    public PlatformData[] data;
    public PlatformData.SizeType offsetAfter;
    public float OffsetAfter => PlatformCreator.instance.GetOffset(offsetAfter);
    public bool globalDisplaySettingsFoldout;
}
