using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UIController : MonoBehaviour
{
    private const float DAMAGE_INFO_LIFESPAN = 0.05f;
    private const int DAMAGE_INFO_LIFESPAN_PERIOD_COUNT = 15;
    private const float DAMAGE_INFO_PERIOD_OFFSET = 0.075f;
    private readonly Color allyQueueCardColor = new Color(0, 1, 0, 0.7f);
    private readonly Color enemyQueueCardColor = new Color(1,0,0,0.7f);
    
    public static UIController Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private LayoutGroup cardsLayoutGroup;
    [SerializeField] private Transform queuePanel;
    [SerializeField] private ActionPointsView actionPointsView;
    [SerializeField] private GameObject winPanel;
    
    [Space(10)]
    [Header("UI Prefabs")]
    [SerializeField] private GameObject queueCardPrefab;
    [SerializeField] private GameObject groupCharacterCardPrefab;
    [SerializeField] private GameObject damageInfoPrefab;
    
    [Space(10)]
    [Header("Phase Buttons")]
    [SerializeField] private Button moveButton;
    [SerializeField] private Button shootButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button reloadButton;
    [SerializeField] private Button endTurnButton;
    
    [Space(10)]
    [Header("Weapons Buttons")]
    [SerializeField] private Button primaryGunButton;
    [SerializeField] private Button secondaryGunButton;
    [SerializeField] private Button meleeWeaponButton;

    private new Camera camera;
    private Queue<GameObject> cardsQueue = new ();
    private List<GameObject> groupCards = new ();
    private Vector3 uiScale = Vector3.one;
    
    public float DamageInfoLiveSpan => DAMAGE_INFO_LIFESPAN;
    public int DamageInfoLiveSpanPeriodCount => DAMAGE_INFO_LIFESPAN_PERIOD_COUNT;
    
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject); 
        }
    }

    public void CreateUI()
    {
        camera = Camera.main;
        CreateQueueCards();           //Order is important
        CreateGroupCharactersCards();
    }

    private void CreateGroupCharactersCards()
    {
        foreach (var fCharacter in FightSceneController.Instance.FightEntityObjs
                     .Select(obj => obj.GetComponent<FightSceneCharacter>())
                     .Where(fCharacter => fCharacter.Type == CharacterType.Ally))
        {
            var card = Instantiate(groupCharacterCardPrefab, cardsLayoutGroup.GetComponent<Transform>());
            var entityCharacter = fCharacter.Entity as IFightCharacter;
            card.GetComponent<FightCharacterCard>().CurrentCharacter = entityCharacter;
            groupCards.Add(card);
        }

    }

    private void CreateQueueCards()
    {
        foreach (var fightCharacter in FightSceneController.CharactersQueue
                     .Select(obj => obj.GetComponent<FightSceneCharacter>()))
        {
            var queueCard = Instantiate(queueCardPrefab, parent:queuePanel.transform);
            queueCard.GetComponent<Image>().color = (fightCharacter.Type == CharacterType.Ally) 
                                            ? allyQueueCardColor : enemyQueueCardColor;
            
            queueCard.GetComponent<UIQueueCard>().SetFightSceneCharacter(fightCharacter);
            
            cardsQueue.Enqueue(queueCard);
        }
    }

    public void ChangeActiveCard()
    {
        var active = cardsQueue.Dequeue();
        active.transform.SetParent(null);
        active.transform.SetParent(queuePanel);
        cardsQueue.Enqueue(active);
    }

    public void DeleteDeathCharacterCard(FightSceneCharacter sceneCharacter)
    {
        var newQueue = new Queue<GameObject>();
        while(cardsQueue.Count > 0)
        {
            var currentCard = cardsQueue.Dequeue();
            if(sceneCharacter == currentCard.GetComponent<UIQueueCard>().CurrentFightSceneCharacter)
            {
                currentCard.transform.parent = null;
                Destroy(currentCard);
            }
            else
                newQueue.Enqueue(currentCard);
        }
        cardsQueue = newQueue;
    }

    public void DrawDamage(float value, Vector3 pos)
    {
        Debug.Log("DamageInfo");
        var textPanel = Instantiate(damageInfoPrefab).transform;
        //textPanel.AddComponent<TextMeshPro>();
        textPanel.Find("DamageToBody").GetComponent<TextMeshPro>().text = Math.Round(value, 2).ToString();
        textPanel.Find("DamageToArmor").GetComponent<TextMeshPro>().text = "";//info.damageToArmor.ToString();
        textPanel.position = pos;
        textPanel
            .LookAt(new Vector3(camera.transform.position.x, textPanel.transform.position.y,
                camera.transform.position.z));
        textPanel.Rotate(new Vector3(0, 180f, 0));
        textPanel.Translate(Vector3.back);
        textPanel.localScale = Vector3.one * (Vector3.Distance(camera.transform.position, textPanel.position) / 10);
        StartCoroutine(DeleteDamageTextPanel(textPanel));
    }

    private IEnumerator DeleteDamageTextPanel(Transform textPanel)
    {
        for (var i = 0; i < DamageInfoLiveSpanPeriodCount; i++)
        {
            yield return new WaitForSeconds(DamageInfoLiveSpan);
            textPanel.Translate(Vector3.up * DAMAGE_INFO_PERIOD_OFFSET);
        }
        Destroy(textPanel.gameObject);
    }

    private void ShiftCards(int startShiftIndex = 0)
    {
        var drawShiftCards = 0;
        foreach(var card in cardsQueue)
        {
            if(drawShiftCards >= startShiftIndex)
            {
                card.transform.Translate(new Vector3(0, -card.GetComponent<RectTransform>().rect.height / uiScale.y, 0));
            }
            drawShiftCards++;
        }
    }

    public void SetEnabledShootButton(bool isActive)
    {
        shootButton.enabled = isActive;
    }
    
    public void SetEnabledHitButton(bool isActive)
    {
        hitButton.enabled = isActive;
    }

    public void RedrawActionsPoints(int current, int max)
    {
        actionPointsView.Redraw(current, max);
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        cardsLayoutGroup.gameObject.SetActive(false);
        queuePanel.gameObject.SetActive(false);
        actionPointsView.gameObject.SetActive(false);
    }
}
