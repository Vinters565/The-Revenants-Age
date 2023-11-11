using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointsView : MonoBehaviour
{
    private SegmentProgressBar progressBar;
    private void Awake()
    {
        progressBar = GetComponent<SegmentProgressBar>();
        progressBar.Init(3);
    }

    public void Redraw(int current, int max)
    {
        progressBar.SetValue(current);
    }
}
