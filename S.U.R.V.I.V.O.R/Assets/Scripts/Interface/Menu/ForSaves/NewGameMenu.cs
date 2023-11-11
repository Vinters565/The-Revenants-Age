using System;
using System.Linq;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
public class NewGameMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private GameObject dialogPanelOnGameSessionMach;

    private string initialSavePath;

    private void Awake()
    {
        initialSavePath = System.IO.Path.Join(
            Application.streamingAssetsPath,
            "InitialSaves/InitialSave.xml");
    }

    public void CreateNewGameSessionCondition()
    {
        if (GameSession.GetAllGameSession().Any(x => x.Name == input.text))
            dialogPanelOnGameSessionMach.gameObject.SetActive(true);
        else if (string.IsNullOrEmpty(input.text))
        {
            input.text = "Новая Игра";
        }
        else
            CreateNewGameSession();
    }

    public void CreateNewGameSession()
    {
        var newGameSession = new GameSession(input.text);
        newGameSession.Clear();
        GameSession.Current = newGameSession;
        try
        {
            var save = Serializer.ReadObject<GlobalMapState>(initialSavePath);
            GlobalMapController.SetState(save);  
        }
        catch (Exception)
        {
            // ignored
        }

        SceneTransition.LoadScene(SceneName.GlobalMapScene);
    }
}