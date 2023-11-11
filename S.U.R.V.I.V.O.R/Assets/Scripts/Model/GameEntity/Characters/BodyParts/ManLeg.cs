using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class ManLeg : BodyPathWearableClothes
    {
        public Clothes Boots => clothesDict?[ClothType.Boots];

        public Clothes Pants => clothesDict?[ClothType.Pants];
    }
}