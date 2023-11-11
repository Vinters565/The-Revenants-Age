using TheRevenantsAge;
using UnityEngine;
using Random = System.Random;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New Ammo", menuName = "Data/Ammo", order = 50)]
    public class SingleAmmo: SavedScriptableObject
    {
        private static Random rnd = new ();
    
        /////////////////////////////// Характеристики указываются не для одного патрона, а для одного поражающего элемента ///////////////////////////////////////////
    
        [SerializeField] [Range(0,1)] private float keneeticDamage;//Какой процент урона точно пройдет по телу
        [SerializeField] [Range(0,1)] private float armorPenetratingChance;//Шанс пробить броню
        [SerializeField] [Range(0,1)] private float underArmorDamage;//Какой процент урона пройдет по телу при непробитии
        [SerializeField] [Range(0,1)] private float upperArmorDamage;//Какой процент урона пройдет по телу при непробитии
        [SerializeField] [Min(0)] private float fullDamage;//Полный урон, наносится при отсутствии брони
        [SerializeField] [Range(0,1)] private float bleedingChance;//Шанс кровотечения, равен 0 при непробитии
        [SerializeField] [Range(0,1)] private float boneBrokingChance;//Шанс перелома кости, умножается на 1.3 при непробитии
        [SerializeField] [Min(0)] private float armorDamageOnPenetration;//Урон броне при непробитии
        [SerializeField] [Min(0)] private float armorDamageOnNonPenetration;//Урон броне при пробитии
    
        [SerializeField] [Min(0)] private float deltaOptimalFireDistanceBegin;//Изменение нижнего порога оптимальной дистанции
        [SerializeField] [Min(0)] private float deltaOptimalFireDistanceEnd;//Изменение верхнего порога оптимальной дистанции
        [SerializeField] [Min(0)] private float deltaSpreadSizeOnOptimalFireDistance;//Изменение круга разброса на оптимальной дистанции
    
        [SerializeField] [Min(0)] private float recoil;//Отдача
        [SerializeField] [Min(0)] private float amountOfBullets;//Количество поражающих элементов
    
        [SerializeField] private Caliber caliber;
        [SerializeField] private string typeOfSingleAmmo;

        public Caliber Caliber => caliber;

        public float KeneeticDamage => keneeticDamage;

        public float Recoil => recoil;

        public string TypeOfSingleAmmo => typeOfSingleAmmo;

        public float AmountOfBullets => amountOfBullets;

        public float ArmorPenetratingChance => armorPenetratingChance;

        public float UnderArmorDamage => underArmorDamage;

        public float UpperArmorDamage => upperArmorDamage;

        public float FullDamage => fullDamage * (rnd.Next(90,110) / 100f);

        public float BleedingChance => bleedingChance;

        public float BoneBrokingChance => boneBrokingChance;

        public float ArmorDamageOnPenetration => armorDamageOnPenetration;

        public float ArmorDamageOnNonPenetration => armorDamageOnNonPenetration;

        public float DeltaOptimalFireDistanceBegin => deltaOptimalFireDistanceBegin;

        public float DeltaOptimalFireDistanceEnd => deltaOptimalFireDistanceEnd;

        public float DeltaSpreadSizeOnOptimalFireDistance => deltaSpreadSizeOnOptimalFireDistance;
    }
}
