using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    [SerializeField] private Text warningText;

    public string WarningText
    {
        get => warningText.text;
        set => warningText.text = value;
    }
}
