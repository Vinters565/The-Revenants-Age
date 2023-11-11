using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class ManArm : BodyPathWearableClothes
    {
        public Clothes Underwear => clothesDict?[ClothType.Underwear];
        public Clothes Jacket => clothesDict?[ClothType.Jacket];
    }
}