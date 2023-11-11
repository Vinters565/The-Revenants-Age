using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class ManChest : BodyPathWearableClothes
    {
        public Clothes Underwear => clothesDict?[ClothType.Underwear];
        public Clothes Jacket => clothesDict?[ClothType.Jacket];
        public Clothes Backpack => clothesDict?[ClothType.Backpack];
        public Clothes Vest => clothesDict?[ClothType.Vest];
    }
}