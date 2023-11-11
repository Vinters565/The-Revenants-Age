using TheRevenantsAge;

namespace Context_Menu.Actions
{
    public class FullHealable : Healable
    {
        public new void Awake()
        {
            base.Awake();
            ButtonText = "Вылечить всё";
        }
        
        public override void InvokeHealMethod(ICharacter character)
        {
            if(item.BaseItem.ItemOwner != null)
                item.BaseItem.ItemOwner.Heal(currentMedicine,character,true,true);
            else
                character.Heal(currentMedicine,character,true,true);
        }
    }
}