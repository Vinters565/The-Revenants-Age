using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

[System.Serializable]
public class Town : MonoBehaviour
{
    [SerializeField] private TownWindow townWindow;

    [SerializeField] private List<Trader> traders;

    public List<Trader> Traders => traders;

    public void Open()
    {
        townWindow.Open(this);
    }
}
