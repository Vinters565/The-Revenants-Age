using System;
using Interface.BodyIndicatorFolder;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerCharacteristicsPanel : MonoBehaviour
{
    private IGlobalMapCharacter character;

    public IGlobalMapCharacter Player
    {
        get => character;
        set
        {
            Unsubscribe();
            character = value;
            gameObject.SetActive(value != null);
            if (value == null) return;
            Subscribe();
            Init();
        }
    }

    [SerializeField] private Image Photo;
    [SerializeField] private TextMeshProUGUI food;
    [SerializeField] private SegmentProgressBar foodProgressBar;
    [SerializeField] private TextMeshProUGUI water;
    [SerializeField] private SegmentProgressBar waterProgressBar;
    [SerializeField] private TextMeshProUGUI energy;
    [SerializeField] private SegmentProgressBar energyProgressBar;

    [FormerlySerializedAs("name")] [SerializeField]
    private TextMeshProUGUI characterName;

    [SerializeField] private GameObject ifInactivePanel;
    [SerializeField] private PosNegPropertiesDrawer posNegPropertiesDrawer;
    [SerializeField] private ProgressBarScript hpProgressBar;
    
    private float dif;

    public void Init()
    {
        characterName.text = $"{Player.FirstName} {Player.SurName}";
        if (Photo != null)
            Photo.sprite = Player.Sprite;
        if (food != null)
            food.text = character.ManBody.Hunger.ToString();
        if (foodProgressBar != null)
            foodProgressBar.Init(character.ManBody.Hunger);
        if (water != null)
            water.text = character.ManBody.Water.ToString();
        if (waterProgressBar != null)
            waterProgressBar.Init(character.ManBody.Water);
        if (energy != null)
            energy.text = character.ManBody.Energy.ToString();
        if (energyProgressBar != null)
            energyProgressBar.Init(character.ManBody.Energy);
        if (hpProgressBar != null)
        {
            hpProgressBar.Init(character.ManBody.MaxHp);
            hpProgressBar.SetValue(character.ManBody.Hp);
        }

        if (posNegPropertiesDrawer is not null)
        {
            posNegPropertiesDrawer.Health = character.ManBody.Health;
        }
    }

    private void OnFoodChanged(int oldValue, int newValue)
    {
        if (food != null)
        {
            food.text = newValue.ToString();
            TextParticles.Instance.LaunchParticle(newValue - oldValue, food.transform.position, food);
        }

        if (foodProgressBar != null)
            foodProgressBar.SetValue(newValue);
    }

    private void OnWaterChanged(int oldValue, int newValue)
    {
        if (water != null)
        {
            water.text = newValue.ToString();
            TextParticles.Instance.LaunchParticle(newValue - oldValue, water.transform.position, water);
        }

        if (waterProgressBar != null)
            waterProgressBar.SetValue(newValue);
    }

    private void OnEnergyChanged(int oldValue, int newValue)
    {
        if (energy != null)
        {
            energy.text = newValue.ToString();
            TextParticles.Instance.LaunchParticle(newValue - oldValue, energy.transform.position, energy);
        }

        if (energyProgressBar != null)
            energyProgressBar.SetValue(newValue);
    }


    private void Subscribe()
    {
        if (character is null) return;
        character.ManBody.EnergyChanged += OnEnergyChanged;
        character.ManBody.WaterChanged += OnWaterChanged;
        character.ManBody.HungerChanged += OnFoodChanged;
        character.ManBody.WhenDamagedOrHealed += HealthChanged;
    }

    private void HealthChanged(float value, Vector3 pos)
    {
        if (hpProgressBar != null)
        {
            dif += value;
            hpProgressBar.SetValue(character.ManBody.Hp);
        }
    }

    private void Unsubscribe()
    {
        if (character is null) return;
        character.ManBody.EnergyChanged -= OnEnergyChanged;
        character.ManBody.WaterChanged -= OnWaterChanged;
        character.ManBody.HungerChanged -= OnFoodChanged;
        character.ManBody.WhenDamagedOrHealed -= HealthChanged;
    }

    private void LateUpdate()
    {
        if (dif == 0) return;
        TextParticles.Instance.LaunchParticle(dif, hpProgressBar.ValueTextField.transform.position,
            hpProgressBar.ValueTextField);
        dif = 0;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}