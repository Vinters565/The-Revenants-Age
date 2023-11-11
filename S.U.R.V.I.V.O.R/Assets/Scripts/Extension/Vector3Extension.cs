using System;
using UnityEngine;

namespace Extension
{
    public static class Vector3Extension
    {
        public static Vector2 To2D(this Vector3 v3) => new Vector2(v3.x, v3.z);
        
        public static Vector3 Round(this Vector3 v3, int numbersOfRound)
        {
            return new Vector3(
                (float) Math.Round(v3.x, numbersOfRound),
                (float) Math.Round(v3.y, numbersOfRound),
                (float) Math.Round(v3.z, numbersOfRound));
        }
    }
}