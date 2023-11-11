using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase
{
    [DisallowMultipleComponent]
    public class Pointer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string address;
        public string Address => address;
    }
}