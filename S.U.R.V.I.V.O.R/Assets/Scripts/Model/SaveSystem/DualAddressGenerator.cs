using System;

namespace TheRevenantsAge
{
    public class DualAddressGenerator
    {
        public (string, string) GenerateDualAddress(string originalAddress)
        {
            if (string.IsNullOrEmpty(originalAddress)) throw new ArgumentException();

            var address2D = "";
            var address3D = "";

            if (originalAddress.Length < 2)
                address2D = originalAddress;
            else switch (originalAddress[^2..])
            {
                case Game.PREFAB_2D_POSTFIX:
                    address2D = originalAddress;
                    address3D = originalAddress.Remove(originalAddress.Length - 2) + Game.PREFAB_3D_POSTFIX;
                    break;
                case Game.PREFAB_3D_POSTFIX:
                    address2D = originalAddress.Remove(originalAddress.Length - 2) + Game.PREFAB_2D_POSTFIX;
                    address3D = originalAddress;
                    break;
                default:
                    address2D = originalAddress;
                    break;
            }

            return (address2D, address3D);
        }
    }
}