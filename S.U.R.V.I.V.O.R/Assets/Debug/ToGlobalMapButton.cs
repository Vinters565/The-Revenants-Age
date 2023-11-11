using System;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToGlobalMapButton : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("End Fight Scene");
        winPanel.SetActive(false);
        FightSceneController.Current.End(FightSceneController.Instance.FightEntityObjs
            .Where(c => c != null)
            .Where(c =>
            {
                var fightCharacter = c.GetComponent<FightSceneCharacter>();
                return fightCharacter.Type == CharacterType.Ally && fightCharacter.Alive;
            })
            .Select(c => c.GetComponent<FightSceneCharacter>().Entity as ICharacter)
            .ToList());
    }
}
