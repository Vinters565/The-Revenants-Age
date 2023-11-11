using System;
using UnityEngine;

namespace Extension
{
    public static class Vector2Extension
    {
        public static Vector3 To3D(this Vector2 v2, float height = 0) => new Vector3(v2.x, height, v2.y);

        public static Vector2 Round(this Vector2 v2, int numbersOfRound)
        {
            return new Vector2(
                (float) Math.Round(v2.x, numbersOfRound),
                (float) Math.Round(v2.y, numbersOfRound));
        }
    }
}