using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBarScript : MonoBehaviour
{
    [SerializeField] private Color bigValuesColor;
    [SerializeField] private Color mediumValuesColor;
    [SerializeField] private Color smallValuesColor;
    [SerializeField] private float maxValue = 10;
    [SerializeField] private float value;
    [SerializeField] private TextMeshProUGUI valueTextField;

    public TextMeshProUGUI ValueTextField => valueTextField;

    private RectTransform valueRect;

    public void Init(float maxValue)
    {
        var rect = transform.Find("ValueRect");
        valueRect = rect.GetComponent<RectTransform>();
        rect.GetComponent<Image>().color = bigValuesColor;
        var localScale = valueRect.localScale;
        localScale = new Vector3(2* value / maxValue, localScale.y, localScale.z);
        valueRect.localScale = localScale;
        this.maxValue = maxValue;
        SetValue(maxValue);
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        valueTextField.text = Math.Truncate(value).ToString(CultureInfo.InvariantCulture);
        var localScale = valueRect.localScale;
        localScale = new Vector3(2* value / maxValue, localScale.y, localScale.z);
        valueRect.localScale = localScale;
        var difference = value / maxValue;
        switch (difference)
        {
            case > 0.9f:
                valueRect.GetComponent<Image>().color = bigValuesColor;
                break;
            case > 0.6f:
                valueRect.GetComponent<Image>().color = mediumValuesColor;
                break;
            case > 0.4f:
                valueRect.GetComponent<Image>().color = smallValuesColor;
                break;
            case > 0.3f:
                valueRect.GetComponent<Image>().color = Color.black;
                break;
        }
        
    }
}
