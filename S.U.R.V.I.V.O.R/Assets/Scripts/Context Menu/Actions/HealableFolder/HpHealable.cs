using TheRevenantsAge;

namespace Context_Menu.Actions
{
    public class HpHealable : Healable
    {
        public new void Awake()
        {
            base.Awake();
            ButtonText = "Вылечить хп";
        }
        
        public override void InvokeHealMethod(ICharacter character)
        {
            if(item.BaseItem.ItemOwner != null)
                item.BaseItem.ItemOwner.Heal(currentMedicine,character,false,true);
            else
                character.Heal(currentMedicine,character,false,true);
        }
    }
}