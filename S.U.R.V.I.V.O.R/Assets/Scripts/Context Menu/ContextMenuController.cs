using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using Context_Menu;
using TheRevenantsAge;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuController : MonoBehaviour
{
    public static ContextMenuController Instance { get; private set; }

    [SerializeField] private float paddingX;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject extendedMenuPrefab;
    [SerializeField] private Button extendedMenuButtonPrefab;
    private readonly List<Button> storedButtons = new List<Button>();
    public bool IsActive { get; private set; }
    private RectTransform extendedMenu;
    private float scaleFactor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
    }

    private void Init()
    {
        scaleFactor = TheRevenantsAge.GlobalMapController.Instance.MainCanvas.scaleFactor;
    }

    private void Update()
    {
        var inBoundsMainMenu = BoundaryCheckMouse(Input.mousePosition, mainMenu);
        var inBoundsExtendedMenu = extendedMenu != null && BoundaryCheckMouse(Input.mousePosition, extendedMenu);
        if (mainMenu.gameObject.activeSelf && !inBoundsMainMenu && !inBoundsExtendedMenu &&
            (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
        {
            Close();
        }
    }

    public void CreateContextMenu(List<IContextMenuAction> actions, Vector2 mousePosition)
    {
        mainMenu.transform.SetParent(canvas.transform);
        mainMenu.transform.SetAsLastSibling();
        foreach (var action in actions)
        {
            var button = CreateButton(buttonPrefab, mainMenu.transform, action.ButtonText);
            AdjustMenuWidth(mainMenu, button);
            button.onClick.AddListener(delegate
            {
                if (action.Extendable)
                {
                    extendedMenu =
                        InitializeExtendedMenu(button.transform.position, button.GetComponent<RectTransform>());
                    var extendedActions = action.GetValues();
                    foreach (ITuple extendedAction in extendedActions)
                    {
                        var extendedButton = CreateButton(extendedMenuButtonPrefab, extendedMenu.transform,
                            (string) extendedAction[1]);
                        AdjustMenuWidth(extendedMenu, extendedButton);
                        extendedButton.onClick.AddListener(delegate
                        {
                            action.OnButtonClickAction(extendedAction[0]);
                            AudioManager.Instance.PlayOneShotSFX(action.GetSoundOnClick());
                            Close();
                        });
                    }

                    AudioManager.Instance.PlayOneShotSFX(Sounds.GetUISoundEffect(UISoundEffectsType.DefaultButton));
                    
                    extendedMenu.transform.position = AdjustPositionExtendedMenu(button.GetComponent<RectTransform>());
                }
                else
                {
                    action.OnButtonClickAction((object) null);
                    AudioManager.Instance.PlayOneShotSFX(action.GetSoundOnClick());
                    Close();
                }
            });
            storedButtons.Add(button);
        }

        mainMenu.position = mousePosition;
        mainMenu.sizeDelta = new Vector2(mainMenu.sizeDelta.x,
            buttonPrefab.GetComponent<RectTransform>().rect.height * storedButtons.Count);
        mainMenu.position = AdjustPositionMainMenu();

        Show();
    }

    private Vector3 AdjustPositionMainMenu()
    {
        var inScreen = BoundaryCheckScreen(mainMenu);
        var positionMainMenu = mainMenu.position;
        if (!inScreen.Item2)
            positionMainMenu.y += mainMenu.rect.height * scaleFactor;
        positionMainMenu.x = Math.Clamp(positionMainMenu.x, 0,
            canvas.GetComponent<RectTransform>().rect.width - mainMenu.rect.width);
        return positionMainMenu / scaleFactor;
    }

    private Vector3 AdjustPositionExtendedMenu(RectTransform parentButton)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(extendedMenu);
        var positionExtendedMenu = extendedMenu.position;
        var inScreenExtend = BoundaryCheckScreen(extendedMenu.transform);
        if (!inScreenExtend.Item1)
            positionExtendedMenu.x -= (extendedMenu.rect.width + parentButton.rect.width) * scaleFactor;
        if (!inScreenExtend.Item2)
            positionExtendedMenu.y += (extendedMenu.rect.height - parentButton.rect.height) * scaleFactor;
        return positionExtendedMenu / scaleFactor;
    }

    private void AdjustMenuWidth(RectTransform menu, Button button)
    {
        var textComponent = button.transform.GetComponentInChildren<TextMeshProUGUI>();
        var buttonWidth = textComponent.preferredWidth + (paddingX * 2);
        if (menu.rect.width < buttonWidth)
        {
            var newSize = new Vector2(buttonWidth, menu.rect.height);
            menu.sizeDelta = newSize;
            button.GetComponent<RectTransform>().sizeDelta = newSize;
        }
    }

    private RectTransform InitializeExtendedMenu(Vector3 position, RectTransform parentButton)
    {
        if (extendedMenu != null) Destroy(extendedMenu.gameObject);
        position.x += parentButton.rect.width;
        return Instantiate(extendedMenuPrefab, position, Quaternion.identity, parentButton)
            .GetComponent<RectTransform>();
    }

    private Button CreateButton(Button prefab, Transform parent, String text)
    {
        var button = Instantiate(prefab, parent, true);
        button.transform.localScale = Vector3.one;
        var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;
        return button;
    }

    public void Clear()
    {
        foreach (var button in storedButtons)
        {
            Destroy(button.gameObject);
        }

        storedButtons.Clear();
    }

    private void Show()
    {
        if (IsActive)
            return;
        mainMenu.gameObject.SetActive(true);
        IsActive = true;
    }

    private void Close()
    {
        if (!IsActive)
            return;
        mainMenu.gameObject.SetActive(false);
        IsActive = false;
    }

    private bool BoundaryCheckMouse(Vector2 mousePosition, Transform menu)
    {
        var position = menu.position;
        var rect = menu.GetComponent<RectTransform>().rect;
        rect.size *= scaleFactor;
        return ((mousePosition.x > position.x) && (mousePosition.x < position.x + rect.width)) &&
               ((mousePosition.y < position.y) && (mousePosition.y > position.y - rect.height));
    }

    private (bool, bool) BoundaryCheckScreen(Transform menu)
    {
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var menuRect = menu.GetComponent<RectTransform>().rect;
        var menuPos = menu.position;
        menuRect.size *= scaleFactor;
        canvasRect.size *= scaleFactor;
        return ((menuPos.x > 0 && menuPos.x + menuRect.width < canvasRect.width),
            (menuPos.y < canvasRect.height && menuPos.y - menuRect.height > 0));
    }
}