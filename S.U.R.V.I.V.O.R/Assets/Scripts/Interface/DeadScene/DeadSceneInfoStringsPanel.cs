using System.Collections.Generic;
using TheRevenantsAge;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.DeadScene
{
    public class DeadSceneInfoStringsPanel : MonoBehaviour
    {
        private const string PREFAB_PATH = @"Interface\Prefabs\DeadScene\Info";
        [SerializeField] private Transform parentTransformToItems;
        public void Init(List<StatisticCharacteristic> characteristicsToAdd)
        {
            foreach (var characteristic in characteristicsToAdd)
            {
                var item = Instantiate(Resources.Load(PREFAB_PATH), parentTransformToItems);
                item.GetComponent<DeadSceneInfoString>().Init(characteristic);
            }
        }
    }
}