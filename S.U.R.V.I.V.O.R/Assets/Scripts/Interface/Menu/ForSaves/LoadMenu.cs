using System;
using System.Linq;
using Interface;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    private GameSession chosenGameSession;
    private GameSave chosenSave;

    private GameObject choseGameScrollElement;
    private GameObject chosenSaveScrollElement;

    private Button gameScrollClickedButton;
    private Button saveScrollClickedButton;

    [SerializeField] private ScrollController gamesScroll;
    [SerializeField] private ScrollController savesScroll;

    [SerializeField] private RectTransform gameScrollElementPrefab;
    [SerializeField] private RectTransform saveScrollElementPrefab;

    [SerializeField] private GameObject dialogPanelDeleteGameSession;
    [SerializeField] private WarningPanel warningPanelOnDeleteGameSession;
    [SerializeField] private WarningPanel warningPanelOnDeleteLastSave;
    [SerializeField] private WarningPanel warningPanelOnLoadingCorruptedFile;


    private void OnDisable()
    {
        gamesScroll.Clear();
        savesScroll.Clear();
    }

    private void OnEnable()
    {
        var gameSessions = GameSession.GetAllGameSession().ToArray();
        foreach (var gameSession in gameSessions)
        {
            var newGameScrollElement = Instantiate(gameScrollElementPrefab);
            newGameScrollElement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = gameSession.Name;
            newGameScrollElement.transform.Find("DataTimeText").GetComponent<TextMeshProUGUI>().text = gameSession.DateTime;
            
            newGameScrollElement.GetComponent<Button>().onClick.AddListener(() =>
                ChooseGameSession(gameSession, newGameScrollElement));

            gamesScroll.AddElementBegin(newGameScrollElement);
            if (gameSession.Equals(GameSession.Current))
                ChooseGameSession(gameSession, newGameScrollElement);
        }
    }

    private void ChooseGameSession(GameSession gameSession, RectTransform elem)
    {
        choseGameScrollElement = elem.gameObject;
        var button = elem.GetComponent<Button>();
        if (gameScrollClickedButton != null)
            gameScrollClickedButton.interactable = true;
        button.interactable = false;
        gameScrollClickedButton = button;

        savesScroll.Clear();
        chosenGameSession = gameSession;
        chosenSaveScrollElement = null;
        chosenSave = null;

        foreach (var gameSave in gameSession.GameSaves)
        {
            var newSavesScrollElement = Instantiate(saveScrollElementPrefab);
            newSavesScrollElement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = gameSave.Name;
            newSavesScrollElement.transform.Find("DataTimeText").GetComponent<TextMeshProUGUI>().text = gameSave.DateTime;
            
            newSavesScrollElement.GetComponent<Button>().onClick
                .AddListener(() => ChooseGameSave(gameSave, newSavesScrollElement));
            savesScroll.AddElementBegin(newSavesScrollElement);
        }
    }

    private void ChooseGameSave(GameSave save, RectTransform elem)
    {
        chosenSaveScrollElement = elem.gameObject;
        chosenSave = save;
        var button = elem.GetComponent<Button>();
        if (saveScrollClickedButton != null)
            saveScrollClickedButton.interactable = true;
        button.interactable = false;
        saveScrollClickedButton = button;
    }

    public void LoadChosenSave()
    {
        try
        {
            GameSession.Current = chosenGameSession;
            if (chosenSave == null)
                chosenGameSession.ContinueTheGame();
            else
                chosenSave.Load();
        }
        catch (Exception e)
        {
            warningPanelOnLoadingCorruptedFile.gameObject.SetActive(true);
            if (Game.IsDebug)
            {
                warningPanelOnLoadingCorruptedFile.WarningText = "Ошибка выведена в консоль";
                Debug.LogError(e.Message);
            }
        }
    }

    public void DeleteGameSessionCondition()
    {
        if (chosenGameSession == null || chosenSave != null)
            return;
        if (GameSession.Current != null && GameSession.Current.Equals(chosenGameSession))
        {
            warningPanelOnDeleteGameSession.gameObject.SetActive(true);
            return;
        }

        dialogPanelDeleteGameSession.gameObject.SetActive(true);
    }

    public void DeleteGameSession()
    {
        chosenGameSession.Delete();
        Destroy(choseGameScrollElement);
        savesScroll.Clear();
        chosenGameSession = null;
        choseGameScrollElement = null;
        chosenSave = null;
        chosenSaveScrollElement = null;
    }

    public void DeleteChosenSave()
    {
        if (chosenSave == null) return;
        if (GameSession.Current != null 
            && GameSession.Current.Equals(chosenGameSession)
            && chosenGameSession?.GameSaves.Count() == 1)
        {
            warningPanelOnDeleteLastSave.gameObject.SetActive(true);
            return;
        }

        chosenSave.Delete();
        Destroy(chosenSaveScrollElement);
        chosenSaveScrollElement = null;
        chosenSave = null;

        if (chosenGameSession?.GameSaves.Count() == 0)
            DeleteGameSession();
    }
}