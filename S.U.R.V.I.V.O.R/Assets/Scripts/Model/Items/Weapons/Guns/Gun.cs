using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Extension;
using Interface;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;
using Random = System.Random;


public abstract class Gun : Weapon, ITooltipPart, IBaseItemComponent
{
    protected static Random rnd;
    protected const float RECOIL_MULTIPLIER = 0.05f;
    protected const float BONE_BROCKING_ON_NON_PENETRATION_MODIFIER = 1.3f;

    //EXP
    protected const float GIVED_EXP_FOR_ONE_DAMAGE = 0.1f;
    protected const float GIVED_EXP_FOR_PENETRATION = 1f;
    protected const float GIVED_EXP_FOR_NON_PENETRATION = 0.7f;
    protected const float GIVED_EXP_FOR_BLEEDING = 1f;
    protected const float GIVED_EXP_FOR_BROKING = 1f;

    protected const float GIVED_EXP_FOR_MISS = 0.5f;
    //

    [SerializeField] protected GunData data;

    protected FireType currentFireType;
    private float currentRecoil;
    private Dictionary<GunModuleType, GunModule> gunModules;
    private Aimer aimer;
    private Transform gunPoint;
    private GunAnimation gunAnimation;
    
    
    private TriggerAudioOn3DScene audioTrigger;

    public Magazine CurrentMagazine { get; protected set; }
    public event Action<GunModuleType> ModulesChanged;
    public GunData Data => data;
    public override Aimer Aimer => aimer;

    //Боевые характеристики
    public override float Ergonomics
    {
        get
        {
            return data.Ergonomics + gunModules
                .SelectNotNullValues()
                .Sum(x => x.Data.DeltaErgonomics);
        }
    }

    //

    public Transform GunPoint
    {
        get
        {
            if (gunPoint == null)
            {
                gunPoint = transform.Find("GunPoint");
                if (gunPoint == null)
                    gunPoint = transform;
            }

            return gunPoint;
        }
    }

    public IReadOnlyDictionary<GunModuleType, GunModule> GunModules => gunModules;

    protected float OptimalFireDistanceBegin =>
        data.OptimalFireDistanceBegin + gunModules
            .SelectNotNullValues()
            .Sum(x => x.Data.DeltaAverageDistanceBegin);

    protected float OptimalFireDistanceEnd =>
        data.OptimalFireDistanceEnd + gunModules
            .SelectNotNullValues()
            .Sum(x => x.Data.DeltaAverageDistanceEnd);

    protected float SpreadSizeOnOptimalFireDistance =>
        data.SpreadSizeOnOptimalFireDistance + gunModules
            .SelectNotNullValues()
            .Sum(x => x.Data.DeltaSpreadSizeOnOptimalFireDistance);

    protected float DeltaRecoil =>
        data.DeltaRecoil + gunModules
            .SelectNotNullValues()
            .Sum(x => x.Data.DeltaRecoil);

    private float CurrentRecoil
    {
        get => currentRecoil;
        set => currentRecoil = Math.Max(0, value);
    }

    protected override void Awake()
    {
        base.Awake();
        rnd = new Random();
        gunModules = new Dictionary<GunModuleType, GunModule>
        {
            {GunModuleType.Grip, null},
            {GunModuleType.Scope, null},
            {GunModuleType.Shutter, null},
            {GunModuleType.Spring, null},
            {GunModuleType.Suppressor, null},
            {GunModuleType.Tactical, null},
            {GunModuleType.Magazine, null}
        };

        aimer = GetComponent<GunAimer>();
        gunAnimation = GetComponent<GunAnimation>();
        audioTrigger = GetComponent<TriggerAudioOn3DScene>();
    }

    public bool CheckGunModule(GunModuleType module) => data.AvailableGunModules.Contains(module);

    public virtual Magazine Reload(Magazine magazine)
    {
        if (CurrentMagazine == null)
        {
            CurrentMagazine = magazine;
            ModulesChanged?.Invoke(GunModuleType.Magazine);
            return null;
        }

        var result = CurrentMagazine;
        CurrentMagazine = magazine;
        ModulesChanged?.Invoke(GunModuleType.Magazine);
        return result;
    }

    protected abstract int GetAmountOfShots(CharacterSkills skills);

    public override void Attack(Vector3 targetPoint, CharacterSkills influenceOfSkills)
    {
        Debug.Log("WeaponAttack");
        var position = transform.position;
        for (var g = 0; g < GetAmountOfShots(influenceOfSkills); g++)
        {
            foreach (var dot in GetOneShot(targetPoint, out var ammo, influenceOfSkills))
            {
                Debug.Log("OneShot");
                var exp = 0.0f;
                var wasHitted =
                    Physics.Raycast(position, dot - position, out var hit); //TODO targetPoint поменять на dot
                if (!wasHitted)
                {
                    return;
                }

                Debug.Log("Shot By Gun");
                var target = hit.transform.gameObject.GetComponent<PointerToBodyPart>();

                gunAnimation.OnAttack(hit.point);//TODO Отображение !!!
                //gunAnimation.GUN_ANIMATION_MUZZLE_FLASH();//TODO Отображение !!!
                //gunAnimation.GUN_ANIMATION_BULLET_TRACER();//TODO Отображение !!!
                //if (target != null)
                //    GiveDamage(ammo, target.BodyPart, out exp);
                //else
                //{
                //    exp = GIVED_EXP_FOR_MISS;
                //}

                influenceOfSkills.AddExperienceToShootingSkill(this, exp);
            }
        }

        CurrentRecoil = 0;
        WhenAttacked?.Invoke();
    }

    public bool AddGunModule(GunModule newGunModule)
    {
        if (CheckGunModule(newGunModule.Data.ModuleType)
            && gunModules[newGunModule.Data.ModuleType] is null)
        {
            gunModules[newGunModule.Data.ModuleType] = newGunModule;
            ModulesChanged?.Invoke(newGunModule.Data.ModuleType);
            return true;
        }

        return false;
    }

    public bool RemoveGunModule(GunModule gunModule)
    {
        var boolean = gunModules[gunModule.Data.ModuleType] = null;
        ModulesChanged?.Invoke(gunModule.Data.ModuleType);
        return boolean;
    }

    protected (Vector3, Vector3, Vector3) GetNewBasis(Vector3 targetPoint)
    {
        //НАХОЖДЕНИЕ НОВОГО БАЗИСА
        var position = transform.position;
        var newK = targetPoint - position;
        var newI = new Vector3(0, -newK.z, newK.y);
        var newJ = Vector3.Cross(newI, newK);

        newK = newK.normalized;
        newI = newI.normalized;
        newJ = newJ.normalized;
        return (newI, newJ, newK);
    }

    protected Vector3 GetOffset(float maxRo, float spreadModifier, (Vector3, Vector3, Vector3) newBasis,
        float influenceOfAccuracy)
    {
        var ro = rnd.NextDouble() * maxRo - influenceOfAccuracy; // Дистанция от центра тем меньше, чем больше точность
        if (spreadModifier < 0 || spreadModifier > 1)
            throw new ArgumentException();
        ro -= rnd.NextDouble() * (maxRo * spreadModifier); // Увеличение разброса от отдачи
        var fi = rnd.NextDouble() * (2 * 3.14f);

        //Смещение в новых координатах
        var newX = (float) (ro * Math.Cos(fi));
        var newY = (float) (ro * Math.Sin(fi));
        var newZ = 0;

        var (newI, newJ, newK) = newBasis;

        //Смещение в старых координатах
        var offsetInOldCoordinates = newI * newX + newJ * newY + newK * newZ;

        return offsetInOldCoordinates;
    }

    protected ICollection<Vector3> GetOneShot(Vector3 targetPoint, out SingleAmmo ammo, CharacterSkills skills)
    {
        var distance = Vector3.Distance(transform.position, targetPoint);
        var influenceOfHandling = skills.GetSkillsInfluenceOnShot(this);

        if (CurrentMagazine is null)
        {
            ammo = null;
            return new List<Vector3>(){targetPoint};
            //TODO Осечка
        }
        else
        {
            ammo = CurrentMagazine.GetAmmo(); //Извлекаем патрон   
        }

        //if (ammo == null)
        //    th; //TODO Осечка

        var currentOptDistBegin = OptimalFireDistanceBegin + ammo.DeltaOptimalFireDistanceBegin -
                                  influenceOfHandling.DeltaOptimalDistance;
        var currentOptDistEnd = OptimalFireDistanceEnd + ammo.DeltaOptimalFireDistanceEnd +
                                influenceOfHandling.DeltaOptimalDistance;

        var distanceDifference =
            currentOptDistBegin <= distance && distance >= currentOptDistEnd
                ? 1
                : distance <= currentOptDistBegin
                    ? distance /
                      currentOptDistBegin //TODO сделать "экспоненциальное" увеличение с помощью коэффициента в степени
                    : currentOptDistEnd / distance; //TODO сделать линейное увеличение, как у конуса

        var spreadSize =
            SpreadSizeOnOptimalFireDistance +
            ammo.DeltaSpreadSizeOnOptimalFireDistance +
            +CurrentRecoil * RECOIL_MULTIPLIER; //Чем меньше точность, тем меньше круг

        CurrentRecoil += (ammo.Recoil + DeltaRecoil) * influenceOfHandling.RecoilModifier *
                         skills.Accuracy.RecoilModifier;

        var maxRo = spreadSize * (1 + (1 - distanceDifference));
        var newBasis = GetNewBasis(targetPoint);
        var result = new List<Vector3>();
        for (var z = 0; z < ammo.AmountOfBullets; z++)
            result.Add(
                GetOffset(maxRo, 0, newBasis, skills.Accuracy.DeltaAccuracy + influenceOfHandling.DeltaAccuracy) +
                targetPoint); //TODO inluenceOfAccuracy - влияние навыка Точность на разлет пуль в круге разброса
        return result;
    }

    protected virtual void
        GiveDamage(SingleAmmo ammo, BodyPart target, out float exp) //TODO переопределить для дробовиков
    {
        Debug.Log("Give Damage");
        target.Hp -= ammo.FullDamage * ammo.KeneeticDamage;
        if (target is BodyPathWearableClothes wear && wear.CurrentArmor > 0)
        {
            GiveDamageToBodyPartWithArmor(ammo, wear, out exp);
            return;
        }

        GiveDamageToBodyPartWithoutArmor(ammo, target, out exp);
    }

    protected virtual void GiveDamageToBodyPartWithArmor(SingleAmmo ammo, BodyPathWearableClothes wear, out float exp)
    {
        exp = 0f;
        var isPenetrated = rnd.NextDouble() <= ammo.ArmorPenetratingChance;
        if (isPenetrated)
        {
            exp += GIVED_EXP_FOR_PENETRATION;
            wear.DamageArmor(ammo.ArmorDamageOnPenetration);
            var damage = ammo.FullDamage * ammo.UpperArmorDamage;
            wear.Hp -= damage;
            exp += damage * GIVED_EXP_FOR_ONE_DAMAGE;
            
            var isBleeding = rnd.NextDouble() <= ammo.BleedingChance;
            if (isBleeding)
            {
                wear.Health.AddProperty(new Bleeding());
                exp += GIVED_EXP_FOR_BLEEDING;
            }

            var isBroking = rnd.NextDouble() <= ammo.BoneBrokingChance;
            if (isBroking)
            {
                wear.Health.AddProperty(new Broking());
                exp += GIVED_EXP_FOR_BROKING;
            }
        }
        else
        {
            exp += GIVED_EXP_FOR_NON_PENETRATION;
            wear.DamageArmor(ammo.ArmorDamageOnNonPenetration);
            var damage = ammo.FullDamage * ammo.UnderArmorDamage;
            wear.Hp -= damage;
            exp += damage * GIVED_EXP_FOR_ONE_DAMAGE;

            var isBroking = rnd.NextDouble() <= ammo.BoneBrokingChance * BONE_BROCKING_ON_NON_PENETRATION_MODIFIER;
            if (isBroking)
            {
                wear.Health.AddProperty(new Broking());
                exp += GIVED_EXP_FOR_BROKING;
            }
        }
    }

    protected virtual void GiveDamageToBodyPartWithoutArmor(SingleAmmo ammo, BodyPart target, out float exp)
    {
        var damage = ammo.FullDamage + ammo.FullDamage * ammo.KeneeticDamage;
        target.Hp -= damage;
        
        exp = 1f + damage * 0.1f;
        var isBleeding = rnd.NextDouble() <= ammo.BleedingChance;
        if (isBleeding)
            target.Health.AddProperty(new Bleeding());

        var isBroking = rnd.NextDouble() <= ammo.BoneBrokingChance;
        if (isBroking)
            target.Health.AddProperty(new Broking());
    }

    public TooltipPartDrawer GetTooltipPart()
    {
        var accuracyIcon = IconsHelper.GetCharacteristicIcon(IconsHelper.Characteristics.Accuracy);
        var recoil = IconsHelper.GetCharacteristicIcon(IconsHelper.Characteristics.Recoil);
        var ergonomics = IconsHelper.GetCharacteristicIcon(IconsHelper.Characteristics.Ergo);
        
        var part = TooltipPartDrawer.InitPart();
        part.AddMainText("Характеристики оружия");
        part.AddImageWithText($"Точность - {Data.SpreadSizeOnOptimalFireDistance}",accuracyIcon);
        part.AddImageWithText($"Отдача - {Data.DeltaRecoil}",recoil);
        part.AddImageWithText($"Эргономика - {Data.Ergonomics}",ergonomics);
        return part;
    }
    
    public ComponentState CreateState()
    {
        return new GunState
        {
            CurrentMagazine = CurrentMagazine?.GetComponent<BaseItem>().CreateState(),
            gunModules = gunModules?.Values
                .Select(x => x?.GetComponent<BaseItem>().CreateState())
                .Where(x => x != null)
                .ToArray()
        };
    }

    public void Restore(ComponentState state)
    {
        if (state is not GunState gunState) return;

        if (CurrentMagazine != null)
            Destroy(CurrentMagazine.gameObject);
        foreach (var pair in GunModules)
        {
            var gunModule = pair.Value;
            if (gunModule != null)
                Destroy(gunModule.gameObject);
        }

        var magState = gunState.CurrentMagazine;
        if (magState != null)
        {
            CurrentMagazine = Game.Is3D
                ? magState.InstantiateGameObj3D().GetComponent<Magazine>()
                : magState.InstantiateGameObj2D().GetComponent<Magazine>();
            if (Game.Is3D)
                CurrentMagazine.transform.SetParent(transform.Find("MagazineContainer"), false);
        }

        if (gunState.gunModules != null)
        {
            foreach (var module in gunState.gunModules)
            {
                var gunModule = Game.Is3D
                    ? module.InstantiateGameObj3D().GetComponent<GunModule>()
                    : module.InstantiateGameObj2D().GetComponent<GunModule>();

                gunModules[gunModule.Data.ModuleType] = gunModule;
            }
        }
    }
}