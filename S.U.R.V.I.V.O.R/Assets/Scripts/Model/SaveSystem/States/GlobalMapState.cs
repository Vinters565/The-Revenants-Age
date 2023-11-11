using System.Collections.Generic;
using System.Runtime.Serialization;
using TheRevenantsAge;
using UnityEngine;

[assembly: ContractNamespace("", ClrNamespace = "")]
namespace TheRevenantsAge
{
    [DataContract]
    public class GlobalMapState
    {
        [DataMember] public int mainGroupIndex;
        [DataMember] public int mainCharacterIndex; 
        [DataMember] public int turnNumber;
        [DataMember] public List<GroupState> groups;
        [DataMember] public int chosenGroupIndex;
        [DataMember] public ItemState[] locationInventory;
        [DataMember] public Vector3 cameraPosition;
        [DataMember] public float zoomHeight;

        public GroupState ChosenGroup => groups[chosenGroupIndex];
        public void ValidOrException()
        {
            if (turnNumber < 1)
                throw new UnableContinueGameException();
            if (groups == null || groups.Count == 0)
                throw new UnableContinueGameException();
            if (chosenGroupIndex < 0 || chosenGroupIndex >= groups.Count)
                throw new UnableContinueGameException();
            foreach (var groupState in groups)
                groupState.ValidOrException();
        }
    }
}