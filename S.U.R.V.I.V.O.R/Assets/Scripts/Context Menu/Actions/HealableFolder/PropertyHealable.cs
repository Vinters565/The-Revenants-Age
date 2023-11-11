using TheRevenantsAge;

namespace Context_Menu.Actions
{
    public class PropertyHealable : Healable
    {
        public new void Awake()
        {
            base.Awake();
            ButtonText = "Вылечить свойства";
        }
        
        public override void InvokeHealMethod(ICharacter character)
        {
            if(item.BaseItem.ItemOwner != null)
                item.BaseItem.ItemOwner.Heal(currentMedicine,character,true,false);
            else
                character.Heal(currentMedicine,character,true,false);
        }
    }
}