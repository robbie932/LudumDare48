using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public void OnEnd()
    {
        StartCoroutine(TheEnd(1f));
    }
    private IEnumerator TheEnd(float duration)
    {
        var t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            var a = t / duration;
            Time.timeScale = Mathf.Max(0, 1 - a);
            yield return null;
        }
    }

    private void OnClick()
    {
        if (clicked)
        {
            return;
        }

        clicked = true;
        StopAllCoroutines();
        Time.timeScale = 1;

        var sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneIndex >= SceneManager.sceneCount)
        {
            sceneIndex = 0;
        }
        SceneManager.LoadScene(sceneIndex);
        //load next level
    }
}
