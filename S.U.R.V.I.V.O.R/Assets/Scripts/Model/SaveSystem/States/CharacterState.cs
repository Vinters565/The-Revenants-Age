using System.Runtime.Serialization;
using DataBase;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [DataContract]
    public class CharacterState
    {
        [DataMember] public string address2D;
        [DataMember] public string address3D;
        [DataMember] public string firstName;
        [DataMember] public string surName;
        [DataMember] public ManBodyState manBody;
        [DataMember] public SkillsData skills;

        [DataMember] public ItemState hat;
        [DataMember] public ItemState underwear;
        [DataMember] public ItemState jacket;
        [DataMember] public ItemState backpack;
        [DataMember] public ItemState vest;
        [DataMember] public ItemState boots;
        [DataMember] public ItemState pants;

        [DataMember] public ItemState primaryGun;
        [DataMember] public ItemState secondaryGun;
        [DataMember] public ItemState meleeWeapon;

        [DataMember] public CharacterStatistics characterStatistics;
        [DataMember] public CharacterCharacteristics characteristics;

        public GameObject InstantiateGameObj3D()
        {
            var pref = PrefabDataBase.Load<GameObject>(address3D) ?? BaseCharacter.Default3DPrefab;
            var gm = Object.Instantiate(pref);
            gm.GetComponent<IFightCharacter>().Restore(this);
            return gm;
        }

        public GameObject InstantiateGameObj2D()
        {
            var pref = PrefabDataBase.Load<GameObject>(address2D) ?? BaseCharacter.Default2DPrefab;
            var gm = Object.Instantiate(pref);
            gm.GetComponent<IGlobalMapCharacter>().Restore(this);
            return gm;
        }

        public void ValidOrException()
        {
            //TODO: continue
        }
    }
}