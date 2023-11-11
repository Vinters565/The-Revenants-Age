using Inventory.SpecialCells;
using UnityEngine;

[CreateAssetMenu(fileName = "New GunModuleData", menuName = "Data/GunModule Data", order = 50)]
public class GunModuleData : ScriptableObject
{
    [SerializeField] private float deltaRecoil;
    [SerializeField] private float deltaSpreadSizeOnOptimalFireDistance;
    [SerializeField] private float deltaNoise;
    [SerializeField] private float deltaAverageDistanceBegin;
    [SerializeField] private float deltaAverageDistanceEnd;
    [SerializeField] private float deltaDamage;
    [SerializeField] private float deltaErgonomics;
    [SerializeField] private GunModuleType moduleType;
    
    public float DeltaRecoil => deltaRecoil;
    public float DeltaSpreadSizeOnOptimalFireDistance => deltaSpreadSizeOnOptimalFireDistance;
    public float DeltaNoise => deltaNoise;
    public float DeltaAverageDistanceBegin => deltaAverageDistanceBegin;
    public float DeltaAverageDistanceEnd => deltaAverageDistanceEnd;
    public float DeltaDamage => deltaDamage;
    public float DeltaErgonomics => deltaErgonomics;
    public GunModuleType ModuleType => moduleType;
}