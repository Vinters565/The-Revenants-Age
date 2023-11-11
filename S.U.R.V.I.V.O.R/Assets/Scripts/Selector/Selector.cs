using System;
using System.Collections;
using System.Collections.Generic;
using Interface.Menu.ForConsole;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;

public class Selector : MonoBehaviour
{
    public static Selector Instance { get; private set; }
    
    public static readonly List<Selectable> Units = new ();
    private static List<Selectable> unitSelected;

    [SerializeField] private GUISkin skin;
    
    private Rect rect;
    private bool draw;
    private Vector2 startPos;
    private Vector2 endPos;
    private Camera mainCamera;

    private bool needDeactivate;

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
        unitSelected = new List<Selectable>();
        mainCamera = Camera.main;
    }

    private void Select()
    {
        foreach (var unit in unitSelected)
            unit.Selected();
    }

    private void Deselect()
    {
        foreach (var unit in unitSelected)
            unit.Deselected();
    }

    private void OnGUI()
    {
        if (needDeactivate && !draw)
        {
            DeactivateImmediate();
            return;
        }
        GUI.skin = skin;
        GUI.depth = 99;

        if (Input.GetMouseButtonDown(0))
        {
            Deselect();
            startPos = Input.mousePosition;
            draw = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            draw = false;
            Select();
        }

        if (draw)
            DrawRectangleAndSelectUnits();
    }


    private void DrawRectangleAndSelectUnits()
    {
        unitSelected.Clear();
        endPos = Input.mousePosition;
        if (startPos == endPos) return;

        rect = new Rect(Mathf.Min(endPos.x, startPos.x),
            Screen.height - Mathf.Max(endPos.y, startPos.y),
            Mathf.Max(endPos.x, startPos.x) - Mathf.Min(endPos.x, startPos.x),
            Mathf.Max(endPos.y, startPos.y) - Mathf.Min(endPos.y, startPos.y)
        );

        GUI.Box(rect, "");
        SelectUnitsInRectangle(rect);
    }

    private void SelectUnitsInRectangle(Rect rect)
    {
        foreach (var unit in Units)
        {
            var pos = unit.transform.position;
            var tmp = new Vector2(mainCamera.WorldToScreenPoint(pos).x,
                Screen.height - mainCamera.WorldToScreenPoint(pos).y);
            if (rect.Contains(tmp))
                unitSelected.Add(unit);
        }
    }

    public static void Activate()
    {
        Instance.needDeactivate = false;
        Instance.gameObject.SetActive(true);
    }
    
    public static void Deactivate()
    {
        if (Instance.draw)
            Instance.needDeactivate = true;
        else
            DeactivateImmediate();
    }

    public static void DeactivateImmediate()
    {
        Instance.needDeactivate = false;
        Instance.gameObject.SetActive(false);
    }
    

    private void OnDisable()
    {
        draw = false;
        startPos = endPos = Vector2.zero;
    }
}