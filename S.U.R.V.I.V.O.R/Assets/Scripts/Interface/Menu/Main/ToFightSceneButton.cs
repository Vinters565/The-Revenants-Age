using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Interface.Menu.Main
{
    public class ToFightSceneButton: MonoBehaviour
    {
        [FormerlySerializedAs("fight")] [SerializeField] private FightInfo fightInfo;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
        }
    }
}