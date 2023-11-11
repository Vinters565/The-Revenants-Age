using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StaticObjectsLoader : MonoBehaviour
{
   private void Awake()
   {
      SceneManager.LoadScene("StaticFightElements", LoadSceneMode.Additive);
   }
}
