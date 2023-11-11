using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TownNode : HardNode
{
    [SerializeField] private Town town;
    [field: SerializeField] public override Sprite LootButtonSprite { get; protected set; }
    
    [field: SerializeField] public override string Description {  get; protected set;  }
    public override void OnLootButtonClick()
    {
        if(town != null)
            town.Open();
    }
}
