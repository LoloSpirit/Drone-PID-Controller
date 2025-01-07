using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataUI : MonoBehaviour
{
    public TMP_Text angleTxt;
    public TMP_Text targetTxt;
    public TMP_Text velocityTxt;

    public static DataUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetData(float angle, float target, float vel)
    {
        angleTxt.text = angle + "";
        targetTxt.text = target + "";
        velocityTxt.text = vel + "";
    }
}
