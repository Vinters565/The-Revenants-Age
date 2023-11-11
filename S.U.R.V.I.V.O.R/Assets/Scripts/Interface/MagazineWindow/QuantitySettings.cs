using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class QuantitySettings : MonoBehaviour
{
    private TMP_InputField inputField;
    private int minValue = 0;
    private int maxValue;

    [SerializeField] private Button actionButton;
    public Button ActionButton => actionButton;

    public int MaxValue
    {
        set => maxValue = value;
    }

    public int GetValue() => Int32.Parse(inputField.text);
    
    public void ResetValue() => transform.Find("InputField").GetComponent<TMP_InputField>().text = minValue.ToString();

    private void Awake()
    {
        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        inputField.text = minValue.ToString();
        inputField.onValueChanged.AddListener(delegate(string arg0)
        {
            if (!string.IsNullOrEmpty(arg0))
                inputField.text = Math.Clamp(Int32.Parse(inputField.text), minValue, maxValue).ToString();
            
            CheckButtonInteractivity();
        });
        CheckButtonInteractivity();
        ActionButton.onClick.AddListener(CheckInputFieldValidity);
    }

    private void CheckButtonInteractivity()
    {
        if (Int32.Parse(inputField.text) == minValue)
            actionButton.interactable = false;
        else
            actionButton.interactable = true;
    }

    public void IncreaseValue(int amount)
    {
        inputField.text = Math.Clamp(Int32.Parse(inputField.text) + amount, minValue, maxValue).ToString();
    }

    public void DecreaseValue(int amount)
    {
        inputField.text = Math.Clamp(Int32.Parse(inputField.text) - amount, minValue, maxValue).ToString();
    }

    public void IncreaseValueFull()
    {
        IncreaseValue(maxValue);
    }

    public void DecreaseValueFull()
    {
        DecreaseValue(maxValue);
    }
    
    public void SetInteractable(bool interactable)
    {
        if (!interactable)
        {
            GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = false);
            GetComponentInChildren<TMP_InputField>().interactable = false;
            return;
        }
        GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        GetComponentInChildren<TMP_InputField>().interactable = true;
    }

    public void CheckInputFieldValidity()
    {
        if (GetValue() > maxValue)
            inputField.text = maxValue.ToString();
    }
}