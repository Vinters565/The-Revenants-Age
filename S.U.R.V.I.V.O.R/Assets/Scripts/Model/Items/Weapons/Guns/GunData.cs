using System.Collections.Generic;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New GunData", menuName = "Data/Gun Data", order = 50)]
    public sealed class GunData : ScriptableObject
    {
        [SerializeField] private int fireRate;
        [SerializeField] private float spreadSizeOnOptimalFireDistance;//Точность
        [SerializeField] private float extraDamage;
        [SerializeField] private float deltaRecoil;//Изменение отдачи патронов
        [SerializeField] private float optimalFireDistanceBegin;
        [SerializeField] private float optimalFireDistanceEnd;
    
        [SerializeField] private float ergonomics; //Чем выше, тем больше негативное влияние на Mobility класса персонажа
        [SerializeField] private Caliber caliber;
        [SerializeField] private GunType gunType;
        [SerializeField] private List<GunModuleType> availableGunModules;

        public GunData(int fireRate, float spreadSizeOnOptimalFireDistance, float extraDamage, float deltaRecoil, float optimalFireDistanceBegin, float optimalFireDistanceEnd, float ergonomics, Caliber caliber, GunType gunType, List<GunModuleType> availableGunModules)
        {
            this.fireRate = fireRate;
            this.spreadSizeOnOptimalFireDistance = spreadSizeOnOptimalFireDistance;
            this.extraDamage = extraDamage;
            this.deltaRecoil = deltaRecoil;
            this.optimalFireDistanceBegin = optimalFireDistanceBegin;
            this.optimalFireDistanceEnd = optimalFireDistanceEnd;
            this.ergonomics = ergonomics;
            this.caliber = caliber;
            this.gunType = gunType;
            this.availableGunModules = availableGunModules;
        }

        public int FireRate => fireRate;
        public float SpreadSizeOnOptimalFireDistance => spreadSizeOnOptimalFireDistance;
        public float ExtraDamage => extraDamage;

        public float DeltaRecoil => deltaRecoil;

        public float OptimalFireDistanceBegin => optimalFireDistanceBegin;

        public float OptimalFireDistanceEnd => optimalFireDistanceEnd;

        public float Ergonomics => ergonomics;
        public Caliber Caliber => caliber;
        public GunType GunType => gunType;
        public IReadOnlyCollection<GunModuleType> AvailableGunModules => availableGunModules;
    }
}