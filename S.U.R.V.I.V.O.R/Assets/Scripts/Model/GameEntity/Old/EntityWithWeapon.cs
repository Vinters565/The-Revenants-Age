// using Model.Items.Weapons;
//
// namespace Model.GameEntity
// {
//     public class EntityWithWeapon: Entity
//     {
//         protected Weapon chosenWeapon;
//         public Weapon ChosenWeapon => chosenWeapon;
//         public void ANIMATION_EVENT_ActivateWeapon()
//         {
//             if (chosenWeapon == null)
//                 return;
//             chosenWeapon.IsActive = true;
//         }
//         
//         public void ANIMATION_EVENT_DeactivateWeapon()
//         {
//             if (chosenWeapon == null)
//                 return;
//             chosenWeapon.IsActive = false;
//         }
//     }
// }