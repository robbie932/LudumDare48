using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectablesUI : MonoBehaviour
{
    public static CollectablesUI instance;

    public Animation anim;
    public float speedMultiplier = 1;
    public TextMeshProUGUI text;
    private float curAmmount;
    private float targetAmmount;

    private void Awake()
    {
        instance = this;
        curAmmount = 0;
    }
    public void UpdateText(int amount)
    {
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
