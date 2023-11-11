using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class ManHead : BodyPathWearableClothes
    {
        public Clothes Hat => clothesDict?[ClothType.Hat];
    }
}