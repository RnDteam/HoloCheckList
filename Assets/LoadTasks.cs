﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTasks : MonoBehaviour {

    public bool IsFullPreflight;
    public Text preflightDisplay;
    private Card[] cards;

	void Start () {
        SetTaskFileName();

        cards = TextsBridge.GetCards();
    }

    public void SetFullPreflight()
    {
        IsFullPreflight = true;
        SetTaskFileName();
    }

    public void SetPartialPreflight()
    {
        IsFullPreflight = false;
        SetTaskFileName();
    }

    public void SetTaskFileName()
    {
        TextsBridge.SetTaskFileName(IsFullPreflight);
        preflightDisplay.text = "(" + (IsFullPreflight ? "אלמ ךרעמ" : "יקלח ךרעמ") + ")";

        LoadCards();
    }

    public void LoadCards()
    {
        cards = TextsBridge.LoadCards();
    }
}
