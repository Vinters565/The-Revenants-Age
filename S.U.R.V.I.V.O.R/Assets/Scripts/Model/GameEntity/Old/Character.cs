// using System;
// using System.Collections.Generic;
// using System.Linq;
// using _3DCharacterExtensions;
// using _3DCharacterExtensions.ForAnimations;
// using Inventory;
// using Inventory.SpecialCells;
// using Model.GameEntity.Characters.CharacterSkills;
// using Model.Items;
// using Model.Items.Cloths;
// using Model.Items.Medicine;
// using Model.Items.Weapons;
// using Model.Items.Weapons.MeleeWeapons;
// using Model.Locations;
// using Model.SaveSystem;
// using Model.SaveSystem.States;
// using Model.Statistics;
// using Model.Weapons;
// using UnityEngine;
//
// namespace Model.GameEntity.Characters
// {
//     [RequireComponent(typeof(Saved))]
//     public class Character : EntityWithWeapon
//     {
//         public static Character DefaultPrefab2D =>
//             Resources.Load<Character>($"Characters/DefaultCharacter");
//
//         public static Character DefaultPrefab3D =>
//             Resources.Load<Character>("Characters/DefaultCharacter");
//
//         //Боевые характиристики
//         private int actionPoints = 3;
//
//         private int turnPoints = 1;
//         //
//
//         //Характеристики умений
//         public int NoiceModifier { get; set; } //На это значение умножается шум от ударов рукопашкой и лута
//
//         public int MaxPeopleInGroup { get; set; }
//         //
//
//         private CharacterStatistics characterStatistics;
//
//         [SerializeField] private Sprite sprite;
//         [SerializeField] private string firstName;
//         [SerializeField] private string surname;
//
//         private Gun primaryGun;
//         private Gun secondaryGun;
//         private MeleeWeapon meleeWeapon;
//
//         private Skills skills;
//         private ChosenWeaponTypes chosenWeaponType;
//
//         private CharacterAnimationController characterAnimationController;
//         private CharacterClothesController clothesController;
//
//         public event Action<Gun, GunType> GunsChanged;
//         public event Action<Weapon, Weapon> ChosenWeaponChanged;
//         public event Action<MeleeWeapon> MeleeWeaponChanged;
//         public event Action RestoreEnd;
//
//         protected override void Awake()
//         {
//             base.Awake();
//             
//             skills = new Skills(this);
//
//             characterAnimationController = GetComponent<CharacterAnimationController>();
//             clothesController = GetComponent<CharacterClothesController>();
//             characterStatistics = new CharacterStatistics();
//         }
//
//         private void OnEnable()
//         {
//             ManBody.HpChanged += OnHpChanged;
//             ManBody.HungerChanged += OnFoodChanged;
//             ManBody.WaterChanged += OnWaterChanged;
//             ManBody.EnergyChanged += OnEnergyChanged;
//         }
//
//         private void OnDisable()
//         {
//             ManBody.HpChanged -= OnHpChanged;
//             ManBody.HungerChanged -= OnFoodChanged;
//             ManBody.WaterChanged -= OnWaterChanged;
//             ManBody.EnergyChanged -= OnEnergyChanged;
//         }
//
//         public ManBody ManBody => (ManBody) Body;
//         public Sprite Sprite => sprite;
//         public string FirstName => firstName;
//         public string Surname => surname;
//         public Skills Skills => skills;
//         public ChosenWeaponTypes ChosenWeaponTypes => chosenWeaponType;
//
//         public int ActionPoints
//         {
//             get => actionPoints;
//             set => actionPoints = value;
//         }
//
//         public int TurnPoints
//         {
//             get => turnPoints;
//             set => turnPoints = value;
//         }
//
//         public override Aimer Aimer
//         {
//             get
//             {
//                 if (ChosenWeapon != null)
//                     return ChosenWeapon.Aimer;
//                 return base.Aimer;
//             }
//         }
//
//         public Gun PrimaryGun
//         {
//             get => primaryGun;
//             set
//             {
//                 var oldGun = primaryGun;
//                 primaryGun = value;
//                 GunsChanged?.Invoke(oldGun, GunType.PrimaryGun);
//             }
//         }
//
//         public Gun SecondaryGun
//         {
//             get => secondaryGun;
//             set
//             {
//                 var oldGun = secondaryGun;
//                 secondaryGun = value;
//                 GunsChanged?.Invoke(oldGun, GunType.SecondaryGun);
//             }
//         }
//
//         public MeleeWeapon MeleeWeapon
//         {
//             get => meleeWeapon;
//             set
//             {
//                 meleeWeapon = value;
//                 MeleeWeaponChanged?.Invoke(meleeWeapon);
//             }
//         }
//
//         public CharacterStatistics CharacterStatistics => characterStatistics;
//
//         public void SetChosenWeapon(ChosenWeaponTypes value)
//         {
//             if (!characterAnimationController.IsCanSetWeapon) return;
//
//             Weapon oldWeapon = chosenWeapon;
//             if (oldWeapon != null)
//             {
//                 Initiative -= oldWeapon.Ergonomics / 5;
//                 oldWeapon.Owner = null;
//             }
//
//             chosenWeaponType = value;
//
//             switch (chosenWeaponType)
//             {
//                 case ChosenWeaponTypes.Primary:
//                     chosenWeapon = PrimaryGun;
//                     break;
//                 case ChosenWeaponTypes.Secondary:
//                     chosenWeapon = SecondaryGun;
//                     break;
//                 case ChosenWeaponTypes.Melee:
//                     chosenWeapon = MeleeWeapon;
//                     break;
//                 case ChosenWeaponTypes.None:
//                     chosenWeapon = null;
//                     break;
//             }
//
//
//             Weapon newWeapon = chosenWeapon;
//
//             if (newWeapon != null)
//             {
//                 Initiative += newWeapon.Ergonomics / 5;
//                 newWeapon.Owner = this;
//             }
//
//             ChosenWeaponChanged?.Invoke(oldWeapon, newWeapon);
//         }
//
//         public void OnTurnEnd()
//         {
//             ManBody.OnTurnEnd();
//             foreach (var bodyPart in ManBody.BodyParts)
//             {
//                 bodyPart.Health.OnTurnEnd();
//             }
//
//             characterStatistics.daysAlive += 1;
//         }
//
//         public void Eat(EatableFood food)
//         {
//             ManBody.Energy += food.Data.DeltaEnergy;
//             ManBody.Water += food.Data.DeltaWater;
//             ManBody.Hunger += food.Data.DeltaHunger;
//             Destroy(food.gameObject);
//         }
//
//         public IEnumerable<InventoryItem> Cook(CookableFood food)
//         {
//             //TODO Добавить опыт к навыку готовки
//             Skills.Cooking.Develop(10);
//             return food.Cook();
//         }
//
//         //Сделать методом расширением
//         public void Heal(Medicine medicine, Character characterToHeal, bool shouldRemoveProperties, bool shouldHealHp)
//         {
//             //TODO добавить вляиние навыка медицины на лечение и добавление опыта к скиллу хила
//             var orderedBp = characterToHeal.ManBody.BodyParts.OrderBy(x => x.MaxHp - x.Hp);
//             foreach (var bodyPart in orderedBp)
//             {
//                 if (shouldRemoveProperties)
//                     characterStatistics.healedHp += medicine.HealOnlyProperties(bodyPart);
//                 if (medicine == null)
//                     return;
//             }
//
//             foreach (var bodyPart in orderedBp)
//             {
//                 if (shouldHealHp)
//                     characterStatistics.healedHp += medicine.HealOnlyHp(bodyPart);
//                 if (medicine == null)
//                     return;
//             }
//         }
//         // сделать методом расширением
//         public void Heal(Medicine medicine, BodyPart bodyPartToHeal, bool shouldRemoveProperties, bool shouldHealHp)
//         {
//             //TODO добавить вляиние навыка медицины на лечение
//             if (shouldHealHp)
//                 medicine.HealOnlyHp(bodyPartToHeal);
//             if (medicine == null)
//                 return;
//             if (shouldRemoveProperties)
//                 medicine.HealOnlyProperties(bodyPartToHeal);
//         }
//
//         public IEnumerable<InventoryItem> Loot(LocationData infoAboutLocation, int amountOfTimes)
//         {
//             //TODO Добавить опыт к навыку лутания в зависмости от редкости найденной вещи
//             for (int i = 0; i < amountOfTimes; i++)
//             {
//                 ManBody.Energy -= 1;
//                 var item = infoAboutLocation.GetLoot();
//                 if (item != null)
//                 {
//                     characterStatistics.lootFound += 1;
//                     Skills.Looting.Develop(5);
//                 }
//
//                 yield return item;
//             }
//
//             var rnd = new System.Random();
//
//             if (rnd.NextDouble() <= Skills.Looting.ChanceToFindExtraItem)
//             {
//                 yield return infoAboutLocation.GetLoot();
//             }
//
//             if (Skills.Looting.CanFreeLoot)
//             {
//                 ManBody.Energy++;
//             }
//         }
//
//         public override void Attack(Vector3 targetPoint)
//         {
//             base.Attack(targetPoint);
//             if (ChosenWeapon != null)
//                 ChosenWeapon.Attack(targetPoint, skills);
//             //TODO получить результаты попадания и добавить опыт к скиллу, через вызов публичного метода у Skills
//         }
//         
//         // сделать мотодом расширением
//         public IEnumerable<T> GetItemsFromAllInventoriesByType<T>()
//             where T : MonoBehaviour
//         {
//             var result = new List<T>();
//             result.AddRange(ManBody.Chest.GetItemsFromInventory()
//                 .Where(x => x.GetComponent<T>())
//                 .Select(x => x.GetComponent<T>()));
//
//             result.AddRange(ManBody.LeftLeg.GetItemsFromInventory()
//                 .Where(x => x.GetComponent<T>())
//                 .Select(x => x.GetComponent<T>()));
//
//
//             return result;
//         }
//         
//         // сделать методом расширением
//         public IEnumerable<InventoryState> GetAllInventoryStates()
//         {
//             var result = new List<InventoryState>();
//             result.AddRange(ManBody.Chest.GetAllInventoryStates());
//             result.AddRange(ManBody.LeftLeg.GetAllInventoryStates());
//
//             return result;
//         }
//
//         #region Statistics
//
//         private void OnHpChanged(float oldValue, float newValue)
//         {
//             var difference = newValue - oldValue;
//             if (difference < 0)
//                 characterStatistics.lostHp -= difference;
//         }
//
//         private void OnFoodChanged(int oldValue, int newValue)
//         {
//             var difference = newValue - oldValue;
//             if (difference < 0)
//                 characterStatistics.foodLost -= difference;
//         }
//
//         private void OnEnergyChanged(int oldValue, int newValue)
//         {
//             var difference = newValue - oldValue;
//             if (difference < 0)
//                 characterStatistics.energyLost -= difference;
//         }
//
//         private void OnWaterChanged(int oldValue, int newValue)
//         {
//             var difference = newValue - oldValue;
//             if (difference < 0)
//                 characterStatistics.waterLost -= difference;
//         }
//
//         #endregion
//
//
//         public CharacterState CreateState()
//         {
//             return new CharacterState()
//             {
//                 characterStatistics = characterStatistics,
//                 resourcesPath = GetComponent<Saved>().ResourcesPath,
//                 firstName = FirstName,
//                 surName = Surname,
//                 manBody = (ManBodyState) ManBody.CreateState(),
//                 skills = skills.CreateState(),
//                 hat = ManBody.Head.Hat?.GetComponent<BaseItem>().CreateState(),
//                 underwear = ManBody.Chest.Underwear?.GetComponent<BaseItem>().CreateState(),
//                 jacket = ManBody.Chest.Jacket?.GetComponent<BaseItem>().CreateState(),
//                 backpack = ManBody.Chest.Backpack?.GetComponent<BaseItem>().CreateState(),
//                 vest = ManBody.Chest.Vest?.GetComponent<BaseItem>().CreateState(),
//                 boots = ManBody.LeftLeg.Boots?.GetComponent<BaseItem>().CreateState(),
//                 pants = ManBody.LeftLeg.Pants?.GetComponent<BaseItem>().CreateState(),
//                 primaryGun = PrimaryGun?.GetComponent<BaseItem>().CreateState(),
//                 secondaryGun = SecondaryGun?.GetComponent<BaseItem>().CreateState(),
//                 meleeWeapon = MeleeWeapon?.GetComponent<BaseItem>().CreateState()
//             };
//         }
//
//         public void Restore(CharacterState state)
//         {
//             if (state == null) return;
//
//             firstName = state.firstName;
//             surname = state.surName;
//
//             characterStatistics = state.characterStatistics;
//
//             if (PrimaryGun != null)
//                 Destroy(PrimaryGun.gameObject);
//             if (SecondaryGun != null)
//                 Destroy(SecondaryGun.gameObject);
//             if (MeleeWeapon != null)
//                 Destroy(MeleeWeapon.gameObject);
//             foreach (var clothe in ManBody.GetClothes())
//                 Destroy(clothe.gameObject);
//
//             if (state.manBody != null)
//                 ManBody.Restore(state.manBody);
//             if (state.skills != null)
//                 skills.Restore(state.skills);
//
//             WearClothes(state.hat);
//             WearClothes(state.underwear);
//             WearClothes(state.jacket);
//             WearClothes(state.backpack);
//             WearClothes(state.vest);
//             WearClothes(state.boots);
//             WearClothes(state.pants);
//
//             EquipWeapon(state.primaryGun);
//             EquipWeapon(state.secondaryGun);
//             EquipWeapon(state.meleeWeapon);
//
//
//             if (Game.Is3D)
//             {
//                 characterAnimationController.EquipAllWeapons();
//             }
//
//             RestoreEnd?.Invoke();
//
//             void EquipWeapon(ItemState weaponState)
//             {
//                 if (weaponState is null) return;
//
//                 var weaponObj = Game.Is3D
//                     ? weaponState.GameObj3D
//                     : weaponState.GameObj2D;
//
//                 if (weaponObj is null) return;
//                 var gun = weaponObj.GetComponent<Gun>();
//                 if (gun != null)
//                     switch (gun.Data.GunType)
//                     {
//                         case GunType.PrimaryGun:
//                             PrimaryGun = gun;
//                             return;
//                         case GunType.SecondaryGun:
//                             SecondaryGun = gun;
//                             return;
//                     }
//
//                 var meleeWeapon = weaponObj.GetComponent<MeleeWeapon>();
//                 if (meleeWeapon != null)
//                     MeleeWeapon = meleeWeapon;
//             }
//
//             void WearClothes(ItemState clothesState)
//             {
//                 if (clothesState is null) return;
//
//                 var clothesObj = Game.Is3D
//                     ? clothesState.GameObj3D
//                     : clothesState.GameObj2D;
//
//                 if (clothesObj is null) return;
//                 var cloth = clothesObj.GetComponent<Clothes>();
//                 clothesObj.ItemOwner = this;
//                 ManBody.Wear(cloth);
//                 if (Game.Is3D)
//                     clothesController.Wear3DClothes(cloth);
//             }
//         }
//     }
// }