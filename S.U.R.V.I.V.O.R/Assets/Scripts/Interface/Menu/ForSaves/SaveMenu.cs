using System.Linq;
using Interface;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
{
    private GameSave chosenSave;
    private GameObject chosenSaveScrollElement;

    [SerializeField] private TextMeshProUGUI currentSaveText;
    [SerializeField] private ScrollController savesController;
    [SerializeField] private RectTransform savesScrollElement;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private GameObject dialogPanelOnMatchingNameSaves;
    [SerializeField] private GameObject warningPanelOnDeleteLastSave;

    private void OnEnable()
    {
        currentSaveText.text = GameSession.Current.Name;
        foreach (var gameSave in GameSession.Current.GameSaves)
            AddSaveElement(gameSave);
    }

    private void OnDisable()
    {
        currentSaveText.text = "";
        savesController.Clear();
        chosenSave = null;
        chosenSaveScrollElement = null;
    }

    public void CreateSaveCondition()
    {
        if (string.IsNullOrEmpty(input.text)) return;
        if (GameSession.Current.GameSaves.Any(x => x.Name == input.text))
        {
            dialogPanelOnMatchingNameSaves.gameObject.SetActive(true);
        }
        else
            CreateSave();
    }

    public void ReSave()
    {
        CreateSave();
        Destroy(chosenSaveScrollElement);
    }
    
    
    public void CreateSave()
    {
        var gameSave = GameSession.Current.Save(input.text);
        input.text = "";
        AddSaveElement(gameSave);
    }

    public void DeleteSave()
    {
        if (chosenSave == null) return;
        if (GameSession.Current.GameSaves.Count() == 1)
        {
            warningPanelOnDeleteLastSave.gameObject.SetActive(true);
            return;
        }

        chosenSave.Delete();
        Destroy(chosenSaveScrollElement);
        chosenSaveScrollElement = null;
        chosenSave = null;
        input.text = "";
    }

    private void AddSaveElement(GameSave gameSave)
    {
        var newScrollElement = Instantiate(savesScrollElement);
        newScrollElement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = gameSave.Name;
        newScrollElement.transform.Find("DataTimeText").GetComponent<TextMeshProUGUI>().text = gameSave.DateTime;
        
        newScrollElement.GetComponent<Button>().onClick.AddListener(() =>
        {
            input.text = gameSave.Name;
            chosenSave = gameSave;
            chosenSaveScrollElement = newScrollElement.gameObject;
        });
        savesController.AddElementBegin(newScrollElement);
    }
}