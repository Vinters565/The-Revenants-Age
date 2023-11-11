using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Context_Menu;
using Inventory;
using TheRevenantsAge;
using UnityEngine;

[RequireComponent(typeof(ContextMenuItem))]
public class DetailedMagazine : MonoBehaviour, IContextMenuAction
{
    public string ButtonText { get; private set; }
    public bool Extendable { get; private set; }

    public AudioClip SoundOnClick { get; }

    private Magazine currentMagazine;

    public void Awake()
    {
        ButtonText = "Подробнее";
        Extendable = false;
        currentMagazine = GetComponent<Magazine>();
    }

    public void OnButtonClickAction<T>(T value)
    {
        MagazineWindow.Instance.ShowWindow(currentMagazine);
    }
}