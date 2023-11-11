using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SegmentProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Color segmentBorderColor;
    [SerializeField] private Color segmentFillColor;
    [SerializeField] private int maxValue = 10;
    [SerializeField] private int value;

    public void Init(int valueToInit)
    {
        for(var i = 0; i < maxValue; i++)
        {
            var rectSize = GetComponent<RectTransform>().rect.size;
            GetComponent<GridLayoutGroup>().cellSize = new Vector2(rectSize.x / (float)maxValue, rectSize.y);
            var segmentObj = Instantiate(segmentPrefab, transform);
            var segment = segmentObj.transform;
            segment.GetComponent<Image>().color = segmentBorderColor;
            segment.Find("Fill").transform.GetComponent<Image>().color = segmentFillColor;
            if (i >= value)
                segment.gameObject.SetActive(false);
        }
        SetValue(valueToInit);
    }

    public void SetValue(int value)
    {
        if (value > maxValue)
            throw new ArgumentException($"Значение больше максимального (Передано: {value} > {maxValue})");
        var i = 0;
        foreach (Transform segmentObj in transform)
        {
            segmentObj.gameObject.SetActive(i < value);
            i++;
            if(i >= maxValue)
                break;
        }
    }
}
