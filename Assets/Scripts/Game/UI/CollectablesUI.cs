using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectablesUI : MonoBehaviour
{
    public static CollectablesUI instance;

    public Animation anim;
    public Animation popAnim;
    public float speedMultiplier = 1;
    public TextMeshProUGUI text;
    public TextMeshProUGUI popText;
    private float curAmmount;
    private float targetAmmount;

    private float valueShown;

    private void Awake()
    {
        instance = this;
        curAmmount = PlayerController.Score;
    }
    public void UpdateText(int amount, int value)
    {
        if (popAnim.isPlaying)
        {
            valueShown += value;
            popAnim.Stop();
            popAnim.Play();
        }
        else
        {
            popAnim.Play();
            valueShown = value;
        }
        popText.SetText("+" + valueShown);

        targetAmmount = amount;
        anim.Stop();
        anim.Play();
    }

    private void Update()
    {
        var diff = targetAmmount - curAmmount;
        curAmmount = Mathf.MoveTowards(curAmmount, targetAmmount, diff * speedMultiplier * Time.deltaTime);
        text.SetText(curAmmount.ToString("0"));
    }
}
