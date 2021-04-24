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

    public float Length => Game.game.GetLengths(length);
    public float Offset => Game.game.GetOffset(offset);
    public float SideOffset => Game.game.GetSideOffset(sideOffset);

    public enum SizeType
    {
        Small = 0,
        Medium = 1,
        Big = 2
    }
}


[CreateAssetMenu(fileName = "PlatformData", menuName = "PlatformData")]
public class PlatformDataObject : ScriptableObject
{
    public PlatformData[] data;
    public float offsetAfter;
    public bool globalDisplaySettingsFoldout;
}
