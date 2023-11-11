using System.Collections.Generic;
using System.Linq;
using Interface.DeadScene;
using TheRevenantsAge;
using UnityEngine;

public class DeadSceneController : MonoBehaviour
{
    [SerializeField] private DeadSceneInfoStringsPanel playerInfoPanel;
    [SerializeField] private DeadSceneInfoStringsPanel groupInfoPanel;

    private void Awake()
    {
        var mainGroupStatistics = GameStatistics.groupDeadStatistics;
        var mainPlayerStatistics = GameStatistics.characterDeadStatistics;
        if (GameStatistics.groupDeadStatistics == null) return;
        var playerStatistics = new List<StatisticCharacteristic>()
        {
            new (mainPlayerStatistics.daysAlive.ToString(), "Дней прожито:",Resources.Load<Sprite>("Interface/Icons/Statistics/time")),
            new (mainPlayerStatistics.lootFound.ToString(), "Вещей найдено:",Resources.Load<Sprite>("Interface/Icons/Statistics/trash")),
            new (mainPlayerStatistics.monstersKilled.ToString(), "Монстров убито:",Resources.Load<Sprite>("Interface/Icons/Statistics/bull-skull")),
            new (mainPlayerStatistics.charactersKilled.ToString(), "Людей убито:",Resources.Load<Sprite>("Interface/Icons/Statistics/reaper")),
            new (mainPlayerStatistics.healedHp.ToString(), "Здоровья вылечено:",Resources.Load<Sprite>("Interface/Icons/Statistics/heal")),
            new (mainPlayerStatistics.lostHp.ToString(), "Здоровья потеряно:",Resources.Load<Sprite>("Interface/Icons/Properties/BloodBag")),
            new (mainPlayerStatistics.amountOfShots.ToString(), "Выстрелов сделано:",Resources.Load<Sprite>("Interface/Icons/Statistics/bullet")),
            new (mainPlayerStatistics.energyLost.ToString(), "Энергии потрачено:",Resources.Load<Sprite>("Interface/Icons/Properties/energyDebuff")),
            new (mainPlayerStatistics.foodLost.ToString(), "Пищи потрачено:",Resources.Load<Sprite>("Interface/Icons/Properties/Hunger")),
            new (mainPlayerStatistics.waterLost.ToString(), "Воды потрачено:",Resources.Load<Sprite>("Interface/Icons/Properties/Thirst")),
        };

        var summStat = new CharacterStatistics();
        summStat.daysAlive = mainGroupStatistics.characterStatisticsArray.Max(x => x.daysAlive);
        summStat.lootFound = mainGroupStatistics.characterStatisticsArray.Sum(x => x.lootFound);
        summStat.monstersKilled = mainGroupStatistics.characterStatisticsArray.Sum(x => x.monstersKilled);
        summStat.charactersKilled = mainGroupStatistics.characterStatisticsArray.Sum(x => x.charactersKilled);
        summStat.healedHp = mainGroupStatistics.characterStatisticsArray.Sum(x => x.healedHp);
        summStat.lostHp = mainGroupStatistics.characterStatisticsArray.Sum(x => x.lostHp);
        summStat.amountOfShots = mainGroupStatistics.characterStatisticsArray.Sum(x => x.amountOfShots);
        summStat.energyLost = mainGroupStatistics.characterStatisticsArray.Sum(x => x.energyLost);
        summStat.foodLost = mainGroupStatistics.characterStatisticsArray.Sum(x => x.foodLost);
        summStat.waterLost = mainGroupStatistics.characterStatisticsArray.Sum(x => x.waterLost);
        
        
        var groupStatistics = new List<StatisticCharacteristic>()
        {
            new (summStat.daysAlive.ToString(), "Дней прожито:",Resources.Load<Sprite>("Interface/Icons/Statistics/time")),
            new (summStat.lootFound.ToString(), "Вещей найдено:",Resources.Load<Sprite>("Interface/Icons/Statistics/trash")),
            new (summStat.monstersKilled.ToString(), "Монстров убито:",Resources.Load<Sprite>("Interface/Icons/Statistics/bull-skull")),
            new (summStat.charactersKilled.ToString(), "Людей убито:",Resources.Load<Sprite>("Interface/Icons/Statistics/reaper")),
            new (summStat.healedHp.ToString(), "Здоровья вылечено:",Resources.Load<Sprite>("Interface/Icons/Statistics/heal")),
            new (summStat.lostHp.ToString(), "Здоровья потеряно:",Resources.Load<Sprite>("Interface/Icons/Properties/BloodBag")),
            new (summStat.energyLost.ToString(), "Энергии потрачено:",Resources.Load<Sprite>("Interface/Icons/Properties/energyDebuff")),
            new (summStat.foodLost.ToString(), "Пищи потрачено:",Resources.Load<Sprite>("Interface/Icons/Properties/Hunger")),
            new (summStat.waterLost.ToString(), "Воды потрачено:",Resources.Load<Sprite>("Interface/Icons/Properties/Thirst")),
        };
        playerInfoPanel.Init(playerStatistics);
        groupInfoPanel.Init(groupStatistics);
    }
}
