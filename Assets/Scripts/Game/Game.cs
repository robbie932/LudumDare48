using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Game : MonoBehaviour
{
    public static Game game;

    private void OnValidate()
    {
        game = this;
    }
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
