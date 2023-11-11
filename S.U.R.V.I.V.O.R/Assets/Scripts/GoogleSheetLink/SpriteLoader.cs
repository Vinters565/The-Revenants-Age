#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleSheetLink
{
    public class SpriteLoader : MonoBehaviour
    {
        // public Sprite LoadSprite(string url)
        // {
        //     if (string.IsNullOrEmpty(url))
        //         return null;
        //     var texture = GetRemoteTexture(url);
        //     var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //     return sprite;
        //     StartCoroutine(GetRemoteTextureCoroutine("ff"));
        //     
        // }
        
        IEnumerator GetRemoteTextureCoroutine(string url)
        {
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            var asyncOp = www.SendWebRequest();
            while (asyncOp.isDone == false)
            {
                yield return null;
            }
            if (www.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}
#endif