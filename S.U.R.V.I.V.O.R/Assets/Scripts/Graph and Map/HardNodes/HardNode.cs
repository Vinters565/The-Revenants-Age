using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HardNode : MonoBehaviour
{
    public abstract Sprite LootButtonSprite { get; protected set; }
    public abstract void OnLootButtonClick();

    public abstract string Description { get; protected set; }
}
