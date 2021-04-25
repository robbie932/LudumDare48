using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    public static WinScreenUI instance;
    public Animation anim;

    public Button button;
    bool clicked;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(true);
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void PlayEnd()
    {
        anim.Play();
    }

    private void OnClick()
    {
        if (clicked)
        {
            return;
        }

        clicked = true;
        //load next level
    }
}
