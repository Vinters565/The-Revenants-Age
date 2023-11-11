using System.Collections;
using System.Globalization;
using System.Text;
using Extension;
using Interface.Menu.ForConsole;

public static class BitArrayExtension
{
    public static int GetCardinality(this BitArray bitArray)
    {
        System.Int32[] ints = new System.Int32[(bitArray.Count >> 5) + 1];

        bitArray.CopyTo(ints, 0);

        System.Int32 count = 0;

        // fix for not truncated bits in last integer that may have been set to true with SetAll()
        ints[ints.Length - 1] &= ~(-1 << (bitArray.Count % 32));

        for (System.Int32 i = 0; i < ints.Length; i++)
        {
            System.Int32 c = ints[i];

            // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
            unchecked
            {
                c = c - ((c >> 1) & 0x55555555);
                c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
            }

            count += c;
        }

        return count;
    }
    
    public static string ToHex(this BitArray bits)
    {
        StringBuilder sb = new StringBuilder(bits.Length / 4);
        for (int i = 0; i < bits.Length; i += 4)
        {
            int v = (bits[i] ? 8 : 0) |
                    (bits[i + 1] ? 4 : 0) |
                    (bits[i + 2] ? 2 : 0) |
                    (bits[i + 3] ? 1 : 0);

            sb.Append(v.ToString("X1"));
        }

        return sb.ToString();
    }
    
    public static string ToString(this BitArray bitArray)
    {
        var builder = new StringBuilder(bitArray.Length);
        for (int i = 0; i < bitArray.Length; i++)
            builder.Append(bitArray[i] ? "1" : "0");
        return builder.ToString();
    }

    public static BitArray AddInsignificantZeros(this BitArray bitArray, int count)
    {
        var newBitArray = new BitArray(bitArray.Length + count);
        var j = 0;
        for (var i = count; i < newBitArray.Length; i++)
        {
            newBitArray[i] = bitArray[j];
            j++;
        }

        return newBitArray;
    }
    public static BitArray RemoveBitsInRange(this BitArray bitArray, int from, int len)
    {
        var removed = len;
        var newBitArray = new BitArray(bitArray.Length - removed);
        var j = 0;
        for (var i = 0; i < from; i++)
        {
            newBitArray[j] = bitArray[i];
            j++;
        }

        for (var i = from + len; i < bitArray.Length; i++)
        {
            newBitArray[j] = bitArray[i];
            j++;
        }

        return newBitArray;
    }
}