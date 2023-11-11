using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolAviabilityDrawer : MonoBehaviour
{
    [SerializeField] private Image aviablePanel;
    
    [SerializeField] private Image unaviablePanel;

    public Image AviablePanel => aviablePanel;

    public Image UnaviablePanel => unaviablePanel;

    public void ReDrawAviability(bool isAviable)
    {
        aviablePanel.gameObject.SetActive(isAviable);
        unaviablePanel.gameObject.SetActive(!isAviable);
    }
}
