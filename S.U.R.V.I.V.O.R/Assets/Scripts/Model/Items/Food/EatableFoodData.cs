using UnityEngine;

[CreateAssetMenu(fileName = "New EatableFoodData", menuName = "Data/Eatable Food Data", order = 50)]
public class EatableFoodData: ScriptableObject
{
    [SerializeField] private int deltaHunger;
    [SerializeField] private int deltaWater;
    [SerializeField] private int deltaEnergy;

    public int DeltaHunger => deltaHunger;

    public int DeltaWater => deltaWater;

    public int DeltaEnergy => deltaEnergy;
}