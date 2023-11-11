using System.Runtime.Serialization;
using DataBase;
using UnityEngine;

namespace TheRevenantsAge
{
    [DataContract]
    public class GroupState
    {
        [DataMember] public string address;
        [DataMember] public CharacterState[] currentGroupMembers;
        [DataMember] public Vector2 position;
        [DataMember] public bool isLooting;
        [DataMember] public bool canMove;


        public Group InstantiateGameObj()
        {
            var pref = PrefabDataBase.Load<Group>(address);
            var gm = Object.Instantiate(pref);
            gm.Restore(this);
            return gm;
        }

        public void ValidOrException()
        {
            if (string.IsNullOrEmpty(address))
                throw new UnableContinueGameException();
            if (currentGroupMembers == null || currentGroupMembers.Length == 0)
                throw new UnableContinueGameException();

            foreach (var member in currentGroupMembers)
                member.ValidOrException();
        }
    }
}