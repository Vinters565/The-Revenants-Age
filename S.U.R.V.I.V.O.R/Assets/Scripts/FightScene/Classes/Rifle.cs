using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// public class Rifle : MonoBehaviour, IGun
// {
//     public int Damage {get;set;}

//     public Rifle(int damage)
//     {
//         Damage = damage;
//     }

//     public void Shoot(GameObject characterObj, GameObject targetObj, string targetName)
//     {
//         var head = characterObj.transform.Find("Head");
//         var target = targetObj.transform.Find(targetName);
//         var direction = (target.transform.position) - head.transform.position;
//         var ray = new Ray(head.transform.position, direction);
//         if (Physics.Raycast(ray, out var hit))
//         {
//             if(hit.transform.gameObject.tag == "Character")
//             {
//                 var enemy = targetObj.GetComponent<Character>();
//                 if (enemy == null)
//                     throw new Exception("Не обнаружен компонент <Character>");

//                 enemy.Health -= Damage;
//                 Debug.Log(enemy.Health);

//                 if (enemy.Health <= 0)
//                 {
//                     enemy.Alive = false;
//                     Destroy(targetObj);
//                     Debug.Log("Death");
//                 }
//             }
//         }
//     }
// }
