using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TestLoadTexture: MonoBehaviour
{
    [SerializeField] string _imageUrl;
    Texture2D _texture;

    async void Start ()
    {
        _texture = await GetRemoteTexture(_imageUrl);
    }

    void OnDestroy () => Dispose();
    public void Dispose () => Object.Destroy(_texture);// memory released, leak otherwise
    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = www.SendWebRequest();
            
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);
            
            if( www.result!=UnityWebRequest.Result.Success )
            {
                Debug.Log($"{www.error}, URL:{www.url}");
                return null;
            }
            else
            {
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}